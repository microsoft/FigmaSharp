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
	public abstract class ProgressIndicatorConverter : CocoaConverter
    {
		public override Type GetControlType(FigmaNode currentNode) => typeof(NSProgressIndicator);


		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			var frame = (FigmaFrame)currentNode;

			var progressIndicator = new NSProgressIndicator();
			progressIndicator.Configure(frame);

			frame.TryGetNativeControlType(out var controlType);
			frame.TryGetNativeControlVariant(out var controlVariant);

			progressIndicator.ControlSize = GetNSControlSize(controlVariant);

			FigmaGroup group = frame.children
				.OfType<FigmaGroup>()
				.FirstOrDefault(s => (s.name == ComponentString.STYLE_DETERMINATE || s.name == ComponentString.STYLE_INDETERMINATE) && s.visible);

			if (group?.name == ComponentString.STYLE_DETERMINATE)
			{
				progressIndicator.Indeterminate = false;
				progressIndicator.MinValue = 0;
				progressIndicator.MaxValue = 1;
				progressIndicator.DoubleValue = 0.618;
			}

			if (group.name == ComponentString.STYLE_INDETERMINATE)
				progressIndicator.Indeterminate = true;

			return new View(progressIndicator);
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

			code.WriteEquality(name, nameof(NSButton.ControlSize), GetNSControlSize(controlVariant));

			FigmaGroup group = frame.children
				.OfType<FigmaGroup>()
				.FirstOrDefault(s => (s.name == ComponentString.STYLE_DETERMINATE || s.name == ComponentString.STYLE_INDETERMINATE) && s.visible);

			if (group != null)
			{
				if (group.name == ComponentString.STYLE_DETERMINATE)
				{
					code.WriteEquality(name, nameof(NSProgressIndicator.Indeterminate), false);
					code.WriteEquality(name, nameof(NSProgressIndicator.MinValue), "0");
					code.WriteEquality(name, nameof(NSProgressIndicator.MaxValue), "1");
					code.WriteEquality(name, nameof(NSProgressIndicator.DoubleValue), "0.618");
				}

				if (group.name == ComponentString.STYLE_INDETERMINATE)
					code.WriteEquality(name, nameof(NSProgressIndicator.Indeterminate), true);
			}

			return code;
		}
	}


	public class ProgressIndicatorCircularConverter : ProgressIndicatorConverter
	{
		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var controlType) &&
				controlType == NativeControlType.ProgressIndicatorCircular;
		}


		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			IView view = base.OnConvertToView(currentNode, parentNode, rendererService);
			(view.NativeObject as NSProgressIndicator).Style = NSProgressIndicatorStyle.Spinning;

			return view;
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			StringBuilder code = base.OnConvertToCode(currentNode, parentNode, rendererService);
			string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

			code.WriteEquality(name, nameof(NSProgressIndicator.Style), NSProgressIndicatorStyle.Spinning);

			return code;
		}
	}


	public class ProgressIndicatorBarConverter : ProgressIndicatorConverter
	{
		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var controlType) &&
				controlType == NativeControlType.ProgressIndicatorBar;
		}


		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			IView view = base.OnConvertToView(currentNode, parentNode, rendererService);
			(view.NativeObject as NSProgressIndicator).Style = NSProgressIndicatorStyle.Bar;

			return view;
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			StringBuilder code = base.OnConvertToCode(currentNode, parentNode, rendererService);
			string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

			code.WriteEquality(name, nameof(NSProgressIndicator.Style), NSProgressIndicatorStyle.Bar);

			return code;
		}
	}
}
