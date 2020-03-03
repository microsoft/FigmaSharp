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
			var view = EmbededWindowConverter.GetSimulatedWindow();

			var nativeView = view.NativeObject as NSView;
			nativeView.Configure(frame);

			var button = new NSButton() { TranslatesAutoresizingMaskIntoConstraints = false, Title = string.Empty };
			button.BezelStyle = NSBezelStyle.HelpButton;
			button.ToolTip = "Press to simulate a Sheet";
			button.Activated += (s, e) => {

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

			nativeView.AddSubview(button);
			button.TopAnchor.ConstraintEqualToAnchor(nativeView.TopAnchor, 3).Active = true;
			button.RightAnchor.ConstraintEqualToAnchor(nativeView.RightAnchor, -3).Active = true;

			var firstElement = currentNode.GetDialogInstanceFromParentContainer () as FigmaInstance;

			if (firstElement != null) {
				firstElement.TryGetNativeControlComponentType (out var controlType);
				if (controlType.ToString ().EndsWith ("Dark", StringComparison.Ordinal)) {
					nativeView.Appearance = NSAppearance.GetAppearance (NSAppearance.NameDarkAqua);

					if (frame.HasFills && frame.fills[0].color != null) {
						nativeView.Layer.BackgroundColor = frame.fills[0].color.ToCGColor ();
					} else {
						nativeView.Layer.BackgroundColor = GetBackgroundColor (false).CGColor;
					}

				} else {
					nativeView.Layer.BackgroundColor = GetBackgroundColor (true).CGColor;
					nativeView.Appearance = NSAppearance.GetAppearance (NSAppearance.NameAqua);
					
				}
			}
			return view;
		}

		public override string ConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			return string.Empty;
		}
	}

	public class EmbededWindowConverter : FigmaViewConverter
	{
		public static NSColor GetWindowBackgroundColor (bool isWhite)
		{
			return isWhite ?
			NSColor.FromRgba (red: 0.961f, green: 0.961f, blue: 0.961f, alpha: 1) :
			NSColor.FromRgba (0.29f, 0.29f, 0.29f, 0.66f);
		}

		public static IView GetSimulatedWindow ()
		{
			var view = new View ();
			var nativeView = view.NativeObject as NSView;
		
			nativeView.Layer.BorderWidth = 1;
			nativeView.Layer.BorderColor = NSColor.Gray.CGColor;
			nativeView.Layer.ShadowOpacity = 1.0f;
			nativeView.Layer.ShadowRadius = 20;
			nativeView.Layer.ShadowOffset = new CoreGraphics.CGSize (0, -10);
			return view;
		}

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
			var view = GetSimulatedWindow ();
			var nativeView = view.NativeObject as NSView;

			nativeView.Layer.CornerRadius = 5;
			nativeView.Configure (currentNode);

			var button = new NSButton() { TranslatesAutoresizingMaskIntoConstraints = false, Title = "" };
			button.BezelStyle = NSBezelStyle.HelpButton;
			button.ToolTip = "Press to simulate a Window";
			button.Activated += (s, e) => {
				var window = new Window(view.Allocation);
				var fileProvider = new FigmaRemoteFileProvider();
				fileProvider.Load(rendererService.FileProvider.File);
				var secondaryRender = new NativeViewRenderingService(fileProvider);
				secondaryRender.RenderInWindow(window, currentNode);
				window.Show ();
				window.Center();
			};

			nativeView.AddSubview(button);
			button.TopAnchor.ConstraintEqualToAnchor(nativeView.TopAnchor, 3).Active = true;
			button.RightAnchor.ConstraintEqualToAnchor(nativeView.RightAnchor, -3).Active = true;

			var firstElement = currentNode.GetDialogInstanceFromParentContainer () as FigmaInstance;
			if (firstElement != null) {
			
				firstElement.TryGetNativeControlComponentType (out var controlType);
				if (controlType.ToString ().EndsWith ("Dark", StringComparison.Ordinal)) {
					nativeView.Appearance = NSAppearance.GetAppearance (NSAppearance.NameDarkAqua);
					if (frame.HasFills && frame.fills[0].color != null) {
						nativeView.Layer.BackgroundColor = frame.fills[0].color.ToCGColor ();
					} else {
						nativeView.Layer.BackgroundColor = GetWindowBackgroundColor (false).CGColor;
					}

				} else {
					nativeView.Layer.BackgroundColor = GetWindowBackgroundColor (true).CGColor;
					nativeView.Appearance = NSAppearance.GetAppearance (NSAppearance.NameAqua);
				}

			}

			return view;
		}

		public override string ConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			return string.Empty;
		}
	}
}
