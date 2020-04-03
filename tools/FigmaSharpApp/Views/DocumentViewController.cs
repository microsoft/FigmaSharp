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

using AppKit;
using Foundation;

using FigmaSharp;
using FigmaSharp.Models;
using FigmaSharp.NativeControls.Cocoa;
using FigmaSharp.Services;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharpApp
{
	public partial class DocumentViewController : NSViewController
	{
		public string Token = "";

		public string DocumentID;
		public string VersionID;
		public int CurrentPageIndex = 0;

		DocumentWindowController windowController;

		ScrollView MainScrollView;
		NSScrollView NativeScrollView;


		public DocumentViewController(IntPtr handle) : base(handle)
		{
		}


		public void OnInitialize()
		{
			MainScrollView = CreateScrollView();
			NativeScrollView = (NSScrollView)MainScrollView.NativeObject;

			View.AddSubview(NativeScrollView);


			var windowDelegate = new WindowDelegate();

			windowDelegate.GotFocus += async (s, e) => {
				await windowController.UpdateVersionMenu (DocumentID);
			};
			windowDelegate.LostFocus += (s, e) => {
				windowController.ClearVersionMenu ();
			};

			View.Window.WeakDelegate = windowDelegate;


			windowController = (DocumentWindowController)View.Window.WindowController;
			windowController.VersionSelected += WindowController_VersionSelected;
			windowController.RefreshRequested += WindowController_RefreshRequested;
			windowController.PageChanged += WindowController_PageChanged;
		}


		ScrollView CreateScrollView()
		{
			var scrollView = new ScrollView();
			var nativeScrollView = (NSScrollView)scrollView.NativeObject;

			nativeScrollView.AllowsMagnification = true;
			nativeScrollView.MaxMagnification = 16f;
			nativeScrollView.MinMagnification = 0.25f;

			nativeScrollView.Frame = View.Bounds;
			nativeScrollView.TranslatesAutoresizingMaskIntoConstraints = true;

			return scrollView;
		}


		public void LoadDocument(string token, string documentId, FigmaFileVersion version = null)
		{
			Token = token;
			DocumentID = documentId;

			ToggleSpinner(toggle_on: true);
			Load(version: version, pageIndex: CurrentPageIndex);
		}


		public void Reload(FigmaFileVersion version = null, int pageIndex = 0)
		{
			Load(version: version, pageIndex: pageIndex);
		}


		void WindowController_PageChanged(object sender, int pageIndex)
		{
			if (pageIndex == CurrentPageIndex)
				return;

			windowController.ToggleToolbarSpinner(toggle_on: true);

			CurrentPageIndex = pageIndex;
			Reload(pageIndex: pageIndex);
		}

		void WindowController_VersionSelected(object sender, FigmaFileVersion version)
		{
			windowController.ToggleToolbarSpinner(toggle_on: true);

			CurrentPageIndex = 0;
			Reload(version: version);
		}

		void WindowController_RefreshRequested(object sender, EventArgs e)
		{
			windowController.ToggleToolbarSpinner(toggle_on: true);
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

			FigmaSharp.AppContext.Current.SetAccessToken(Token);

			if (response == null || version != null) {
				fileProvider = new FigmaRemoteFileProvider() { File = DocumentID, Version = version };
				fileProvider.ImageLinksProcessed += (sender, e) => {
					InvokeOnMainThread(() => {
						windowController.ToggleToolbarSpinner(toggle_on: false);
					});
				};

				var embeddedSheetConverter = new EmbededSheetDialogConverter(fileProvider);
				var embeddedWindowConverter = new EmbededWindowConverter(fileProvider);

				rendererService = new NativeViewRenderingService(fileProvider);
				rendererService.CustomConverters.Add(embeddedSheetConverter);
				rendererService.CustomConverters.Add(embeddedWindowConverter);

				embeddedWindowConverter.LivePreviewLoading += delegate {
					InvokeOnMainThread(() => windowController.ToggleToolbarSpinner(true));
				};

				embeddedWindowConverter.LivePreviewLoaded += delegate {
					InvokeOnMainThread(() => windowController.ToggleToolbarSpinner(false));
				};
			}

			var scrollView = CreateScrollView();
			await rendererService.StartAsync (DocumentID, scrollView.ContentView, new FigmaViewRendererServiceOptions() { StartPage = pageIndex });

			windowController.ToggleToolbarSpinner(toggle_on: true);

			new StoryboardLayoutManager().Run(scrollView.ContentView, rendererService);
			response = fileProvider.Response;

			NativeScrollView.RemoveFromSuperview();
			MainScrollView = scrollView;

			NativeScrollView = (NSScrollView)scrollView.NativeObject;
			View.AddSubview(NativeScrollView);

			windowController.Window.Title = windowController.Title = response.name;
			windowController.Window.BackgroundColor = NativeScrollView.BackgroundColor;

			windowController.UpdatePagesPopupButton(response.document);
			await windowController.UpdateVersionMenu(DocumentID);
			windowController.EnableButtons(true);

			ToggleSpinner(toggle_on: false);
		}

		public void ZoomIn()  { NativeScrollView.Magnification *= 2; }
		public void ZoomOut() { NativeScrollView.Magnification *= 0.5f; }
		public void Zoom100() { NativeScrollView.Magnification = 1; }



		public void ToggleSpinner (bool toggle_on)
		{
			View.AddSubview(Spinner);
			Spinner.UsesThreadedAnimation = true;

			if (toggle_on) {
				Spinner.StartAnimation (this);
			} else {
				Spinner.StopAnimation (this);
			}
		}


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
