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

namespace FigmaSharp.NativeControls.Cocoa
{
    public class StepperConverter : FigmaNativeControlConverter
	{
		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var value) && value == NativeControlType.Stepper;
		}

		protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parent, FigmaViewRendererService rendererService)
		{
			var instance = (FigmaFrameEntity)currentNode;
			var view = new Stepper ();
			var nativeView = (FNSStepper)view.NativeObject;
			nativeView.Configure (instance);

			instance.TryGetNativeControlComponentType (out var controlType);
			switch (controlType) {
				case NativeControlComponentType.StepperSmall:
				case NativeControlComponentType.StepperSmallDark:
					nativeView.ControlSize = NSControlSize.Small;
					break;
				case NativeControlComponentType.StepperStandard:
				case NativeControlComponentType.StepperStandardDark:
					nativeView.ControlSize = NSControlSize.Regular;
					break;
			}

			if (controlType.ToString ().EndsWith ("Dark", System.StringComparison.Ordinal)) {
				nativeView.Appearance = NSAppearance.GetAppearance (NSAppearance.NameDarkAqua);
			} else {
				nativeView.Appearance = NSAppearance.GetAppearance (NSAppearance.NameAqua);
			}

			return view;
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var figmaInstance = (FigmaFrameEntity)currentNode.Node;

			StringBuilder builder = new StringBuilder ();
			string name = currentNode.Name;

			if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
				builder.WriteConstructor (name, typeof (AppKit.NSStepper));

			builder.Configure (figmaInstance, name);

			figmaInstance.TryGetNativeControlComponentType (out var controlType);
			switch (controlType) {
				case NativeControlComponentType.ProgressSpinnerSmall:
				case NativeControlComponentType.ProgressSpinnerSmallDark:
					builder.WriteEquality (name, nameof (NSStepper.ControlSize), NSControlSize.Small);
					break;
				case NativeControlComponentType.ProgressSpinnerStandard:
				case NativeControlComponentType.ProgressSpinnerStandardDark:
					builder.WriteEquality (name, nameof (NSStepper.ControlSize), NSControlSize.Regular);
					break;
			}

			return builder;
		}

	}
}
