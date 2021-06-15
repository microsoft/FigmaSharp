﻿// Authors:
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
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Controls.Cocoa.Converters
{
	public class ColorWellConverter : CocoaConverter
	{
		public override Type GetControlType(FigmaNode currentNode) => typeof(NSColorWell);

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var colorType) &&
				colorType == FigmaControlType.ColorWell;
		}


		protected override IView OnConvertToView(FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
		{
			var colorWell = new NSColorWell();
			var frame = (FigmaFrame)currentNode;

			FigmaVector rectangle = frame.children
				.OfType<FigmaVector>()
				.FirstOrDefault(s => s.name == ComponentString.VALUE);

			foreach (var styleMap in rectangle?.styles)
			{
				if (rendererService.NodeProvider.TryGetStyle(styleMap.Value, out FigmaStyle style))
				{
					if (styleMap.Key == "fill")
						colorWell.Color = ColorService.GetNSColor(style.name);
				}
			}

			return new View(colorWell);
		}


		protected override StringBuilder OnConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
		{
			var code = new StringBuilder();
			string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

			var frame = (FigmaFrame)currentNode.Node;
			currentNode.Node.TryGetNativeControlType(out FigmaControlType controlType);
			currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

			if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
				code.WriteConstructor(name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

			FigmaVector rectangle = frame.children
	            .OfType<FigmaVector>()
	            .FirstOrDefault(s => s.name == ComponentString.VALUE);

			foreach (var styleMap in rectangle?.styles)
			{
				if ((rendererService.NodeProvider as NodeProvider).TryGetStyle(styleMap.Value, out FigmaStyle style))
				{
					if (styleMap.Key == "fill")
						code.WritePropertyEquality(name, nameof(NSColorWell.Color), ColorService.GetNSColorString(style.name));
				}
			}

			return code;
		}
	}
}
