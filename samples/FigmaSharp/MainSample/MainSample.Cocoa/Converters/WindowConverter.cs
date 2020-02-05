using System;
using AppKit;
using FigmaSharp;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views.Cocoa;
using System.Linq;
using FigmaSharp.Views;

namespace FigmaSharp.Samples
{
	class SheetDialogConverter : FigmaViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			if (currentNode is FigmaFrameEntity figmaFrameEntity && figmaFrameEntity.children.Any (s => s is FigmaInstance instance && instance.name.In ("Window/Sheet Dark", "Window/Sheet"))
						) 
				return true;
				
			return false;
		}

		static CoreGraphics.CGColor backgroundWindowColor =
			(true ?
			NSColor.FromRgba (0.93f, 0.93f, 0.93f, 0.66f) :
			NSColor.FromRgba (0.29f, 0.29f, 0.29f, 0.66f)).CGColor;

		public override IView ConvertTo (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var view = new View ();
			var nativeView = view.NativeObject as NSView;
			nativeView.Configure (currentNode);
			nativeView.Layer.BackgroundColor = backgroundWindowColor;
			return view;
		}

		public override string ConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			return string.Empty;
		}
	}

	class WindowConverter : FigmaViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			string id;
			var nextSpace = currentNode.name.IndexOf (' ');
			if (nextSpace > 0) {
				id = currentNode.name.Substring (0, nextSpace);
			} else {
				id = currentNode.name;
			}

			if (id == "content") {
				if (currentNode.Parent is FigmaFrameEntity frameEntity &&
					frameEntity.children.Any (s => s is FigmaInstance instance && instance.name.In ("Window/Standard", "Window/Standard Dark"))
					)
					return true;
			}

			if (currentNode is FigmaFrameEntity figmaFrameEntity && figmaFrameEntity.children.Any (s => s is FigmaInstance instance && instance.name.In ("Window/Standard", "Window/Standard Dark"))
					)
				return true;

			return false;
		}

		static CoreGraphics.CGColor backgroundWindowColor =
			(true ?
			NSColor.FromRgb (red: 0.96f, green: 0.96f, blue: 0.96f) :
			NSColor.FromRgb (red: 0.18f, green: 0.18f, blue: 0.18f)).CGColor;

		public override IView ConvertTo (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var view = new View ();
			var nativeView = view.NativeObject as NSView;
			nativeView.Configure (currentNode);
			nativeView.Layer.BackgroundColor = backgroundWindowColor;
			return view;
		}

		public override string ConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			return string.Empty;
		}
	}
}
