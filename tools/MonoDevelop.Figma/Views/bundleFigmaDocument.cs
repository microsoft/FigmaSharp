//  * Copyright (C) 2020 Microsoft, Corp
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining
//  * a copy of this software and associated documentation files (the
//  * "Software"), to deal in the Software without restriction, including
//  * without limitation the rights to use, copy, modify, merge, publish,
//  * distribute, sublicense, and/or sell copies of the Software, and to permit
//  * persons to whom the Software is furnished to do so, subject to the
//  * following conditions:
//  * 
//  * The above copyright notice and this permission notice shall be included in
//  * all copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
//  * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//  * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
//  * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//  * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
//  * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
//  * USE OR OTHER DEALINGS IN THE SOFTWARE.
//  */
//
//

using System;
using System.Linq;
using System.Threading.Tasks;

using AppKit;

using FigmaSharp;
using FigmaSharp.Models;

namespace MonoDevelop.Figma.FigmaBundles
{
	public partial class bundleFigmaWindow : NSWindow
	{
		public string FileId => figmaUrlTextField.StringValue;

		public bundleFigmaWindow ()
		{
			InitializeComponent ();

			Initialize ();
		}

		FigmaFileVersion[] versions = new FigmaFileVersion[0];

		public FigmaFileVersion SelectedFileVersion {
			get {
				if (versionPopUp.ItemCount > 0 && versionPopUp.ItemCount == versions.Length + 1 && versionPopUp.IndexOfSelectedItem > -1) {
					return versions[(int)versionPopUp.IndexOfSelectedItem];
				}
				return null;
			}
		}

		// Shared initialization code
		void Initialize ()
		{
			figmaUrlTextField.Changed += FigmaUrlTextField_Changed;
			versionPopUp.Activated += ItemsRefreshState_Handler;

			codeRadio.Activated += ItemsRefreshState_Handler;
			templateRadio.Activated += ItemsRefreshState_Handler;
			nothingRadio.Activated += ItemsRefreshState_Handler;

			cancelButton.Activated += CancelButton_Activated;
			bundleButton.Activated += BundleButton_Activated;

			nothingRadio.Enabled =
			templateRadio.Enabled = false;

			RefreshStates ();
		}

		void RefreshStates ()
		{
			codeRadio.Enabled =
			//TemplateNoneOptionBox.Enabled =
			//TemplateMarkUpOptionBox.Enabled =
			versionPopUp.Enabled = SelectedFileVersion != null;

			RefreshBundleButtonState ();
		}

		void RefreshBundleButtonState ()
		{
			bundleButton.Enabled =
				SelectedFileVersion != null &&
				(codeRadio.State ==     NSCellStateValue.On ||
				 templateRadio.State == NSCellStateValue.On ||
				 nothingRadio.State ==  NSCellStateValue.On);
		}

		public event EventHandler BundleCreated;

		void BundleButton_Activated (object sender, EventArgs e)
		{
			BundleCreated?.Invoke (this, e);
		}

		void CancelButton_Activated (object sender, EventArgs e)
		{
			Close ();
		}

		void ItemsRefreshState_Handler (object sender, EventArgs e)
		{
			RefreshStates ();
		}

		async void FigmaUrlTextField_Changed (object sender, EventArgs e)
		{
			versionSpinner.Hidden = false;
			versionSpinner.StartAnimation (versionSpinner);

			//loads current versions
			versionPopUp.RemoveAllItems ();
			versionPopUp.Enabled = false;

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

			versionSpinner.StopAnimation (versionSpinner);
			versionSpinner.Hidden = true;

			if (versions != null) {
				var menu = new NSMenu();
				menu.AddItem(new NSMenuItem("Current"));

				if (versions.Length > 0) {
					menu.AddItem(NSMenuItem.SeparatorItem);

					foreach (FigmaFileVersion version in versions.Skip(1)) {
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

			RefreshStates ();
		}
	}
}
