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
using FigmaSharp.Cocoa;
using FigmaSharp.Controls.Cocoa.Helpers;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Controls.Cocoa.Converters
{
    public class StackViewConverter : CocoaConverter
    {
        public static class Properties
        {
            public static string EdgeInsets = "EdgeInsets";
            public static string Spacing = "Spacing";
            public static string Orientation = "Orientation";
            public static string Distribution = "Distribution";
        }

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

        public void ConfigureProperty(string propertyName, FigmaNode node, IView view)
        {
            var frame = (FigmaFrame)node;

            var stackView = (NSStackView)view.NativeObject;
            if (propertyName == Properties.EdgeInsets)
            {
                stackView.EdgeInsets = new NSEdgeInsets(
                   top: frame.verticalPadding,
                   left: frame.horizontalPadding,
                   bottom: frame.verticalPadding,
                   right: frame.horizontalPadding
                );
                return;
            }

            if (propertyName == Properties.Spacing)
            {
                stackView.Spacing = frame.itemSpacing;
                return;
            }

            if (propertyName == Properties.Orientation)
            {
                stackView.Orientation = frame.LayoutMode == FigmaLayoutMode.Horizontal ?
               NSUserInterfaceLayoutOrientation.Horizontal : NSUserInterfaceLayoutOrientation.Vertical;
                return;
            }

            if (propertyName == Properties.Distribution)
            {
                stackView.Distribution = NSStackViewDistribution.FillEqually;
                return;
            }
        }

        protected override IView OnConvertToView (FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
        {
            var view = new View(new NSStackView());

            ConfigureProperty(Properties.EdgeInsets, currentNode, view);
            ConfigureProperty(Properties.Spacing, currentNode, view);
            ConfigureProperty(Properties.Orientation, currentNode, view);
            ConfigureProperty(Properties.Distribution, currentNode, view);

            return view;
        }

        protected override StringBuilder OnConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            var code = new StringBuilder();

            string identifier = Resources.Ids.Conversion.NameIdentifier;

            var frame = (FigmaFrame)currentNode.Node;

            currentNode.Node.TryGetNativeControlType(out FigmaControlType controlType);
            currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

            if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
                code.WriteConstructor(identifier, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

            code.WriteEquality(identifier, nameof(NSButton.ControlSize), ViewHelper.GetNSControlSize(controlVariant));

            var edgeInsets = typeof(NSEdgeInsets).GetConstructor(
                frame.verticalPadding.ToString (),
                frame.horizontalPadding.ToString(),
                frame.verticalPadding.ToString(),
                frame.horizontalPadding.ToString());

            code.WriteEquality(identifier, nameof(NSStackView.Spacing), edgeInsets);

            var orientation = frame.LayoutMode == FigmaLayoutMode.Horizontal ?
                NSUserInterfaceLayoutOrientation.Horizontal : NSUserInterfaceLayoutOrientation.Vertical;

            code.WriteEquality(identifier, nameof(NSStackView.Orientation), orientation.GetFullName ());
            code.WriteEquality(identifier, nameof(NSStackView.Distribution), NSStackViewDistribution.FillEqually.GetFullName());
           
            return code;
        }
    }
}
