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
using System.Linq;
using FigmaSharp.NativeControls.Base;
using UIKit;

using FigmaSharp;

namespace FigmaSharp.NativeControls
{
    public class ButtonConverter : ButtonConverterBase
    {
        public override IViewWrapper ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
            var view = new UIButton();
            view.Configure(currentNode);

            //if (currentNode is IFigmaDocumentContainer documentContainer)
            //{
            //    view.BackgroundColor = documentContainer.backgroundColor.ToUIColor();
            //    var figmaText = documentContainer.children.OfType<FigmaText>().FirstOrDefault();
            //    if (figmaText != null)
            //    {
            //        view.Alpha = figmaText.opacity;
            //        view.Font = figmaText.style.ToUIFont();
            //        view.SetTitle(figmaText.characters, UIControlState.Normal);
            //    }
            //    return new ViewWrapper(view);
            //}

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
                 //   view. = ToEnum<NSControlSize>(key.Value);
                }
                else if (key.Key == "style")
                {
                    //view.sth = ToEnum<NSBezelStyle>(key.Value);
                }
                else if (key.Key == "buttontype")
                {
                    //view.ButtonType = UIButtonType. (ToEnum<NSButtonType>(key.Value));
                }
            }
            if (currentNode is IFigmaDocumentContainer instance)
            {
                var figmaText = instance.children.OfType<FigmaText>().FirstOrDefault(s => s.name == "title");
                if (figmaText != null)
                {
                    view.Alpha = figmaText.opacity;
                    view.Font = figmaText.style.ToUIFont();
                    view.SetTitle (figmaText.characters, UIControlState.Normal);
                }

                var image = instance.children.OfType<FigmaVectorEntity>().FirstOrDefault(s => s.name == "image");
                if (image != null)
                {
                    var paint = image.fills.OfType<FigmaPaint>().FirstOrDefault();
                    if (paint != null)
                    {
                        //var query = new FigmaImageQuery ()
                        //FigmaApiHelper.GetFigmaImage (new FigmaImageQuery ())
                    }
                }
            }

            return new ViewWrapper(view);
        }

        public override string ConvertToCode(FigmaNode currentNode, ProcessedNode parent)
        {
            return string.Empty;
        }
    }
}
