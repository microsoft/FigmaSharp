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
using System.Linq;
using System.Text;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class ColorWellConverter : CocoaConverter
	{
		public override Type GetControlType(FigmaNode currentNode) => typeof(NSColorWell);

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var colorType) &&
				colorType == NativeControlType.ColorWell;
		}


		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			var colorWell = new NSColorWell();
			var frame = (FigmaFrame)currentNode;

			FigmaVectorEntity rectangle = frame.children
				.OfType<FigmaVectorEntity>()
				.FirstOrDefault(s => s.name == ComponentString.VALUE);

			foreach (var styleMap in rectangle?.styles)
			{
				if (rendererService.FileProvider.TryGetStyle(styleMap.Value, out FigmaStyle style))
				{
					if (styleMap.Key == "fill")
						colorWell.Color = GetNSColor(style.name);
				}
			}

			return new View(colorWell);
		}


		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var code = new StringBuilder();
			string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

			var frame = (FigmaFrame)currentNode.Node;
			currentNode.Node.TryGetNativeControlType(out NativeControlType controlType);
			currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

			if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
				code.WriteConstructor(name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

			FigmaVectorEntity rectangle = frame.children
	            .OfType<FigmaVectorEntity>()
	            .FirstOrDefault(s => s.name == ComponentString.VALUE);

			foreach (var styleMap in rectangle?.styles)
			{
				if ((rendererService.figmaProvider as FigmaFileProvider).TryGetStyle(styleMap.Value, out FigmaStyle style))
				{
					if (styleMap.Key == "fill")
						code.WriteEquality(name, nameof(NSColorWell.Color), GetNSColorName(style.name));
				}
			}

			return code;
		}
	}
}
