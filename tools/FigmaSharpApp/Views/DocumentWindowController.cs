/* 
 * Author:
 *   Hylke Bons <hylbo@microsoft.com>
 *
 * Copyright (C) 2019 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */


using System;
using System.Linq;

using AppKit;
using CoreGraphics;
using Foundation;

using FigmaSharp.Models;
using FigmaSharp.Services;

namespace FigmaSharp.Samples
{
	public partial class DocumentWindowController : NSWindowController
	{
		public string Title {
			get { return TitleTextField.StringValue; }
			set { TitleTextField.StringValue = value; }
		}

		public event EventHandler<string> VersionSelected;
		public event EventHandler<int> PageChanged;

		public event EventHandler RefreshRequested;

		public DocumentWindowController (IntPtr handle) : base (handle)
		{
		}


		public override void WindowDidLoad ()
		{
			base.WindowDidLoad ();

			CGRect frame = Window.Frame;
			ShouldCascadeWindows = true;

			frame.Width  = NSScreen.MainScreen.Frame.Width  * 0.8f;
			frame.Height = NSScreen.MainScreen.Frame.Height * 0.8f;

			Window.SetFrame(frame, display: true);

			if (NSApplication.SharedApplication.Windows.Length == 3) // Magic number :(
				Window.Center();
		}


		FigmaCanvas[] elements;

		public void UpdatePagesPopupButton (IFigmaFileProvider fileProvider)
		{
			elements = fileProvider.Nodes
				.OfType<FigmaCanvas> ()
				.ToArray ();

			foreach (var item in elements) {
				PagePopUpButton.AddItem (item.name);
				PagePopUpButton.Activated += (s, e) => {
					var button = (NSPopUpButton)s;
					PageChanged?.Invoke (this, (int) button.IndexOfSelectedItem );
				};
			}
		}

		public void EnableButtons (bool enable)
		{
			RefreshButton.Enabled = enable;
			PagePopUpButton.Enabled = enable;
		}


		VersionMenu VersionMenu;

		public async void UpdateVersionMenu (string link_id)
		{
			if (VersionMenu != null) {
				VersionMenu.UseAsVersionsMenu();
				return;
			}

			VersionMenu = new VersionMenu();

			VersionMenu.VersionSelected += delegate (string version_id) {
				VersionSelected?.Invoke (this, version_id);
			};

			FigmaFileVersion[] versions = null;

			try
			{
				var query = new FigmaFileVersionQuery(link_id);

				AppContext.Current.SetAccessToken(TokenStore.SharedTokenStore.GetToken());
				versions = AppContext.Api.GetFileVersions(query).versions;
				versions = versions.GroupBy(x => x.created_at)
					.Select(group => group.First())
					.ToArray();

			} catch (Exception ex) {
				Console.WriteLine(ex);
			}

			foreach (var version in versions)
			{
				if (version.label != null && !string.IsNullOrWhiteSpace(version.label))
					VersionMenu.AddItem(version.id, version.label, version.created_at);
				else
					VersionMenu.AddItem(version.id, version.created_at);
			}

			VersionMenu.UseAsVersionsMenu ();
		}


		partial void RefreshClicked (NSObject sender)
		{
			RefreshRequested?.Invoke (this, EventArgs.Empty);
		}

		public void ToggleSpinnerState (bool toggle_on)
		{
			if (MainToolbar.VisibleItems[1].Identifier == "Spinner") {
				(MainToolbar.VisibleItems[1].View as NSProgressIndicator).StopAnimation (this);
				MainToolbar.RemoveItem (1);
			}

			if (toggle_on) {
				MainToolbar.InsertItem ("Spinner", 1);
				(MainToolbar.VisibleItems[1].View as NSProgressIndicator).StartAnimation (this);
			}
		}


		public void ShowError (string linkId)
		{
			var alert = new NSAlert () {
				AlertStyle = NSAlertStyle.Warning,
				MessageText = string.Format ("Could not open “{0}”", linkId),
				InformativeText = "Please check if the provided Figma Link and Personal Access Token are correct.",
			};

			alert.AddButton ("Close");
			alert.RunSheetModal (Window);

			Window.PerformClose (this);
		}
	}
}
