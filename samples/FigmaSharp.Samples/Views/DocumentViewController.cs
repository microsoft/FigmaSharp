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
using System.Threading;

using FigmaSharp;
using FigmaSharp.Services;

using AppKit;
using Foundation;

namespace FigmaSharp.Samples
{
    public partial class DocumentViewController : NSViewController
    {
        public string Link_ID = "";
        public string Token = "";

        // FileProvider etc.


        public DocumentViewController(IntPtr handle) : base(handle)
        {
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }


        public void Load(string version_id, string page_id)
        {
            ToggleSpinnerState(toggle_on: true);

            // load figma document

            UpdateVersionMenu();
            UpdatePagesPopupButton();

            ToggleSpinnerState(toggle_on: false);
        }


        public void Reload()
        {
            Load(Link_ID, Token);
        }


        void ToggleSpinnerState (bool toggle_on)
        {
            if (toggle_on) {
                Spinner.Hidden = false;
                Spinner.StartAnimation(this);

            } else {
                Spinner.Hidden = true;
                Spinner.StopAnimation(this);
            }
        }


        void UpdateVersionMenu()
        {
            var menu = new VersionMenu();

            menu.VersionSelected += delegate (string version_id) {
                Load(version_id, null);
            };

            menu.AddItem("1", "FigmaSharp.Cocoa 0.0.1", DateTime.Now);
            menu.AddItem("2", "FigmaSharp.Cocoa 0.0.2", DateTime.Now);
            menu.AddItem("3", "FigmaSharp.Cocoa 0.0.3", DateTime.Now);
            menu.AddItem("4", DateTime.Now);
            menu.AddItem("5", DateTime.Now.AddDays(-7));
            menu.AddItem("6", DateTime.Now.AddDays(-14));

            menu.UseAsVersionsMenu();
        }


        void UpdatePagesPopupButton()
        {

        }


        void ShowError ()
        {
            /*
                var alert = new NSAlert()
                {
                    AlertStyle = NSAlertStyle.Warning,
                    MessageText = "Could not open “DhOTs1gwx837ysnG3X6RZqZm”",
                    InformativeText = "Please check if your Figma Link and Personal Access Token are correct",
                };

                alert.AddButton("Close");
                //alert.RunSheetModal(View.Window);
                            NSApplication.SharedApplication.BeginSheet();
            */
        }


        public override NSObject RepresentedObject
        {
            get {
                return base.RepresentedObject;
            }

            set {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
