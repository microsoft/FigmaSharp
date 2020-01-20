/* 
 * FigmaRuntime.cs 
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
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
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Dialogs;

namespace MonoDevelop.Figma
{
	static class FigmaRuntime
	{
		const string FigmaSetting = "FigmaToken";

		public static string Token {
			get => PropertyService.Get<string> (FigmaSetting);
            set
            {
                FigmaSharp.AppContext.Current.SetAccessToken(value);
                PropertyService.Set(FigmaSetting, value);
				TokenChanged?.Invoke (null, EventArgs.Empty);
			}
		}

		public static event EventHandler TokenChanged;
	}

	class FigmaOptionsWidget : VBox
	{
		Entry tokenValueEntry;
		FigmaOptionsPanel panel;
		public FigmaOptionsWidget (FigmaOptionsPanel figmaOptionsPanel)
		{
			panel = figmaOptionsPanel;

			var mainVBox = new HBox ();
			PackStart (mainVBox);

			var tokenLabel = new Label ();
			tokenLabel.Text = GettextCatalog.GetString ("Personal Access Token:");
			mainVBox.PackStart (tokenLabel, false, false, 6);
			tokenValueEntry = new Entry ();
			mainVBox.PackStart (tokenValueEntry, false, false, 10);

			tokenValueEntry.WidthRequest = 350;
            tokenValueEntry.Text = FigmaRuntime.Token;

            tokenValueEntry.Changed += NeedsStoreValue;
			tokenValueEntry.FocusOutEvent += NeedsStoreValue;

            var refreshLabel = new Label() { Text = GettextCatalog.GetString("Reloads all the converters in the assembly folder") };
            mainVBox.PackStart(refreshLabel, false, false, 10);
            reloadButton = new Button() { Label = "Go!" };
            mainVBox.PackStart(reloadButton, false, false, 10);
            reloadButton.Activated += RefresButton_Activated;

            ShowAll();
		}

        Button reloadButton;

        private void RefresButton_Activated(object sender, EventArgs e)
        {
           
        }

        void NeedsStoreValue (object sender, EventArgs e)
		{
			FigmaRuntime.Token = tokenValueEntry.Text;
		}

		internal void ApplyChanges ()
		{
			FigmaRuntime.Token = tokenValueEntry.Text;
		}

        public override void Dispose()
        {
            reloadButton.Activated -= RefresButton_Activated;
        }
    }

	class FigmaOptionsPanel : OptionsPanel
	{
		FigmaOptionsWidget widget;

		public override Control CreatePanelWidget ()
		{
			widget = new FigmaOptionsWidget (this);
			return widget;
		}

		public FilePath LoadSdkLocationSetting ()
		{
			return FigmaRuntime.Token;
		}

		public void SaveSdkLocationSetting (FilePath location)
		{
			if (location == FigmaRuntime.Token) {
				return;
			}
			FigmaRuntime.Token = location;
		}

		public override void ApplyChanges ()
		{
			widget.ApplyChanges ();
		}
	}
}
