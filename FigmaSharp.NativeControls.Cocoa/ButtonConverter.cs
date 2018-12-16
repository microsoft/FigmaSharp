/* 
 * CustomButtonConverter.cs 
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

namespace FigmaSharp.NativeControls
{
    public class ButtonConverter : ButtonConverterBase
    {
        public override IViewWrapper ConvertTo(FigmaNode currentNode, FigmaNode parentNode, IViewWrapper parentView)
        {
            var button = new NSButton() { TranslatesAutoresizingMaskIntoConstraints = false };
            button.Configure(currentNode);

            var instance = (IFigmaDocumentContainer)currentNode;
            var figmaText = instance.children.OfType<FigmaText>().FirstOrDefault();
            if (figmaText != null)
            {
                button.AlphaValue = figmaText.opacity;
                button.Font = figmaText.style.ToNSFont();
            }

            if (instance.children.OfType<FigmaGroup>().Any())
            {
                button.Title = "";
                button.AlphaValue = 0.15f;
                button.BezelStyle = NSBezelStyle.TexturedSquare;
            }
            else
            {
                if (figmaText != null)
                {
                    button.AlphaValue = figmaText.opacity;
                    button.Title = figmaText.characters;
                }

                button.BezelStyle = NSBezelStyle.Rounded;
                button.Layer.BackgroundColor = instance.backgroundColor.ToNSColor().CGColor;
                return null;
            }
            return new ViewWrapper(button);
        }
    }
}
