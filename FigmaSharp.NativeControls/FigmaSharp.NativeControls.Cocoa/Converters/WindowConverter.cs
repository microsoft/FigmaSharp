// Authors:
//   Jose Medrano <josmed@microsoft.com>
//   Hylke Bons <hylbo@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Text;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class ImageRenderConverter : CocoaConverter
	{
		public override Type GetControlType(FigmaNode currentNode) => typeof(NSImageView);

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.name.StartsWith("!image");
		}

		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			var vector = new FigmaSharp.Views.Cocoa.ImageView();
			var currengroupView = (NSImageView)vector.NativeObject;
			currengroupView.Configure(currentNode);
			return vector;
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var builder = new StringBuilder();
			if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
				builder.WriteConstructor(currentNode.Name, GetControlType (currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

			builder.Configure(currentNode.Node, Resources.Ids.Conversion.NameIdentifier);
			currentNode.Node.TryGetNodeCustomName(out string nodeName);

			var imageNamedMethod = CodeGenerationHelpers.GetMethod(typeof(NSImage).FullName, nameof(NSImage.ImageNamed), nodeName, true);
			builder.WriteEquality(currentNode.Name, nameof(NSImageView.Image), imageNamedMethod);

			return builder;
		}
	}

	public class WindowConverter : CocoaConverter
	{
		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var controlType) &&
				controlType == NativeControlType.Window;
		}

		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			return null;
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			return new StringBuilder();
		}

		public override Type GetControlType(FigmaNode currentNode)
		{
			return null;
		}
	}

	public class WindowSheetConverter : CocoaConverter
	{
		public override Type GetControlType(FigmaNode currentNode) => typeof(NSView);

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var controlType) &&
				controlType == NativeControlType.WindowSheet;
		}

		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			return null;
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			return new StringBuilder();
		}
	}

	public class WindowPanelConverter : CocoaConverter
	{
		public override Type GetControlType(FigmaNode currentNode) => typeof(NSPanel);

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var controlType) &&
                controlType == NativeControlType.WindowPanel;
		}

		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			return null;
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			return new StringBuilder();
		}
	}
}
