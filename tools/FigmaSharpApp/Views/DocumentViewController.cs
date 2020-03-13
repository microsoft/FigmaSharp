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
using System.Threading.Tasks;

using AppKit;
using Foundation;

using FigmaSharp;
using FigmaSharp.Models;
using FigmaSharp.NativeControls.Cocoa;
using FigmaSharp.Services;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharpApp
{
	public partial class DocumentViewController : NSViewController
	{
		public string Token = "";

		public string DocumentID;
		public string VersionID;
		public int PageIndex = 0;

		ScrollView scrollview;
		NSScrollView nativeScrollView;

		DocumentWindowController windowController;

		public DocumentViewController(IntPtr handle) : base(handle)
		{
		}

		public void OnInitialize()
		{
			var windowDelegate = new WindowDelegate();

			windowDelegate.GotFocus += async (s, e) => {
				await windowController.UpdateVersionMenu (DocumentID);
			};
			windowDelegate.LostFocus += (s, e) => {
				windowController.ClearVersionMenu ();
			};

			View.Window.WeakDelegate = windowDelegate;

			if (scrollview == null)
			{
				scrollview = new ScrollView();

				nativeScrollView = (FNSScrollview)scrollview.NativeObject;
				nativeScrollView.AllowsMagnification = true;
				nativeScrollView.MaxMagnification = 16f;
				nativeScrollView.MinMagnification = 0.25f;

				View.AddSubview(nativeScrollView);

				nativeScrollView.Frame = View.Bounds;
				nativeScrollView.TranslatesAutoresizingMaskIntoConstraints = true;

				windowController = (DocumentWindowController)this.View.Window.WindowController;
				windowController.VersionSelected += WindowController_VersionSelected;
				windowController.RefreshRequested += WindowController_RefreshRequested;
				windowController.PageChanged += WindowController_PageChanged;
			}
		}

		public void LoadDocument(string token, string documentId, FigmaFileVersion version = null)
		{
			Token = token;
			DocumentID = documentId;

			ToggleSpinnerState(toggle_on: true);
			Load(version: version, pageIndex: PageIndex);
		}

		public void Reload(FigmaFileVersion version = null, int pageIndex = 0)
		{
			Load(version: version, pageIndex: pageIndex);
		}

		void WindowController_PageChanged(object sender, int pageIndex)
		{
			if (pageIndex == PageIndex)
				return;

			PageIndex = pageIndex;
			Reload(pageIndex: pageIndex);
		}

		void WindowController_VersionSelected(object sender, FigmaFileVersion version)
		{
			PageIndex = 0;
			Reload(version: version);
		}

		void WindowController_RefreshRequested(object sender, EventArgs e)
		{
			Reload();
		}

		FigmaFileResponse response;
		FigmaRemoteFileProvider fileProvider;
		NativeViewRenderingService rendererService;

		async void Load (FigmaFileVersion version = null, int pageIndex = 0)
		{
			if (string.IsNullOrEmpty (DocumentID))
				return;

			windowController.EnableButtons (false);

			if (response == null)
				windowController.Title = string.Format ("Opening “{0}”…", DocumentID);

			windowController.ToggleSpinnerState(toggle_on: true);

			FigmaSharp.AppContext.Current.SetAccessToken(Token);

			if (response == null || version != null) {
				fileProvider = new FigmaRemoteFileProvider() { File = DocumentID, Version = version };
				fileProvider.ImageLinksProcessed += (s, e) => {
					InvokeOnMainThread(() => {
						windowController.ToggleSpinnerState(toggle_on: false);
					});
				};

				rendererService = new NativeViewRenderingService(fileProvider);
				rendererService.CustomConverters.Add(new EmbededSheetDialogConverter(fileProvider));
				rendererService.CustomConverters.Add(new EmbededWindowConverter(fileProvider));
			}

			scrollview.ClearSubviews();

			await rendererService.StartAsync (DocumentID, scrollview.ContentView, new FigmaViewRendererServiceOptions() { StartPage = pageIndex });

			new StoryboardLayoutManager().Run(scrollview.ContentView, rendererService);
			response = fileProvider.Response;

			windowController.Window.Title = windowController.Title = response.name;
			windowController.Window.BackgroundColor = nativeScrollView.BackgroundColor;

			windowController.UpdatePagesPopupButton(response.document);
			windowController.UpdateVersionMenu(DocumentID);
			windowController.EnableButtons(true);

			ToggleSpinnerState(toggle_on: false);
		}

		public void ZoomIn()  { nativeScrollView.Magnification *= 2; }
		public void ZoomOut() { nativeScrollView.Magnification *= 0.5f; }
		public void Zoom100() { nativeScrollView.Magnification = 1; }

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

		class WindowDelegate : NSWindowDelegate
		{
			public event EventHandler GotFocus;
			public event EventHandler LostFocus;

			public override void DidBecomeKey(NSNotification notification)
			{
				GotFocus?.Invoke (this, EventArgs.Empty);
			}
			public override void DidResignKey(NSNotification notification)
			{
				LostFocus?.Invoke(this, EventArgs.Empty);
			}
		}

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
