/* 
 * CustomTextFieldConverter.cs
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

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views;
using FigmaSharp.NativeControls;

namespace FigmaSharp.NativeControls.Cocoa
{
    public class EmbededSheetDialogConverter : FigmaViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			var isWindow = currentNode.IsDialogParentContainer (NativeControlType.WindowSheet)
				|| currentNode.IsDialogParentContainer (NativeControlType.WindowPanel);
			return isWindow;
		}

		public static NSColor GetBackgroundColor (bool isWhite)
		{
			return isWhite ?
			NSColor.FromRgba (0.93f, 0.93f, 0.93f, 0.66f) :
			NSColor.FromRgba (0.29f, 0.29f, 0.29f, 0.66f);
		}

		public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var frame = (FigmaFrameEntity)currentNode;
			var view = new View(new FakeWindowView("Window"));

			var nativeView = view.NativeObject as FakeWindowView;
			nativeView.Configure(frame);

			nativeView.LiveButton.Activated += (s, e) => {
				if (nativeView.Window == null)
					return;

				var window = new Window(view.Allocation);

				window.KeyDown += (send, eve) => {
					NSApplication.SharedApplication.EndSheet((NSWindow)window.NativeObject);
					window.Close();
				};

				var fileProvider = new FigmaRemoteFileProvider();
				fileProvider.Load(rendererService.FileProvider.File);
				var secondaryRender = new NativeViewRenderingService(fileProvider);
				////we want to include some special converters to handle windows like normal view containers
				//rendererService.CustomConverters.Add(new EmbededSheetDialogConverter());
				//rendererService.CustomConverters.Add(new EmbededWindowConverter());
				secondaryRender.RenderInWindow(window, currentNode);
				NSApplication.SharedApplication.BeginSheet((NSWindow)window.NativeObject, nativeView.Window);
			};

			return view;
		}

		public override string ConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			return string.Empty;
		}
	}




	public class EmbededWindowConverter : FigmaViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			if (currentNode.IsWindowContent ()) {
				return currentNode.Parent != null && currentNode.Parent.IsDialogParentContainer (NativeControlType.WindowStandard);
			}

			var isWindow = currentNode.IsDialogParentContainer (NativeControlType.WindowStandard);
			return isWindow;
		}

		public override IView ConvertTo (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var frame = (FigmaFrameEntity)currentNode;

			var nativeView = new FakeWindowView("Window");
			var view = new View(nativeView);

			nativeView.Layer.CornerRadius = 5;
			nativeView.Configure (currentNode);

			nativeView.LiveButton.Activated += (s, e) => {
				var window = new Window(view.Allocation);
				var fileProvider = new FigmaRemoteFileProvider();
				fileProvider.Load(rendererService.FileProvider.File);
				var secondaryRender = new NativeViewRenderingService(fileProvider);
				secondaryRender.RenderInWindow(window, currentNode);
				(window.NativeObject as NSWindow).Appearance = (s as NSView).EffectiveAppearance;
				window.Show ();
				window.Center();
			};

			return view;
		}

		public override string ConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			return string.Empty;
		}
	}
}
