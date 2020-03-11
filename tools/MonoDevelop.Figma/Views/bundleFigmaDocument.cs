/*
*/
using System;
using AppKit;
using System.Linq;
using FigmaSharp.Models;
using System.Threading.Tasks;
using FigmaSharp;
using FigmaSharp.Cocoa;

namespace MonoDevelop.Figma.FigmaBundles
{
	public partial class bundleFigmaDocument : AppKit.NSWindow
	{
		public string FileId => figmaUrlTextField.StringValue;

		public bundleFigmaDocument ()
		{
			InitializeComponent ();

			figmaUrlTextField.Changed += FigmaUrlTextField_Changed;
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

		void RefreshStates ()
		{
			templateCodeOptionBox.Enabled =
			//TemplateNoneOptionBox.Enabled =
			//TemplateMarkUpOptionBox.Enabled =
			versionComboBox.Enabled = versions.Length > 0;

			RefreshBundleButtonState ();
		}

		void RefreshBundleButtonState ()
		{
			bundleButton.Enabled =
				versionComboBox.Enabled && (templateCodeOptionBox.State == NSCellStateValue.On || templateMarkUpOptionBox.State == NSCellStateValue.On || templateNoneOptionBox.State == NSCellStateValue.On);
		}

		public event EventHandler BundleCreated;

		private void BundleButton_Activated (object sender, EventArgs e)
		{
			BundleCreated?.Invoke (this, e);
		}

		private void CancelButton_Activated (object sender, EventArgs e)
		{
			this.Close ();
		}

		private void ItemsRefreshState_Handler (object sender, EventArgs e)
		{
			RefreshStates ();
		}

		private async void FigmaUrlTextField_Changed (object sender, EventArgs e)
		{
			loadingProgressIndicator.Hidden = false;
			loadingProgressIndicator.StartAnimation (loadingProgressIndicator);

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

			loadingProgressIndicator.StopAnimation (loadingProgressIndicator);
			loadingProgressIndicator.Hidden = true;

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
