// Authors:
//   Jose Medrano <josmed@microsoft.com>
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
using System.Collections.Generic;
using System.Text;

using AppKit;

using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Cocoa;
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
            var stackView = new NSStackView();

            stackView.EdgeInsets = new NSEdgeInsets(
                top:    frame.verticalPadding,
                left:   frame.horizontalPadding,
                bottom: frame.verticalPadding,
                right:  frame.horizontalPadding);

            stackView.Spacing = frame.itemSpacing;


            // Decide on the NSStackView's gravity by looking at the constraints.
            // This is not a perfect behavior, but makes sense for most cases
            var gravity = NSStackViewGravity.Leading;

            if (frame.LayoutMode == FigmaLayoutMode.Horizontal)
            {
                stackView.Orientation = NSUserInterfaceLayoutOrientation.Horizontal;

                if (frame.constraints.horizontal == "RIGHT")
                    gravity = NSStackViewGravity.Trailing;
            }
            else
            {
                stackView.Orientation = NSUserInterfaceLayoutOrientation.Vertical;

                if (frame.constraints.vertical == "BOTTOM")
                    gravity = NSStackViewGravity.Trailing;
            }


            var converters = FigmaControlsContext.Current.GetConverters();

            foreach (FigmaNode node in currentNode.GetChildren(t => t.visible, reverseChildren: true))
            {
                // TODO: Convert each child node and add it

                var control = new NSButton()
                {
                    Title = "test123",
                    BezelStyle = NSBezelStyle.Rounded,
                };

                stackView.AddView(control, gravity);
            }

            return new View(stackView);
        }


        protected override StringBuilder OnConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            var code = new StringBuilder();
            string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

            var frame = (FigmaFrame)currentNode.Node;

            /*
            currentNode.Node.TryGetNativeControlType(out FigmaControlType controlType);
            currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

            if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
                code.WriteConstructor(name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

            var itemNodes = frame.FirstChild(s => s.name == ComponentString.ITEMS);

            if (itemNodes == null)
                return null;

            code.AppendLine();
            code.AppendLine($"{ name }.{ nameof(NSTabView.SetItems) }(");
            code.AppendLine($"\tnew { typeof(NSTabViewItem[]) }");
            code.AppendLine("\t{");

            foreach (FigmaNode tabNode in itemNodes.GetChildren(t => t.visible, reverseChildren: true))
            {
                var firstChild = tabNode.FirstChild(s => s.name.In(ComponentString.STATE_REGULAR, ComponentString.STATE_SELECTED) && s.visible);

                if (firstChild != null)
                {
                    FigmaText text = firstChild.FirstChild(s => s.name == ComponentString.TITLE) as FigmaText;

                    if (text != null)
                        code.AppendLine($"\t\tnew {typeof(NSTabViewItem)}() {{ {nameof(NSTabViewItem.Label)} = \"{text.characters}\" }},");
                }
            }

            code.AppendLine("\t}");
            code.AppendLine(");");
            */
            return code;
        }
    }
}
