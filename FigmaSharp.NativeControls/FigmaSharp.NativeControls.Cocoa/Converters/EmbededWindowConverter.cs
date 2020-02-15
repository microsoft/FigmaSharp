using System;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

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

		public override IView ConvertTo (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var frame = (FigmaFrameEntity)currentNode;
			var view = EmbededWindowConverter.GetSimulatedWindow ();
		
			var nativeView = view.NativeObject as NSView;
			nativeView.Configure (frame);

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
