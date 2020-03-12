/*
*/
using System;
using System.Threading.Tasks;
using FigmaSharp.Models;
using System.Linq;
using FigmaSharp;
using AppKit;
using FigmaSharp.Services;
using Foundation;
using FigmaSharp.Cocoa;
using System.IO;
using System.Collections.Generic;
using MonoDevelop.Ide;

namespace MonoDevelop.Figma.FigmaBundles
{
	public partial class BundleUpdateWindow : AppKit.NSWindow
	{
		private FigmaFileVersion[] versions = new FigmaFileVersion[0];
		private FigmaVersionMenu versionMenu = new FigmaVersionMenu();

		public event EventHandler NeedsUpdate;

		FigmaBundle mainBundle;
		Projects.Project project;

		public FigmaFileVersion SelectedFileVersion {
			get {
				if (versionComboBox.ItemCount > 0 && versionComboBox.ItemCount == versions.Length + 1 && versionComboBox.IndexOfSelectedItem > -1 && versions.Length > 0) {
					return versions[(int)versionComboBox.IndexOfSelectedItem];
				}
				return null;
			}
		}

		public BundleUpdateWindow ()
		{
			InitializeComponent ();
			versionComboBox.AutoEnablesItems = false;
			versionSpinner.Hidden = true;
			updateButton.Activated += UpdateButton_Activated;
			cancelButton.Activated += CancelButton_Activated;
		}

		private void CancelButton_Activated(object sender, System.EventArgs e)
		{
			PerformClose(this);
		}

		void EnableViews (bool value)
		{
			versionComboBox.Enabled = bundlePopUp.Enabled =
				updateButton.Enabled = value;
		}

		private async void UpdateButton_Activated(object sender, System.EventArgs e)
		{
			EnableViews(false);
			ShowLoading(true);

			IdeApp.Workbench.StatusBar.BeginProgress($"Updating Bundle {mainBundle.FileId}...");

			var includeImages = true;

			var version = versionMenu.GetFileVersion(versionComboBox.SelectedItem);

			await Task.Run(() => {
			   var fileProvider = new FigmaRemoteFileProvider() { Version = version };
			   fileProvider.Load(mainBundle.FileId);
				Console.WriteLine($"[Done] Loaded Remote File provider for Version {version?.id ?? "Current"}");
			   var codeRendererService = new NativeViewCodeService(fileProvider);
			   mainBundle.Update(version, codeRendererService, includeImages: includeImages);
		   });

			await project.IncludeBundle(mainBundle, includeImages: includeImages);

			IdeApp.Workbench.StatusBar.EndProgress();

			PerformClose(this);
		}

		static IEnumerable<FigmaBundle> GetFromFigmaDirectory (string directory)
		{
			foreach (var item in Directory.EnumerateDirectories(directory)) {
				var bundle = FigmaBundle.FromDirectoryPath(item);
				if (bundle != null)
					yield return bundle;
			}
		}

		void ShowLoading (bool value)
		{
			if (value) {
				versionSpinner.Hidden = false;
				versionSpinner.StartAnimation(versionSpinner);
			} else {
				versionSpinner.StopAnimation(versionSpinner);
				versionSpinner.Hidden = true;
			}
		}

		internal async void Load (FigmaBundle bundle, Projects.Project project)
		{
			this.mainBundle = bundle;
			this.project = project;

			ShowLoading(true);

			EnableViews(false);

			bundlePopUp.RemoveAllItems();

			//loads current versions
			versionComboBox.RemoveAllItems();

			var versionTask = Task.Run(() => {
				try {
					var query = new FigmaFileVersionQuery(bundle.FileId);
					var figmaFileVersions = FigmaSharp.AppContext.Api.GetFileVersions(query)
						.versions;
					var result = figmaFileVersions
						.GroupByCreatedAt ()
						.ToArray();
					return result;
				} catch (Exception ex) {
					Console.WriteLine(ex);
					return null;
				}
			});

			var figmaDirectory = Path.GetDirectoryName(bundle.DirectoryPath);
			var currentProjectBundles = GetFromFigmaDirectory(figmaDirectory);

			versions = await versionTask;

			foreach (var figmaNode in currentProjectBundles) {
				bundlePopUp.AddItem(figmaNode.Manifest.DocumentTitle);
			}

			ShowLoading(false);
			EnableViews(true);

			if (versions != null && versions.Length > 0) {
				foreach (var version in versions) {
					versionMenu.AddItem (version);
				}
			}

			versionMenu.Generate(versionComboBox.Menu);

			//select current version
			var menu = versionMenu.GetMenuItem (bundle.Version);
			versionComboBox.SelectItem(menu);
		}
	}
}
