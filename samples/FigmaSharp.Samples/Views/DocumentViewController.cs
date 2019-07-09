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
using System.Collections.Generic;
using System.Threading;

using FigmaSharp;
using FigmaSharp.Services;

using AppKit;
using Foundation;

namespace FigmaSharp.Samples
{
    public partial class DocumentViewController : NSViewController
    {
        readonly string ID;
        readonly string Token;

        public DocumentViewController(IntPtr handle) : base(handle)
        {
        }


        public DocumentViewController(string id, string token)
        {
            ID = id;
            Token = token;
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Spinner.StartAnimation(this);

            var menu = new VersionMenu();
            menu.AddItem("1", "FigmaSharp.Cocoa 0.0.1", DateTime.Now);
            menu.AddItem("2", "FigmaSharp.Cocoa 0.0.2", DateTime.Now);
            menu.AddItem("3", "FigmaSharp.Cocoa 0.0.3", DateTime.Now);
            menu.AddItem("4", DateTime.Now);
            menu.AddItem("5", DateTime.Now.AddDays(-7));
            menu.AddItem("6", DateTime.Now.AddDays(-14));
            menu.UseAsVersionsMenu();

            menu.VersionSelected += delegate (string version_id) {
                Load(version_id, null);
            };

            //Console.WriteLine("test: " + (View.Window.WindowControllerWindowController as DocumentWindowController).test);

            //  var parent = (OpenLocationViewController) pare;
            //  Console.WriteLine(parent.Token);

            // var parent = (OpenLocationViewController) parent.;
            // Console.WriteLine(parent.Token);

            Load(ID, Token);
        }


        public void Load(string version_id, string page_id)
        {
            ShowError();
           // Console.WriteLine((View.Window.WindowController as DocumentWindowController).test);


            //  Spinner.Hidden = false;
            //  Spinner.StartAnimation(this);
            /*
              var converters = AppContext.Current.GetFigmaConverters();

              file_provider = new FigmaRemoteFileProvider();

              var rendererService = new FigmaViewRendererService(file_provider, converters);
              rendererService.Start(Id, scrollViewWrapper);

              var distributionService = new FigmaViewRendererDistributionService(rendererService);
              distributionService.Start();

              //We want know the background color of the figma camvas and apply to our scrollview
              var canvas = file_provider.Nodes.OfType<FigmaCanvas>().FirstOrDefault();
              if (canvas != null)
                  scrollViewWrapper.BackgroundColor = canvas.backgroundColor;

              //NOTE: some toolkits requires set the real size of the content of the scrollview before position layers
              scrollViewWrapper.AdjustToContent();

      */

            // Spinner.Hidden = true;
            //  Spinner.StopAnimation(this);
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
alert.RunSheetModal(Window);
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
