/*
*/
using System;
using AppKit;
using System.Linq;
using FigmaSharp.Models;
using System.Threading.Tasks;
using FigmaSharp;
using FigmaSharp.Cocoa;
using MonoDevelop.Projects;
using FigmaSharp.Services;
using MonoDevelop.Ide;

namespace MonoDevelop.Figma.Packages
{
	public partial class FigmaPackageWindow : AppKit.NSWindow
	{
		public string FileId => figmaUrlTextField.StringValue;

		Project currentProject;

		public FigmaPackageWindow (Project currentProject)
		{
			InitializeComponent ();

			this.currentProject = currentProject;

			this.namespacePopUp.StringValue = currentProject.GetDefaultFigmaNamespace();

			figmaUrlTextField.Changed += FigmaUrlTextField_Changed;

			versionPopUp.AutoEnablesItems = false;
			versionPopUp.Activated += ItemsRefreshState_Handler;

			codeRadio.Activated += ItemsRefreshState_Handler;
			templateRadio.Activated += ItemsRefreshState_Handler;
			nothingRadio.Activated += ItemsRefreshState_Handler;

			cancelButton.Activated += CancelButton_Activated;
			bundleButton.Activated += BundleButton_Activated;

			nothingRadio.Enabled =
			templateRadio.Enabled = false;

			versionMenu.VersionSelected += (s, e) => {
				SelectedFileVersion = e;
			};

			RefreshStates();

			this.InitialFirstResponder = figmaUrlTextField;
		}

		readonly FigmaVersionMenu versionMenu = new FigmaVersionMenu();

		private FigmaFileVersion[] versions;

		public FigmaFileVersion SelectedFileVersion { get; private set; }

		void RefreshStates (bool enable = true)
		{
			figmaUrlTextField.Enabled = enable;

			namespacePopUp.Enabled =
			includeOriginalCheckbox.Enabled =
			codeRadio.Enabled =
			//TemplateNoneOptionBox.Enabled =
			//TemplateMarkUpOptionBox.Enabled =
			versionPopUp.Enabled = enable && versions != null;

			RefreshBundleButtonState (enable);
		}

		void RefreshBundleButtonState (bool enable = true)
		{
			bundleButton.Enabled = enable &&
				versionPopUp.Enabled && (codeRadio.State == NSCellStateValue.On || templateRadio.State == NSCellStateValue.On || nothingRadio.State == NSCellStateValue.On);
		}

		private async void BundleButton_Activated (object sender, EventArgs e)
		{
			var includeImages = true;
			ShowLoading(true);
			RefreshStates(false);

			await GenerateBundle(FileId, SelectedFileVersion, this.namespacePopUp.StringValue, includeImages);

			RefreshStates(true);
			ShowLoading(false);

			PerformClose(this);
		}

		async Task GenerateBundle(string fileId, FigmaFileVersion version, string namesSpace, bool includeImages)
		{
			IdeApp.Workbench.StatusBar.BeginProgress ($"Bundling {fileId}...");

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

			IdeApp.Workbench.StatusBar.EndProgress ();
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
				versionSpinner.Hidden = false;
				versionSpinner.StartAnimation(versionSpinner);
			} else
			{
				versionSpinner.StopAnimation(versionSpinner);
				versionSpinner.Hidden = true;
			}
		}

		private async void FigmaUrlTextField_Changed (object sender, EventArgs e)
		{
			ShowLoading(true);

			SelectedFileVersion = null;

			//loads current versions
			versionPopUp.RemoveAllItems ();

			RefreshStates ();

			if (FigmaApiHelper.TryParseFileUrl (FileId, out string fileId)) {
				figmaUrlTextField.StringValue = fileId;
			}

			versions = await Task.Run (() => {
				try {
					var query = new FigmaFileVersionQuery (fileId);
					var figmaFileVersions = FigmaSharp.AppContext.Api.GetFileVersions (query).versions;
					var result = figmaFileVersions
						.GroupByCreatedAt()
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

				versionMenu.Generate(versionPopUp.Menu);

				versionPopUp.SelectItem(versionMenu.CurrentMenu);
				SelectedFileVersion = versionMenu.CurrentVersion;
			}

			RefreshStates ();
		}
	}
}
