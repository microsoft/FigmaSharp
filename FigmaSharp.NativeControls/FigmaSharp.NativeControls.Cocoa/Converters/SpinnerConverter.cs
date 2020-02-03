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
using AppKit;
using FigmaSharp.NativeControls.Base;
using System.Linq;
using System.Text;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Views;
using FigmaSharp.Services;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class SpinnerConverter : SpinnerConverterBase
	{
		public override IView ConvertTo (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var instance = (FigmaInstance)currentNode;
			var view = new Spinner ();
			var nativeView = (FNSProgressIndicator)view.NativeObject;
			nativeView.Configure (instance);

			var figmaInstance = (FigmaInstance)currentNode;
			var controlType = figmaInstance.ToNativeControlComponentType ();
			switch (controlType) {
				case NativeControlComponentType.ProgressSpinnerSmall:
				case NativeControlComponentType.ProgressSpinnerSmallDark:
					nativeView.ControlSize = NSControlSize.Small;
					break;
				case NativeControlComponentType.ProgressSpinnerStandard:
				case NativeControlComponentType.ProgressSpinnerStandardDark:
					nativeView.ControlSize = NSControlSize.Regular;
					break;
			}
			//if (controlType.ToString ().EndsWith ("Dark", System.StringComparison.Ordinal)) {
			//	nativeView.Appearance = NSAppearance.GetAppearance (NSAppearance.NameDarkAqua);
			//}

			return view;
		}

		public override string ConvertToCode (FigmaNode currentNode, FigmaCodeRendererService rendererService)
		{
			var figmaInstance = (FigmaInstance)currentNode;

			StringBuilder builder = new StringBuilder ();
			string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

			if (rendererService.NeedsRenderInstance (currentNode))
				builder.WriteConstructor (name, typeof (NSProgressIndicator));

			builder.Configure (name, figmaInstance);

			builder.WriteEquality (name, nameof (NSProgressIndicator.Style), NSProgressIndicatorStyle.Spinning);

			//hidden by default
			builder.WriteEquality (name, nameof (NSProgressIndicator.Hidden), true);

			var controlType = figmaInstance.ToNativeControlComponentType ();

			switch (controlType) {
				case NativeControlComponentType.ProgressSpinnerSmall:
				case NativeControlComponentType.ProgressSpinnerSmallDark:
					builder.WriteEquality (name, nameof (NSButton.ControlSize), NSControlSize.Small);
					break;
				case NativeControlComponentType.ProgressSpinnerStandard:
				case NativeControlComponentType.ProgressSpinnerStandardDark:
					builder.WriteEquality (name, nameof (NSButton.ControlSize), NSControlSize.Regular);
					break;
			}

			return builder.ToString ();
		}
	}
}
