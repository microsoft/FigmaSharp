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

namespace MonoDevelop.Figma.FigmaBundles
{
	public partial class BundleUpdateWindow : AppKit.NSWindow
	{
		private FigmaFileVersion[] versions = new FigmaFileVersion[0];
		private FigmaVersionMenu versionMenu = new FigmaVersionMenu();

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
			this.Close();
		}

		private void UpdateButton_Activated(object sender, System.EventArgs e)
		{
			this.Close();
		}

		internal async void Load (FigmaBundle bundle)
		{
			loadingProgressIndicator.Hidden = false;
			loadingProgressIndicator.StartAnimation(loadingProgressIndicator);

			BundlePopUp.RemoveAllItems();

			//loads current versions
			versionComboBox.RemoveAllItems();

			RefreshStates();

			var mainLayersTask = Task.Run(() => {
				var remoteFile = new FigmaRemoteFileProvider();
				remoteFile.Load(bundle.FileId);
				return remoteFile.GetMainLayers();
			});
			
			var versionTask = Task.Run(() => {
				try
				{
					var query = new FigmaFileVersionQuery(bundle.FileId);
					var figmaFileVersions = FigmaSharp.AppContext.Api.GetFileVersions(query).versions;
					var result = figmaFileVersions.GroupBy(x => x.created_at)
						.Select(group => group.First())
						.ToArray();
					return result;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					return null;
				}
			});

			await Task.WhenAll (mainLayersTask, versionTask);

			var remoteMainViews = await mainLayersTask;
			versions = await versionTask;

			foreach (var figmaNode in remoteMainViews) {
				figmaNode.TryGetNodeCustomName(out var name);
				BundlePopUp.AddItem(name);
			}

			loadingProgressIndicator.StopAnimation(loadingProgressIndicator);
			loadingProgressIndicator.Hidden = true;

			if (versions != null && versions.Length > 0) {
				foreach (var version in versions) {
					versionMenu.AddItem (version);
				}
			}

			versionMenu.Generate(versionComboBox.Menu);
		
			versionComboBox.Enabled = true;

			RefreshStates();
		}

		private void RefreshStates()
		{
			BundlePopUp.Enabled = BundlePopUp.ItemCount > 0;
			versionComboBox.Enabled = versionComboBox.ItemCount > 0;

			UpdateButton.Enabled = BundlePopUp.Enabled && versionComboBox.Enabled && BundlePopUp.SelectedItem != null && versionComboBox.SelectedItem != null;
		}
	}
}
