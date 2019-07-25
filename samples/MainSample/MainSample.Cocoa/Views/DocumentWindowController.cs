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
using System.Threading;

using AppKit;
using CoreGraphics;
using Foundation;

using FigmaSharp;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;

namespace FigmaSharp.Samples
{
    public partial class DocumentWindowController : NSWindowController
    {
        public static int WindowCount { get; private set; }
        const int NEW_WINDOW_OFFSET = 38;

        public string Title
        {
            get { return TitleTextField.StringValue; }
            set { TitleTextField.StringValue = value; }
        }


        public string Token = "";

        public string Link_ID = "";
        public string Version_ID = null;
        public string Page_ID = null;


        // FileProvider etc.


        public DocumentWindowController(IntPtr handle) : base(handle)
        {
        }


        public override void WindowDidLoad()
        {
            PositionWindow();
            base.WindowDidLoad();
        }


        public void LoadDocument(string token, string link_id)
        {
            Token = token;
            Link_ID = link_id;

            Load(version_id: "", page_id: "");
        }


        NSScrollView my_scroll_view;
        FigmaFileProvider fileProvider;

        void Load(string version_id, string page_id)
        {
            Title = string.Format("Opening “{0}”…", Link_ID);

            (Window.ContentViewController as DocumentViewController).ToggleSpinnerState(toggle_on: true);
            RefreshButton.Enabled = false;

            new Thread(() => {
                this.InvokeOnMainThread(() =>
                {

                    AppContext.Current.SetAccessToken(Token);

                    var converters = AppContext.Current.GetFigmaConverters();

                    Console.WriteLine("TOKEN: " + Token);
                    my_scroll_view = new NSScrollView();


                    ScrollViewWrapper wrapper = new ScrollViewWrapper(my_scroll_view);

                    fileProvider = new FigmaRemoteFileProvider();
                    var rendererService = new FigmaFileRendererService(fileProvider, converters);

                    rendererService.Start(Link_ID, wrapper);

                    var distributionService = new FigmaViewRendererDistributionService(rendererService);
                    distributionService.Start();

                    fileProvider.ImageLinksProcessed += (s, e) =>
                    {
                        Console.WriteLine("LOADING DONE");
                    };

                    var canvas = fileProvider.Nodes.OfType<FigmaCanvas>().FirstOrDefault();
                    //fileProvider.Nodes.OfType<FigmaCanvas>().Where(name => "User Testing");


                    if (canvas != null)
                        wrapper.BackgroundColor = canvas.backgroundColor;

                    ////NOTE: some toolkits requires set the real size of the content of the scrollview before position layers
                    wrapper.AdjustToContent();
                    // TODO: scroll to middle

                    string document_name = fileProvider.Response.name;
                    Title = document_name;
                    Window.Title = document_name;


                    var scroll = (NSScrollView)wrapper.NativeObject;
                    scroll.ScrollerStyle = NSScrollerStyle.Overlay;
                    scroll.AutohidesScrollers = true;

                    Window.ContentView.AddSubview(scroll);
                    scroll.Frame = Window.ContentView.Bounds;

                    UpdateVersionMenu();
                    UpdatePagesPopupButton();

                    RefreshButton.Enabled = true;
                    PagePopUpButton.Enabled = true;

                    (Window.ContentViewController as DocumentViewController).ToggleSpinnerState(toggle_on: false);
                });
   

            }).Start();
        }


        public void Reload()
        {
            Load(null, null);
        }


        void UpdatePagesPopupButton()
        {
            foreach (FigmaCanvas canvas in fileProvider.Nodes.OfType<FigmaCanvas>())
                PagePopUpButton.AddItem(canvas.name);

            PagePopUpButton.Activated += delegate {
                Console.WriteLine(PagePopUpButton.SelectedItem.Title);
            };
        }


        void UpdateVersionMenu()
        {
            var menu = new VersionMenu();

            menu.VersionSelected += delegate (string version_id) {
                Load(version_id, null);
            };

            /*
            menu.AddItem("1", "FigmaSharp.Cocoa 0.0.1", DateTime.Now);
            menu.AddItem("2", "FigmaSharp.Cocoa 0.0.2", DateTime.Now);
            menu.AddItem("3", "FigmaSharp.Cocoa 0.0.3", DateTime.Now);
            menu.AddItem("4", DateTime.Now);
            menu.AddItem("5", DateTime.Now.AddDays(-7));
            menu.AddItem("6", DateTime.Now.AddDays(-14));
            */

            menu.UseAsVersionsMenu();
        }


        partial void RefreshClicked(NSObject sender)
        {
            ToggleSpinnerState(toggle_on: true);
            RefreshButton.Enabled = false;

            new Thread(() => {
                                

                this.InvokeOnMainThread(() => {
                Load(Version_ID, null);

                    RefreshButton.Enabled = true;
                    ToggleSpinnerState(toggle_on: false);
                });

            }).Start();
        }


        void ToggleSpinnerState(bool toggle_on)
        {
            if (MainToolbar.VisibleItems[1].Identifier == "Spinner") {
                (MainToolbar.VisibleItems[1].View as NSProgressIndicator).StopAnimation(this);
                MainToolbar.RemoveItem(1);
            }

            if (toggle_on) {
                MainToolbar.InsertItem("Spinner", 1);
                (MainToolbar.VisibleItems[1].View as NSProgressIndicator).StartAnimation(this);
            }
        }


        void PositionWindow()
        {
            WindowCount++;
            CGRect frame = Window.Frame;

            frame.X += NEW_WINDOW_OFFSET * WindowCount;
            frame.Y -= NEW_WINDOW_OFFSET * WindowCount;

            Window.SetFrame(frame, display: true);
        }


        void ShowError()
        {
            var alert = new NSAlert() {
                AlertStyle = NSAlertStyle.Warning,
                MessageText = string.Format("Could not open “{0}”", Link_ID),
                InformativeText = "Please check if the provided Figma Link and Personal Access Token are correct.",
            };

            alert.AddButton("Close");
            alert.RunSheetModal(Window);

            WindowCount--;
            Window.PerformClose(this);
        }
    }
}
