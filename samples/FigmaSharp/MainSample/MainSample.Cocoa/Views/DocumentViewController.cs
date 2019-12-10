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
using System.Linq;
using AppKit;
using FigmaSharp.Services;
using Foundation;
using FigmaSharp.Models;
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

		public DocumentViewController(IntPtr handle) : base(handle)
		{

		}

		public void Reload(string versionId = "", string pageId = "") => Load(versionId, pageId);

		public void LoadDocument(string token, string linkId, string versionId = "", string pageId = "")
		{
			Token = token;
			Link_ID = linkId;
			Load(version_id: versionId, page_id: pageId);
		}

		public void OnInitialize()
		{
			if (scrollview == null)
			{
				///View.AddSubview(progressIndicator);

				scrollview = new ScrollView();
				nativeScrollView = (FNSScrollview)scrollview.NativeObject;
				View.AddSubview(nativeScrollView);
				nativeScrollView.Frame = View.Bounds;

				windowController = (DocumentWindowController)this.View.Window.WindowController;
				windowController.VersionSelected += WindowController_VersionSelected;
				windowController.RefreshRequested += WindowController_RefreshRequested;
			}
		}

		private void WindowController_VersionSelected(object sender, string versionId) => Reload(versionId);
		private void WindowController_RefreshRequested(object sender, EventArgs e) => Reload();

		//NSProgressIndicator progressIndicator;

		void Load(string version_id, string page_id)
		{
			if (string.IsNullOrEmpty(Link_ID))
			{
				return;
			}
			windowController.Title = string.Format("Opening “{0}”…", Link_ID);

			ToggleSpinnerState(toggle_on: true);
			windowController.EnableButtons(false);

			scrollview.ClearSubviews();

			new Thread(() =>
			{

				this.InvokeOnMainThread(() =>
				{

					AppContext.Current.SetAccessToken(Token);

					var converters = AppContext.Current.GetFigmaConverters();

					Console.WriteLine("TOKEN: " + Token);

					var fileProvider = new FigmaRemoteFileProvider();
					var rendererService = new FigmaFileRendererService(fileProvider, converters);

					rendererService.Start(Link_ID, scrollview);

					var distributionService = new FigmaViewRendererDistributionService(rendererService);
					distributionService.Start();

					fileProvider.ImageLinksProcessed += (s, e) =>
					{
						// done
					};

					//We want know the background color of the figma camvas and apply to our scrollview
					var canvas = fileProvider.Nodes.OfType<FigmaCanvas>().FirstOrDefault();
					if (canvas != null)
						scrollview.BackgroundColor = canvas.backgroundColor;

					////NOTE: some toolkits requires set the real size of the content of the scrollview before position layers
					scrollview.AdjustToContent();

					windowController.Title = Link_ID;

					windowController.UpdateVersionMenu();
					windowController.UpdatePagesPopupButton();
					windowController.EnableButtons(true);

					ToggleSpinnerState(toggle_on: false);

				});

			}).Start();
		}

		#region Spinner

		public void ToggleSpinnerState(bool toggle_on)
		{
			if (toggle_on)
			{
				Spinner.Hidden = false;
				Spinner.StartAnimation(this);

			}
			else
			{
				Spinner.Hidden = true;
				Spinner.StopAnimation(this);
			}
		}

		#endregion

		public override NSObject RepresentedObject
		{
			get
			{
				return base.RepresentedObject;
			}

			set
			{
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
	}
}
