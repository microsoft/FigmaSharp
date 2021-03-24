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
using System.Text;

using AppKit;

using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Cocoa.Converters
{
    public class StackViewConverter : StackViewBase
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSStackView);
        public override bool ScanChildren(FigmaNode currentNode) => true;

        public void ConfigureProperty(string propertyName, FigmaNode node, IView view)
        {
            var frame = (FigmaFrame)node;
            var stackView = (NSStackView)view.NativeObject;

            if (propertyName == Properties.EdgeInsets)
            {
                stackView.EdgeInsets = new NSEdgeInsets(
                   top: frame.paddingTop,
                   left: frame.paddingLeft,
                   bottom: frame.paddingBottom,
                   right: frame.paddingRight
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
                stackView.Orientation = NSUserInterfaceLayoutOrientation.Horizontal;
                stackView.Alignment = NSLayoutAttribute.Top;

                if (frame.LayoutMode == FigmaLayoutMode.Horizontal)
                {
                    switch (frame.counterAxisAlignItems)
                    {
                        case "MIN":    stackView.Alignment = NSLayoutAttribute.Top; break;
                        case "CENTER": stackView.Alignment = NSLayoutAttribute.CenterY; break;
                        case "MAX":    stackView.Alignment = NSLayoutAttribute.Bottom; break;
                    }

                } else {
                    stackView.Orientation = NSUserInterfaceLayoutOrientation.Vertical;

                    switch (frame.counterAxisAlignItems)
                    {
                        case "MIN":    stackView.Alignment = NSLayoutAttribute.Left; break;
                        case "CENTER": stackView.Alignment = NSLayoutAttribute.CenterX; break;
                        case "MAX":    stackView.Alignment = NSLayoutAttribute.Right; break;
                    }
                }

                return;
            }

            if (propertyName == Properties.Distribution)
            {
                stackView.Distribution = NSStackViewDistribution.FillProportionally;
                return;
            }
        }

        public override IView ConvertToView (FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var view = new View(new NSStackView());

            ConfigureProperty(Properties.EdgeInsets, currentNode, view);
            ConfigureProperty(Properties.Spacing, currentNode, view);
            ConfigureProperty(Properties.Orientation, currentNode, view);
            ConfigureProperty(Properties.Distribution, currentNode, view);

            return view;
        }

        public void ConfigureCodeProperty(string propertyName, CodeNode codeNode, StringBuilder code)
        {
            var frame = (FigmaFrame)codeNode.Node;

            if (propertyName == Properties.EdgeInsets)
            {
                var edgeInsets = typeof(NSEdgeInsets).GetConstructor(
                    frame.paddingTop.ToString(),
                    frame.paddingRight.ToString(),
                    frame.paddingBottom.ToString(),
                    frame.paddingLeft.ToString());
                code.WritePropertyEquality(codeNode.Name, nameof(NSStackView.EdgeInsets), edgeInsets);
                return;
            }

            if (propertyName == Properties.Spacing)
            {
                code.WritePropertyEquality(codeNode.Name, nameof(NSStackView.Spacing), frame.itemSpacing.ToDesignerString ());
                return;
            }

            if (propertyName == Properties.Orientation)
            {
                NSUserInterfaceLayoutOrientation orientation = NSUserInterfaceLayoutOrientation.Horizontal;
                NSLayoutAttribute layoutAttribute = NSLayoutAttribute.Top;

                if (frame.LayoutMode == FigmaLayoutMode.Horizontal)
                {
                    switch (frame.counterAxisAlignItems)
                    {
                        case "MIN":    layoutAttribute = NSLayoutAttribute.Top; break;
                        case "CENTER": layoutAttribute = NSLayoutAttribute.CenterY; break;
                        case "MAX":    layoutAttribute = NSLayoutAttribute.Bottom; break;
                    }
                } else {
                    orientation = NSUserInterfaceLayoutOrientation.Vertical;

                    switch (frame.counterAxisAlignItems)
                    {
                        case "MIN":    layoutAttribute = NSLayoutAttribute.Left; break;
                        case "CENTER": layoutAttribute = NSLayoutAttribute.CenterX; break;
                        case "MAX":    layoutAttribute = NSLayoutAttribute.Right; break;
                    }
                }

                code.WritePropertyEquality(codeNode.Name, nameof(NSStackView.Orientation), orientation.GetFullName());
                code.WritePropertyEquality(codeNode.Name, nameof(NSStackView.Alignment), layoutAttribute.GetFullName());

                return;
            }

            if (propertyName == Properties.Distribution)
            {
                code.WritePropertyEquality(codeNode.Name, nameof(NSStackView.Distribution), NSStackViewDistribution.FillProportionally.GetFullName());
                return;
            }
        }

        public override string ConvertToCode (CodeNode codeNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            var code = new StringBuilder();

            if (rendererService.NeedsRenderConstructor(codeNode, parentNode))
                code.WriteConstructor(codeNode.Name, GetControlType(codeNode.Node), rendererService.NodeRendersVar(codeNode, parentNode));

            code.WritePropertyEquality(codeNode.Name, nameof(NSView.TranslatesAutoresizingMaskIntoConstraints), false);

            ConfigureCodeProperty(Properties.EdgeInsets, codeNode, code);
            ConfigureCodeProperty (Properties.Spacing, codeNode, code);
            ConfigureCodeProperty (Properties.Orientation, codeNode, code);
            ConfigureCodeProperty (Properties.Distribution, codeNode, code);
           
            return code.ToString ();
        }
    }
}
