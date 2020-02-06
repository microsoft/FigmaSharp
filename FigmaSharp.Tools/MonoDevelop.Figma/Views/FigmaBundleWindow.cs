/*
*/
using System;
using AppKit;
using FigmaSharp.Cocoa.Converters;
using FigmaSharp.Services;
using System.Linq;
using FigmaSharp.Models;
using Foundation;
using System.Threading.Tasks;
using FigmaSharp;

namespace MonoDevelop.Figma
{
	public partial class FigmaBundleWindow : AppKit.NSWindow
	{
		public string FileId => FigmaUrlTextField.StringValue;

		public FigmaBundleWindow ()
		{
			InitializeComponent ();

			Initialize ();
		}

		private FigmaFileVersion[] versions = new FigmaFileVersion[0];

		public FigmaFileVersion SelectedFileVersion {
			get {
				if (VersionComboBox.ItemCount > 0 && VersionComboBox.ItemCount == versions.Length && VersionComboBox.IndexOfSelectedItem > -1) {
					return versions[(int)VersionComboBox.IndexOfSelectedItem];
				}
				return null;
			}
		}

		// Shared initialization code
		void Initialize ()
		{
			FigmaUrlTextField.Changed += FigmaUrlTextField_Changed;
			VersionComboBox.Activated += ItemsRefreshState_Handler;

			TemplateCodeOptionBox.Activated += ItemsRefreshState_Handler;
			TemplateMarkUpOptionBox.Activated += ItemsRefreshState_Handler;
			TemplateNoneOptionBox.Activated += ItemsRefreshState_Handler;

			CancelButton.Activated += CancelButton_Activated;
			BundleButton.Activated += BundleButton_Activated;

			RefreshStates ();

		}

		void RefreshStates ()
		{
			TemplateCodeOptionBox.Enabled =
			//TemplateNoneOptionBox.Enabled =
			//TemplateMarkUpOptionBox.Enabled =
			VersionComboBox.Enabled = SelectedFileVersion != null;

			RefreshBundleButtonState ();
		}

		void RefreshBundleButtonState ()
		{
			BundleButton.Enabled =
				SelectedFileVersion != null && (TemplateCodeOptionBox.State == NSCellStateValue.On || TemplateMarkUpOptionBox.State == NSCellStateValue.On || TemplateNoneOptionBox.State == NSCellStateValue.On);
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
			LoadingProgressIndicator.Hidden = false;
			LoadingProgressIndicator.StartAnimation (LoadingProgressIndicator);

			//loads current versions
			VersionComboBox.RemoveAllItems ();
			VersionComboBox.Enabled = false;

			RefreshStates ();

			if (FigmaApiHelper.TryParseFileUrl (FileId, out string fileId)) {
				FigmaUrlTextField.StringValue = fileId;
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

			LoadingProgressIndicator.StopAnimation (LoadingProgressIndicator);
			LoadingProgressIndicator.Hidden = true;

			if (versions != null) {
				for (int i = 0; i < versions.Length; i++) {
					string title;
					if (i == 0) {
						title = "Current";
					} else {
						title = versions[i].created_at.ToString ("MM/dd/yyyy HH:mm:ss");
					}
					VersionComboBox.AddItem (title);
				}
			}

			RefreshStates ();
		}
	}
}
