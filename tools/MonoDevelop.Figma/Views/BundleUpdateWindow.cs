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

namespace MonoDevelop.Figma.FigmaBundles
{
	public partial class BundleUpdateWindow : AppKit.NSWindow
	{
		private FigmaFileVersion[] versions = new FigmaFileVersion[0];

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
			
			loadingProgressIndicator.Hidden = true;
			UpdateButton.Activated += UpdateButton_Activated;
			CancelButton.Activated += CancelButton_Activated;

			//versionMenu = new VersionMenu ()

		}

		VersionMenu versionMenu;

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
			BundlePopUp.RemoveAllItems();

			var remoteFile = new FigmaRemoteFileProvider();
			remoteFile.Load (bundle.FileId);

			var remoteMainViews = remoteFile.GetMainLayers();
			foreach (var figmaNode in remoteMainViews) {
				figmaNode.TryGetNodeCustomName(out var name);
				BundlePopUp.AddItem(name);
			}

			loadingProgressIndicator.Hidden = false;
			loadingProgressIndicator.StartAnimation(loadingProgressIndicator);

			//loads current versions
			versionComboBox.RemoveAllItems();
			versionComboBox.Enabled = false;

			versions = await Task.Run(() => {
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

			loadingProgressIndicator.StopAnimation(loadingProgressIndicator);
			loadingProgressIndicator.Hidden = true;

			if (versions != null)
			{
				var menu = new NSMenu();
				menu.AddItem(new NSMenuItem("Current"));

				if (versions.Length > 0)
				{
					menu.AddItem(NSMenuItem.SeparatorItem);

					foreach (FigmaFileVersion version in versions.Skip(1))
					{
						if (!string.IsNullOrEmpty(version.label))
							menu.AddItem(new NSMenuItem(version.label));
						else
							menu.AddItem(new NSMenuItem(version.created_at.ToString("f")));
					}
				}

				versionComboBox.Menu = menu;
				versionComboBox.SelectItem(0);
				menu.Update();
			}
			versionComboBox.Enabled = true;

			RefreshStates();
		}

		private void RefreshStates()
		{
			
		}
	}
}
