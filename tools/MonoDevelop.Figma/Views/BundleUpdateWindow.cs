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

namespace MonoDevelop.Figma.FigmaBundles
{
	public partial class BundleUpdateWindow : AppKit.NSWindow
	{
		private FigmaFileVersion[] versions = new FigmaFileVersion[0];
		private FigmaVersionMenu versionMenu = new FigmaVersionMenu();

		public event EventHandler NeedsUpdate;

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
			loadingProgressIndicator.Hidden = true;
			UpdateButton.Activated += UpdateButton_Activated;
			CancelButton.Activated += CancelButton_Activated;

			BundlePopUp.Activated += (s,e) => RefreshStates ();
			versionComboBox.Activated += (s, e) => RefreshStates();
		}

		private void CancelButton_Activated(object sender, System.EventArgs e)
		{
			PerformClose(this);
		}

		private async void UpdateButton_Activated(object sender, System.EventArgs e)
		{
			var version = versionMenu.GetFileVersion (versionComboBox.SelectedItem);

			var fileProvider = new FigmaRemoteFileProvider() { Version = version };
			fileProvider.Load (mainBundle.FileId);

			var codeRendererService = new NativeViewCodeService(fileProvider);

			var includeImages = true;

			mainBundle.Update (version, codeRendererService, includeImages: includeImages);

			await project.IncludeBundle(mainBundle, includeImages: includeImages);
			PerformClose(this);
		}

		FigmaBundle mainBundle;

		static IEnumerable<FigmaBundle> GetFromFigmaDirectory (string directory)
		{
			foreach (var item in Directory.EnumerateDirectories(directory)) {
				var bundle = FigmaBundle.FromDirectoryPath(item);
				if (bundle != null)
					yield return bundle;
			}
		}

		Projects.Project project;

		internal async void Load (FigmaBundle bundle, Projects.Project project)
		{
			this.mainBundle = bundle;
			this.project = project;

			loadingProgressIndicator.Hidden = false;
			loadingProgressIndicator.StartAnimation(loadingProgressIndicator);

			BundlePopUp.RemoveAllItems();

			//loads current versions
			versionComboBox.RemoveAllItems();

			RefreshStates();

			var versionTask = Task.Run(() => {
				try {
					var query = new FigmaFileVersionQuery(bundle.FileId);
					var figmaFileVersions = FigmaSharp.AppContext.Api.GetFileVersions(query)
						.versions;
					var result = figmaFileVersions.GroupBy(x => x.created_at)
						.Select(group => group.First())
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
				BundlePopUp.AddItem(figmaNode.Manifest.DocumentTitle);
			}

			loadingProgressIndicator.StopAnimation(loadingProgressIndicator);
			loadingProgressIndicator.Hidden = true;

			if (versions != null && versions.Length > 0) {
				foreach (var version in versions) {
					versionMenu.AddItem (version);
				}
			}

			versionMenu.Generate(versionComboBox.Menu);

			//select current version
			var menu = versionMenu.GetMenuItem (bundle.Version);
			versionComboBox.SelectItem(menu);

			versionComboBox.Enabled = true;

			RefreshStates();
		}

		private void RefreshStates()
		{
			//BundlePopUp.Enabled = BundlePopUp.ItemCount > 0;
			//versionComboBox.Enabled = versionComboBox.ItemCount > 0;

			//UpdateButton.Enabled = BundlePopUp.Enabled && versionComboBox.Enabled && BundlePopUp.SelectedItem != null && versionComboBox.SelectedItem != null;
		}
	}
}
