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
			var controlType = figmaInstance.ToControlType ();
			switch (controlType) {
				case NativeControlType.ProgressSpinnerSmall:
				case NativeControlType.ProgressSpinnerSmallDark:
					nativeView.ControlSize = NSControlSize.Small;
					break;
				case NativeControlType.ProgressSpinnerStandard:
				case NativeControlType.ProgressSpinnerStandardDark:
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
          
            var view = new Spinner ();
            var nativeView = (NSProgressIndicator) view.NativeObject;
          
            if (rendererService.NeedsRenderInstance (currentNode)) {
                builder.AppendLine ($"var {name} = new {typeof (NSProgressIndicator).FullName}();");
            }

            builder.Configure (name, figmaInstance);

            builder.AppendLine (string.Format ("{0}.Style = {1};", name, NSProgressIndicatorStyle.Spinning.GetFullName ()));

			//hidden by default
            builder.AppendLine (string.Format ("{0}.Hidden = {1};", name, (true).ToDesignerString ()));

            var controlType = figmaInstance.ToControlType ();

            switch (controlType) {
                case NativeControlType.PopUpButtonSmall:
                case NativeControlType.PopUpButtonSmallDark:
                    builder.AppendLine (string.Format ("{0}.ControlSize = {1};", name, NSControlSize.Small.GetFullName ()));
                    break;
                case NativeControlType.PopUpButtonStandard:
                case NativeControlType.PopUpButtonStandardDark:
                    builder.AppendLine (string.Format ("{0}.ControlSize = {1};", name, NSControlSize.Regular.GetFullName ()));
                    break;
            }

            return builder.ToString ();
		}
	}
}
