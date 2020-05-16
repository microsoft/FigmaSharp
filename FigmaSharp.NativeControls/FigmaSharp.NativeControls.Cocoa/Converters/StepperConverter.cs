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
using System.Text;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class StepperConverter : CocoaConverter
	{
		public override Type GetControlType(FigmaNode currentNode) => typeof(NSStepper);


		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var controlType) &&
                controlType == NativeControlType.Stepper;
		}


		protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			var stepper = new NSStepper();

			var frame = (FigmaFrame)currentNode;
			frame.TryGetNativeControlVariant(out var controlVariant);

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

			var frame = (FigmaFrame) currentNode.Node;
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
