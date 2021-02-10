// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using System.Windows.Controls;
using FigmaSharp.Controls;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Wpf; 

namespace FigmaSharp.Wpf.Converters
{
    public class TextBlockConverter : TextConverterBase
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(TextBlock);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);
            Console.WriteLine(controlType);
            return controlType == FigmaControlType.TextBlock;
        }

        public override IView ConvertToView (FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var textBlock = new TextBlock();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            switch (controlType)
            {
                case FigmaControlType.TextBlock:
                    // apply any styles here
                    break;
            }

            FigmaText text = frame.children
                    .OfType<FigmaText>()
                    .FirstOrDefault(s => s.name == ComponentString.TITLE);

            textBlock.Configure(text);

            var wrapper = new View(textBlock);
            return wrapper;
        }
         
        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        } 
    }
}
