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

using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Native.Cocoa;

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

		public void Reload (string versionId = "", string pageId = "", int startPage = 0) => Load (versionId, pageId, startPage);

		public void LoadDocument (string token, string linkId, string versionId = "", string pageId = "")
		{
			Token = token;
			Link_ID = linkId;
			ToggleSpinnerState(toggle_on: true);
			Load(version_id: versionId, page_id: pageId, startPage:0);
		}

		public void OnInitialize ()
		{
			if (scrollview == null) {

				scrollview = new ScrollView ();
				nativeScrollView = (FNSScrollview)scrollview.NativeObject;
				View.AddSubview (nativeScrollView);
				nativeScrollView.Frame = View.Bounds;
				nativeScrollView.ScrollerStyle = NSScrollerStyle.Overlay;

				nativeScrollView.ScrollerInsets = new NSEdgeInsets() { Top = 2, Bottom = 2, Left = 2, Right = 2 };
				nativeScrollView.AllowsMagnification = true;
				nativeScrollView.UsesPredominantAxisScrolling = false;

				windowController = (DocumentWindowController)this.View.Window.WindowController;
				windowController.VersionSelected += WindowController_VersionSelected;
				windowController.RefreshRequested += WindowController_RefreshRequested;
				windowController.PageChanged += WindowController_PageChanged;
			}
		}

		private void WindowController_PageChanged (object sender, int startPage) => Reload (startPage: startPage);
		private void WindowController_VersionSelected (object sender, string versionId) => Reload (versionId);
		private void WindowController_RefreshRequested (object sender, EventArgs e) => Reload ();

		//NSProgressIndicator progressIndicator;

		void Load (string version_id = "", string page_id = "", int startPage = 0)
		{
			if (string.IsNullOrEmpty (Link_ID))
				return;

			windowController.Title = string.Format ("Opening “{0}”…", Link_ID);

			ToggleSpinnerState (toggle_on: true);
			windowController.EnableButtons (false);

			scrollview.ClearSubviews ();

			new Thread (() => {

				this.InvokeOnMainThread (() => {

					AppContext.Current.SetAccessToken (Token);

					var converters = NativeControlsContext.Current
						.GetConverters ()
						.ToList ();
					converters.Add (new NativeControls.Cocoa.EmbeddedSheetDialogConverter ());
					converters.Add (new NativeControls.Cocoa.EmbededWindowConverter ());
					
					Console.WriteLine ("TOKEN: " + Token);

					var options = new FigmaViewRendererServiceOptions () { StartPage = startPage };
					var fileProvider = new FigmaRemoteFileProvider () { File = Link_ID };
					var rendererService = new NativeViewRenderingService (fileProvider, converters.ToArray ());
					rendererService.Start (Link_ID, scrollview, options);

					var distributionService = new FigmaViewRendererDistributionService (rendererService);
					distributionService.Start ();

					fileProvider.ImageLinksProcessed += (s, e) => {
						// done
					};

					//We want know the background color of the figma camvas and apply to our scrollview
					var canvas = fileProvider.Nodes.OfType<FigmaCanvas> ().FirstOrDefault ();
					if (canvas != null)
						scrollview.BackgroundColor = canvas.backgroundColor;

					////NOTE: some toolkits requires set the real size of the content of the scrollview before position layers
					scrollview.AdjustToContent ();


					NSScrollView scrollView = (scrollview.NativeObject as NSScrollView);
					NSView windowView = windowController.Window.ContentView;
					NSView documentView = (scrollView.DocumentView as NSView);


					// Center the document
					var posX = 0.5 * (windowView.Frame.Width  - documentView.Frame.Width);
					var posY = 0.5 * (windowView.Frame.Height - documentView.Frame.Height);

					NSView wrapper = new NSView(new CGRect(0, 0, windowView.Frame.Width, windowView.Frame.Height));

					wrapper.AutoresizingMask = documentView.AutoresizingMask = NSViewResizingMask.MaxXMargin |
						NSViewResizingMask.MinXMargin | NSViewResizingMask.MinYMargin;

					documentView.SetFrameOrigin(new CGPoint(posX, posY));


					// Add padding if the document is larger than the window
					const int padding = 64;

					if (documentView.Frame.Width + padding > windowView.Frame.Width) {
						wrapper.SetFrameSize(new CGSize(documentView.Frame.Width + (padding * 2), wrapper.Frame.Height));
						documentView.SetFrameOrigin (new CGPoint(padding, padding));
					}

					if (documentView.Frame.Height + padding > windowView.Frame.Height) {
						wrapper.SetFrameSize(new CGSize(wrapper.Frame.Width, documentView.Frame.Height + (padding * 2)));
						documentView.SetFrameOrigin(new CGPoint(padding, padding));
					}


					wrapper.AddSubview(documentView);
					scrollView.DocumentView = wrapper;


					// Scroll to top left
					scrollView.ContentView.ScrollPoint(new CGPoint(0, documentView.Frame.Size.Height));
					scrollView.ReflectScrolledClipView(scrollView.ContentView);


					windowController.Title = fileProvider.Response.name;

					windowController.UpdateVersionMenu ();
					windowController.UpdatePagesPopupButton (fileProvider);
					windowController.EnableButtons (true);

					ToggleSpinnerState (toggle_on: false);

				});

			}).Start ();
		}

		#region Spinner

		public void ToggleSpinnerState (bool toggle_on)
		{
			if (toggle_on) {
				View.AddSubview(Spinner);
				Spinner.Hidden = false;
				Spinner.StartAnimation (this);

			} else {
				Spinner.RemoveFromSuperview();
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
