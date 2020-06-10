// Authors:
//   Hylke Bons <hylbo@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
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
using System.Text;

using AppKit;

using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Controls.Cocoa.Converters
{
    public class StackViewConverter : CocoaConverter
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSStackView);
        public override bool ScanChildren(FigmaNode currentNode) => true;

        public override bool CanConvert(FigmaNode currentNode)
        {
            if (currentNode is FigmaFrame frame)
            {
                if (frame.LayoutMode == FigmaLayoutMode.Horizontal ||
                    frame.LayoutMode == FigmaLayoutMode.Vertical)
                {
                    return true;
                }
            }

            return false;
        }


        protected override IView OnConvertToView (FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
        {
            var frame = (FigmaFrame)currentNode;
            var stackView = new NSStackView() { Spacing = frame.itemSpacing };

            stackView.EdgeInsets = new NSEdgeInsets(
                top:    frame.verticalPadding,
                left:   frame.horizontalPadding,
                bottom: frame.verticalPadding,
                right:  frame.horizontalPadding);

            stackView.Orientation =
                (frame.LayoutMode == FigmaLayoutMode.Horizontal)
                    ? NSUserInterfaceLayoutOrientation.Horizontal
                    : NSUserInterfaceLayoutOrientation.Vertical;

            stackView.Distribution = NSStackViewDistribution.Fill;

            return new View(stackView);
        }


        protected override StringBuilder OnConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            var code = new StringBuilder();
            string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

            var frame = (FigmaFrame)currentNode.Node;


            return code;
        }
    }
}
