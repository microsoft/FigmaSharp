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
using System.Threading.Tasks;

using AppKit;
using CoreGraphics;
using Foundation;

using FigmaSharp;
using FigmaSharp.Models;
using FigmaSharp.Cocoa;

namespace FigmaSharpApp
{
	public partial class DocumentWindowController : NSWindowController
	{
		public string Title {
			get { return TitleTextField.StringValue; }
			set { TitleTextField.StringValue = value; }
		}

		public event EventHandler<FigmaFileVersion> VersionSelected;
		public event EventHandler<int> PageChanged;

		public event EventHandler RefreshRequested;

		public DocumentWindowController (IntPtr handle) : base (handle)
		{
			UsesDarkMode = NSApplication.SharedApplication.EffectiveAppearance.Name == NSAppearance.NameDarkAqua;
		}

		static bool firstWindow = true;

		NSMenuItem versionsMainMenu;
		FigmaVersionMenu menuManager;

		public override void WindowDidLoad ()
		{
			base.WindowDidLoad ();

			menuManager = new FigmaVersionMenu ();
			menuManager.VersionSelected += (s, e) => {
				VersionSelected?.Invoke (this, e);
			};
			versionsMainMenu = NSApplication.SharedApplication.MainMenu.ItemWithTitle("Versions");

			CGRect frame = Window.Frame;
			ShouldCascadeWindows = true;

			frame.Width  = NSScreen.MainScreen.Frame.Width  * 0.8f;
			frame.Height = NSScreen.MainScreen.Frame.Height * 0.8f;

			Window.SetFrame(frame, display: true);

			if (firstWindow)
				Window.Center();

			if (UsesDarkMode)
				DarkModeButton.State = NSCellStateValue.On;

			firstWindow = false;
		}

		public bool UsesDarkMode = false;

		public void ToggleDarkMode()
		{
			if (UsesDarkMode) {
				Window.Appearance = NSAppearance.GetAppearance(NSAppearance.NameAqua);
				DarkModeButton.State = NSCellStateValue.Off;
				UsesDarkMode = false;

			} else {
				Window.Appearance = NSAppearance.GetAppearance(NSAppearance.NameDarkAqua);
				DarkModeButton.State = NSCellStateValue.On;
				UsesDarkMode = true;
			}
		}

		partial void DarkModeClicked(NSObject sender) => ToggleDarkMode();

		public void UpdatePagesPopupButton (FigmaDocument document)
		{
			PagePopUpButton.RemoveAllItems();

			foreach (var item in document.children.ToArray()) {
				PagePopUpButton.AddItem (item.name);

				PagePopUpButton.Activated += delegate {
					PageChanged?.Invoke (this, (int) PagePopUpButton.IndexOfSelectedItem );
				};
			}

			PagePopUpButton.SelectItem((ContentViewController as DocumentViewController).PageIndex);
		}

		public void EnableButtons (bool enable)
		{
			RefreshButton.Enabled = enable;
			PagePopUpButton.Enabled = enable;
		}

		bool UseAsVersionsMenu ()
		{
			if (versionsMainMenu == null)
				return false;
			menuManager.Generate(versionsMainMenu.Submenu);
			return true;
		}

		internal void ClearVersionMenu()
		{
			menuManager.Clear (versionsMainMenu.Submenu);
		}

		public async Task UpdateVersionMenu (string link_id)
		{
			if (!UseAsVersionsMenu()) {
				return;
			}

			versionsMainMenu.Activated += (s,e) => {
				FigmaFileVersion version = menuManager.GetFileVersion(versionsMainMenu);
				if (version != null)
					VersionSelected?.Invoke (this, version);
			};

			var versions = await Task.Run(() =>
			{
				var query = new FigmaFileVersionQuery(link_id);

				FigmaSharp.AppContext.Current.SetAccessToken(TokenStore.SharedTokenStore.GetToken());

				return FigmaSharp.AppContext.Api.GetFileVersions(query)
					.versions.GroupBy(x => x.created_at)
					.Select(group => group.First())
					.ToArray ();
			});

			menuManager.Clear();

			foreach (var version in versions)
				menuManager.AddItem(version);

			UseAsVersionsMenu();
		}

		partial void RefreshClicked (NSObject sender)
		{
			RefreshRequested?.Invoke (this, EventArgs.Empty);
		}

		public void ToggleSpinnerState (bool toggle_on)
		{
			if (toggle_on)
				ToolbarSpinner.StartAnimation(this);
			else
				ToolbarSpinner.StopAnimation(this);
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

		public override void KeyDown(NSEvent theEvent)
		{
			if (theEvent.ModifierFlags.HasFlag(NSEventModifierMask.CommandKeyMask) &&
				theEvent.CharactersIgnoringModifiers == "w")
			{
				Window.PerformClose(this);
			}
		}
	}
}
