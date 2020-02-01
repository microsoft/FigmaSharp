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
using System;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using System.Linq;
using FigmaSharp.Views;
using FigmaSharp.Services;
using FigmaSharp.Views.Cocoa;
using System.Text;

namespace FigmaSharp.NativeControls.Cocoa
{
    public class ComboBoxConverter : ComboBoxConverterBase
    {
        public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var view = new NSComboBox();

            var figmaInstance = (FigmaInstance)currentNode;
            var controlType = figmaInstance.ToControlType();
            switch (controlType)
            {
                case NativeControlType.ComboBoxSmall:
                case NativeControlType.ComboBoxSmallDark:
                    view.ControlSize = NSControlSize.Small;
                    break;
                case NativeControlType.ComboBoxStandard:
                case NativeControlType.ComboBoxStandardDark:
                    view.ControlSize = NSControlSize.Regular;
                    break;
            }

            //if (controlType.ToString().EndsWith("Dark", StringComparison.Ordinal))
            //{
            //    view.Appearance = NSAppearance.GetAppearance(NSAppearance.NameDarkAqua);
            //}

            return new View(view);
        }

        public override string ConvertToCode(FigmaNode currentNode, FigmaCodeRendererService rendererService)
        {
            var figmaInstance = (FigmaInstance)currentNode;

            var builder = new StringBuilder ();
            var name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

            if (rendererService.NeedsRenderInstance (currentNode))
                builder.AppendLine ($"var {name} = new {typeof (NSComboBox).FullName}();");

            builder.AppendLine (string.Format ("{0}.BezelStyle = {1};", name, NSBezelStyle.Rounded.GetFullName ()));
            builder.AppendLine (string.Format ("{0}.Title = string.Empty;", name));

            builder.Configure (name, currentNode);

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

            var label = figmaInstance.children.OfType<FigmaText> ().FirstOrDefault ();

            if (!string.IsNullOrEmpty (label?.characters)) {
                builder.AppendLine (string.Format ("{0}.AddItem (\"{1}\");", name, label.characters));
            }

            if (controlType.ToString ().EndsWith ("Dark", StringComparison.Ordinal)) {
                builder.AppendLine (string.Format ("{0}.Appearance = NSAppearance.GetAppearance ({1});", name, NSAppearance.NameDarkAqua.GetType ().FullName));
            }

            return builder.ToString ();
        }
    }
}
