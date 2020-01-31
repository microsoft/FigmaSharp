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

namespace FigmaSharp.NativeControls.Cocoa
{
    public partial class TextFieldConverter : TextFieldConverterBase
    {
        public override IView ConvertTo (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var instance = (FigmaInstance)currentNode;

            var textBox = new TextBox ();
            var view = (NSTextField)textBox.NativeObject;

            var figmaInstance = (FigmaInstance)currentNode;
            var controlType = figmaInstance.ToControlType ();
            switch (controlType) {
                case NativeControlType.TextFieldSmall:
                case NativeControlType.TextFieldSmallDark:
                    view.ControlSize = NSControlSize.Small;
                    break;
                case NativeControlType.TextFieldStandard:
                case NativeControlType.TextFieldStandardDark:
                    view.ControlSize = NSControlSize.Regular;
                    break;
            }

            var texts = instance.children
                .OfType<FigmaText> ();

            var text = texts.FirstOrDefault (s => s.name == "lbl");
            if (text != null) {
                textBox.Text = text.characters;
                view.Configure (text);
            }

            var placeholder = texts.FirstOrDefault (s => s.name == "placeholder");
            if (placeholder != null)
                view.PlaceholderString = placeholder.characters;

            //if (controlType.ToString ().EndsWith ("Dark", System.StringComparison.Ordinal)) {
            //    view.Appearance = NSAppearance.GetAppearance (NSAppearance.NameDarkAqua);
            //}

            return textBox;
        }

        public override string ConvertToCode (FigmaNode currentNode, FigmaCodeRendererService rendererService)
        {
            StringBuilder builder = new StringBuilder ();
            builder.AppendLine ($"var {FigmaSharp.Resources.Ids.Conversion.NameIdentifier} = new {typeof (NSTextField).FullName}();");
            if (currentNode is IFigmaDocumentContainer container) {
                var figmaText = ((IFigmaDocumentContainer)currentNode).children.OfType<FigmaText> ()
       .FirstOrDefault ();

                var text = figmaText.characters ?? string.Empty;
                var isMultiline = text.Contains ('\n');

                builder.AppendLine (string.Format ("{0}.StringValue = {1}\"{2}\";",
                    FigmaSharp.Resources.Ids.Conversion.NameIdentifier,
                    isMultiline ? "@" : "",
                     isMultiline ? text.Replace ("\"", "\"\"") : text));
            }
            builder.Configure (FigmaSharp.Resources.Ids.Conversion.NameIdentifier, currentNode);
            return builder.ToString ();
        }
    }
}
