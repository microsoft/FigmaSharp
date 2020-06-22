/* 
* TextConverter.cs
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
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace FigmaSharp.Web.Converters
{
    //	<div class="right top text" id="n2_4">dsdssdsdsdsddddddd</div>
    public class TextConverter : TextConverterBase
    {
        public override IView ConvertToView(FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService) => null;

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            var figmaText = (FigmaText)currentNode.Node;

            StringBuilder builder = new StringBuilder();
            builder.Append("<div class=\"right top text\" id =\"n2_4\" > dsdssdsdsdsddddddd</div>");

   //         if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
   //             builder.WriteEquality (currentNode.Name, null, FigmaExtensions.CreateLabelToDesignerString (figmaText.characters), instanciate: true);

   //         //builder.Configure(figmaText, currentNode.Name);
   //         builder.Configure (currentNode.Node, currentNode.Name);

   //         var alignment = FigmaExtensions.ToNSTextAlignment (figmaText.style.textAlignHorizontal);
			//if (alignment != default) {
   //             builder.WriteEquality (currentNode.Name, nameof (AppKit.NSTextField.Alignment), alignment);
   //         }
            return builder.ToString();
        }

        public override Type GetControlType(FigmaNode currentNode)
            => null;
    }
}
