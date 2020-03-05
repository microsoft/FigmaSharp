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
using Foundation;
using FigmaSharp.Services;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Native.Cocoa;
using FigmaSharp.NativeControls.Cocoa;

namespace FigmaSharp.Samples
{
	public partial class DocumentViewController : NSViewController
	{
		public string Token = "";

		public string Link_ID = "";
		public string Version_ID = null;
		public string Page_ID = null;

		ScrollView scrollview;
		FNSScrollview nativeScrollView;

		DocumentWindowController windowController;

		public DocumentViewController (IntPtr handle) : base (handle)
		{
		}

		class WindowDelegate : NSWindowDelegate
		{
			public event EventHandler WindowFocused = delegate { };

			public override void DidBecomeKey(NSNotification notification)
			{
				WindowFocused(this, new EventArgs());
			}
		}

		public void Reload (Models.FigmaFileVersion version = null, string pageId = "", int startPage = 0) => Load (version, pageId, startPage);

		public void LoadDocument (string token, string linkId, Models.FigmaFileVersion version = null, string pageId = "")
		{
			Token = token;
			Link_ID = linkId;
			ToggleSpinnerState(toggle_on: true);
			Load(version: version, page_id: pageId, startPage:0);
		}

		public void OnInitialize ()
		{
			var windowDelegate = new WindowDelegate();

			windowDelegate.WindowFocused += delegate {
				windowController.UpdateVersionMenu(Link_ID);
			};

			View.Window.WeakDelegate = windowDelegate;

			if (scrollview == null) {
				scrollview = new ScrollView ();
				nativeScrollView = (FNSScrollview)scrollview.NativeObject;
				View.AddSubview (nativeScrollView);

				nativeScrollView.Frame = View.Bounds;
				nativeScrollView.TranslatesAutoresizingMaskIntoConstraints = true;

				windowController = (DocumentWindowController)this.View.Window.WindowController;
				windowController.VersionSelected += WindowController_VersionSelected;
				windowController.RefreshRequested += WindowController_RefreshRequested;
				windowController.PageChanged += WindowController_PageChanged;
			}
		}

		private void WindowController_PageChanged (object sender, int startPage) => Reload (startPage: startPage);
		private void WindowController_VersionSelected (object sender, Models.FigmaFileVersion version) => Reload (version: version);
		private void WindowController_RefreshRequested (object sender, EventArgs e) => Reload ();

		//NSProgressIndicator progressIndicator;

		void Load (Models.FigmaFileVersion version = null, string page_id = "", int startPage = 0)
		{
			if (string.IsNullOrEmpty (Link_ID))
				return;

			windowController.Title = string.Format ("Opening “{0}”…", Link_ID);

			ToggleSpinnerState (toggle_on: true);
			windowController.EnableButtons (false);

			scrollview.ClearSubviews ();

			Task.Run(() =>
			{
				InvokeOnMainThread (() =>
				{
					AppContext.Current.SetAccessToken (Token);

					Console.WriteLine ("TOKEN: " + Token);

					var fileProvider = new FigmaRemoteFileProvider () { File = Link_ID, Version = version };
					var rendererService = new NativeViewRenderingService (fileProvider);

                    //we want to include some special converters to handle windows like normal view containers
					rendererService.CustomConverters.Add(new EmbededSheetDialogConverter());
					rendererService.CustomConverters.Add(new EmbededWindowConverter());

					rendererService.Start (Link_ID, scrollview.ContentView, new FigmaViewRendererServiceOptions() { StartPage = startPage });

					new StoryboardLayoutManager()
						.Run (scrollview.ContentView, rendererService);

					fileProvider.ImageLinksProcessed += (s, e) => {
						// done
					};

					var nativeScrollView = scrollview.NativeObject as NSScrollView;

					windowController.Window.Title = windowController.Title = fileProvider.Response?.name ?? "";

					NSColor backgroundColor = fileProvider.Response?.document.children[0].backgroundColor.ToNSColor();
					windowController.Window.BackgroundColor = backgroundColor;
					nativeScrollView.BackgroundColor = backgroundColor;

					windowController.UpdateVersionMenu (Link_ID);
					windowController.UpdatePagesPopupButton (fileProvider);
					windowController.EnableButtons (true);


					ToggleSpinnerState (toggle_on: false);

				});
			});
		}

		#region Spinner

		public void ToggleSpinnerState (bool toggle_on)
		{
			if (toggle_on) {
				View.AddSubview(Spinner);

				Spinner.Hidden = false;
				Spinner.StartAnimation (this);

			} else {
				Spinner.Hidden = true;
				Spinner.StopAnimation (this);
			}
		}

		#endregion

		public override NSObject RepresentedObject {
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
