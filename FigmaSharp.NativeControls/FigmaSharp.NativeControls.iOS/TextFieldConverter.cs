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
using System.Linq;
using FigmaSharp.NativeControls.Base;
using UIKit;

namespace FigmaSharp.NativeControls
{
    public class TextFieldConverter : TextFieldConverterBase
    {
        public override IViewWrapper ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
            var view = new UITextField();
            var keyValues = GetKeyValues(currentNode);

            foreach (var key in keyValues)
            {
                if (key.Key == "type")
                {
                    continue;
                }
                if (key.Key == "enabled")
                {
                    view.Enabled = key.Value == "true";
                }
                else if (key.Key == "size")
                {
                    //view.ControlSize = ToEnum<NSControlSize>(key.Value);
                }
            }

            if (currentNode is IFigmaDocumentContainer container)
            {
                var placeholderView = container.children.OfType<FigmaText>()
            .FirstOrDefault(s => s.name == "placeholderstring");
                if (placeholderView != null)
                {
                    view.Placeholder = placeholderView.characters;
                }

                var textFieldView = container.children.OfType<FigmaText>()
                   .FirstOrDefault(s => s.name == "text");
                if (textFieldView != null)
                {
                    view.Text = textFieldView.characters;
                    view.Configure(textFieldView);
                }
                else
                {
                    view.Configure(currentNode);
                }
            }
            else
            {
                view.Configure(currentNode);
            }

            return new ViewWrapper(view);
        }

        public override string ConvertToCode(FigmaNode currentNode, ProcessedNode parent)
        {
            return string.Empty;
        }
    }
}
