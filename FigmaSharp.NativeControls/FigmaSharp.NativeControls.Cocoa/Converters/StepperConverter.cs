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

using System.Text;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Native.Cocoa;
using FigmaSharp.NativeControls;
using System;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class StepperConverter : FigmaNativeControlConverter
	{
		public override Type GetControlType(FigmaNode currentNode)
		{
			return typeof(NSStepper);
		}

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var value) && value == NativeControlType.Stepper;
		}

		protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var stepper = new NSStepper();

			var frame = (FigmaFrameEntity)currentNode;
			frame.TryGetNativeControlVariant(out var controlVariant);

			stepper.Configure (frame);

			switch (controlVariant) {
				case NativeControlVariant.Regular:
					stepper.ControlSize = NSControlSize.Regular;
					break;
				case NativeControlVariant.Small:
					stepper.ControlSize = NSControlSize.Small;
					break;
			}

			return new View(stepper);
		}


		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var code = new StringBuilder ();

			string name = currentNode.Name;

			if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
				code.WriteConstructor (name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

			var frame = (FigmaFrameEntity) currentNode.Node;
			frame.TryGetNativeControlVariant (out var controlVariant);

            code.Configure (frame, name);

			switch (controlVariant) {
				case NativeControlVariant.Regular:
					code.WriteEquality (name, nameof (NSStepper.ControlSize), NSControlSize.Regular);
					break;
				case NativeControlVariant.Small:
					code.WriteEquality (name, nameof (NSStepper.ControlSize), NSControlSize.Small);
					break;
			}

			return code;
		}
	}
}
