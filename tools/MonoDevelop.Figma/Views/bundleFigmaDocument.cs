/*
*/
using System;
using AppKit;
using System.Linq;
using FigmaSharp.Models;
using System.Threading.Tasks;
using FigmaSharp;
using FigmaSharp.Cocoa;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using FigmaSharp.Services;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;

namespace MonoDevelop.Figma.FigmaBundles
{
	public partial class bundleFigmaDocument : AppKit.NSWindow
	{
		public string FileId => figmaUrlTextField.StringValue;

		Project currentProject;

		public bundleFigmaDocument (Project currentProject)
		{
			InitializeComponent ();

			this.currentProject = currentProject;

			figmaUrlTextField.Changed += FigmaUrlTextField_Changed;

			versionComboBox.AutoEnablesItems = false;
			versionComboBox.Activated += ItemsRefreshState_Handler;

			templateCodeOptionBox.Activated += ItemsRefreshState_Handler;
			templateMarkUpOptionBox.Activated += ItemsRefreshState_Handler;
			templateNoneOptionBox.Activated += ItemsRefreshState_Handler;

			cancelButton.Activated += CancelButton_Activated;
			bundleButton.Activated += BundleButton_Activated;

			templateNoneOptionBox.Enabled =
			templateMarkUpOptionBox.Enabled = false;

			versionMenu.VersionSelected += (s, e) => {
				SelectedFileVersion = e;
			};

			RefreshStates();
		}

		readonly FigmaVersionMenu versionMenu = new FigmaVersionMenu();

		private FigmaFileVersion[] versions = new FigmaFileVersion[0];

		public FigmaFileVersion SelectedFileVersion { get; private set; }

		void RefreshStates (bool enable = true)
		{
			figmaUrlTextField.Enabled = enable;

			templateCodeOptionBox.Enabled =
			//TemplateNoneOptionBox.Enabled =
			//TemplateMarkUpOptionBox.Enabled =
			versionComboBox.Enabled = enable && versions.Length > 0;

			RefreshBundleButtonState (enable);
		}

		void RefreshBundleButtonState (bool enable = true)
		{
			bundleButton.Enabled = enable &&
				versionComboBox.Enabled && (templateCodeOptionBox.State == NSCellStateValue.On || templateMarkUpOptionBox.State == NSCellStateValue.On || templateNoneOptionBox.State == NSCellStateValue.On);
		}

		private async void BundleButton_Activated (object sender, EventArgs e)
		{
			var includeImages = true;
			ShowLoading(true);
			RefreshStates(false);

			var namesSpace = currentProject.GetDefaultFigmaNamespace ();
			await GenerateBundle(FileId, SelectedFileVersion, namesSpace, includeImages);

			RefreshStates(true);
			ShowLoading(false);

			PerformClose(this);
		}

		async Task GenerateBundle(string fileId, FigmaSharp.Models.FigmaFileVersion version, string namesSpace, bool includeImages)
		{
			var currentBundle = await Task.Run(() =>
			{
				//we need to ask to figma server to get nodes as demmand
				var fileProvider = new FigmaRemoteFileProvider();
				fileProvider.Load(fileId);

				//var bundleName = $"MyTestCreated{FigmaBundle.FigmaBundleDirectoryExtension}";
				var bundle = currentProject.CreateBundle(fileId, version, fileProvider, namesSpace);

				//to generate all layers we need a code renderer
				var codeRendererService = new NativeViewCodeService(fileProvider);
				bundle.SaveAll(codeRendererService, includeImages);

				return bundle;
			});

			//now we need to add to Monodevelop all the stuff
			await currentProject.IncludeBundle(currentBundle, includeImages)
				.ConfigureAwait(true);
		}

		private void CancelButton_Activated (object sender, EventArgs e)
		{
			this.Close ();
		}

		private void ItemsRefreshState_Handler (object sender, EventArgs e)
		{
			RefreshStates ();
		}

		void ShowLoading (bool value)
		{
			if (value)
			{
				loadingProgressIndicator.Hidden = false;
				loadingProgressIndicator.StartAnimation(loadingProgressIndicator);
			} else
			{
				loadingProgressIndicator.StopAnimation(loadingProgressIndicator);
				loadingProgressIndicator.Hidden = true;
			}
		}

		private async void FigmaUrlTextField_Changed (object sender, EventArgs e)
		{
			ShowLoading(true);

			SelectedFileVersion = null;

			//loads current versions
			versionComboBox.RemoveAllItems ();
			versionComboBox.Enabled = false;

			RefreshStates ();

			if (FigmaApiHelper.TryParseFileUrl (FileId, out string fileId)) {
				figmaUrlTextField.StringValue = fileId;
			}

			versions = await Task.Run (() => {
				try {
					var query = new FigmaFileVersionQuery (fileId);
					var figmaFileVersions = FigmaSharp.AppContext.Api.GetFileVersions (query).versions;
					var result = figmaFileVersions.GroupBy (x => x.created_at)
						.Select (group => group.First ())
						.ToArray ();
					return result;
				} catch (Exception ex) {
					Console.WriteLine (ex);
					return null;
				}
			});

			ShowLoading(false);
		
			versionMenu.Clear ();

			if (versions != null) {
				foreach (var item in versions)
					versionMenu.AddItem(item);

				versionMenu.Generate(versionComboBox.Menu);

				versionComboBox.SelectItem(versionMenu.CurrentMenu);
				SelectedFileVersion = versionMenu.CurrentVersion;
			}

			RefreshStates ();
		}
	}
}
