/* 
* FigmaTextConverter.cs
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

using System;
using System.Text;

using FigmaSharp.Converters;
using FigmaSharp.Models;

using LiteForms;
using LiteForms.Cocoa;

namespace FigmaSharp.Cocoa.Converters
{
    public class FigmaTextConverter : FigmaTextConverterBase
    {
        public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
            var figmaText = ((FigmaText)currentNode);
            Console.WriteLine("'{0}' with Font:'{1}({2})' s:{3} w:{4} ...", figmaText.characters, figmaText.style.fontFamily, figmaText.style.fontPostScriptName, figmaText.style.fontSize, figmaText.style.fontWeight);

            var font = figmaText.style.ToNSFont();
            var textField = ViewsHelper.CreateLabel(figmaText.characters, font);
            textField.Configure(figmaText);
            var wrapper = new View(textField);
            return wrapper;
        }

        public override string ConvertToCode(FigmaNode currentNode)
        {
            var figmaText = ((FigmaText)currentNode);
            StringBuilder builder = new StringBuilder();
            var name = "[NAME]";
            builder.AppendLine(string.Format ("var {0} = {1};", name, FigmaExtensions.CreateLabelToDesignerString (figmaText.characters)));

            var nsFont = figmaText.style.ToNSFont();
            builder.AppendLine(string.Format("{0}.Font = {1};", name, figmaText.style.ToNSFontDesignerString()));

            builder.Configure(name, (FigmaText)currentNode);
            return builder.ToString();
        }
    }
}
