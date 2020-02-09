using System;
using AppKit;
using FigmaSharp;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views.Cocoa;
using System.Linq;
using FigmaSharp.Views;
using FigmaSharp.NativeControls;

namespace FigmaSharp.Samples
{
	class SheetDialogConverter : FigmaViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			var isWindow = currentNode.IsDialogParentContainer (NativeControlType.WindowSheet)
				|| currentNode.IsDialogParentContainer (NativeControlType.WindowPanel);
			return isWindow;
		}

		static CoreGraphics.CGColor backgroundWindowColor =
			(true ?
			NSColor.FromRgba (0.93f, 0.93f, 0.93f, 0.66f) :
			NSColor.FromRgba (0.29f, 0.29f, 0.29f, 0.66f)).CGColor;

		public override IView ConvertTo (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var frame = (FigmaFrameEntity)currentNode;

			var view = WindowConverter.GetSimulatedWindow ();
		
			var nativeView = view.NativeObject as NSView;
			nativeView.Configure (frame);

			var firstElement = currentNode.GetDialogInstanceFromParentContainer ();
			firstElement.TryGetNativeControlComponentType (out var controlType);
			if (controlType.ToString ().EndsWith ("Dark", StringComparison.Ordinal)) {
				nativeView.Appearance = NSAppearance.GetAppearance (NSAppearance.NameDarkAqua);
			}
			return view;
		}

		public override string ConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			return string.Empty;
		}
	}

	class WindowConverter : FigmaViewConverter
	{
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
			var view = GetSimulatedWindow ();
			var nativeView = view.NativeObject as NSView;
			
			nativeView.Layer.CornerRadius = 5;
			nativeView.Configure (currentNode);

			var firstElement = currentNode.GetDialogInstanceFromParentContainer ();
			firstElement.TryGetNativeControlComponentType (out var controlType);
			if (controlType.ToString ().EndsWith ("Dark", StringComparison.Ordinal)) {
				nativeView.Appearance = NSAppearance.GetAppearance (NSAppearance.NameDarkAqua);
				nativeView.Layer.BackgroundColor = NSColor.FromRgb (red: 0.18f, green: 0.18f, blue: 0.18f).CGColor;
			}  else {
				nativeView.Layer.BackgroundColor = NSColor.FromRgb (red: 0.96f, green: 0.96f, blue: 0.96f).CGColor;
			}
			return view;
		}

		public override string ConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			return string.Empty;
		}
	}
}
