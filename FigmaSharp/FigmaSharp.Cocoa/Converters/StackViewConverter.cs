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
                node.TryGetAttributeValue (DistributionPropertyName, out var value);

                NSStackViewDistribution distribution = NSStackViewDistribution.Fill;
                if (!string.IsNullOrEmpty (value))
                    Enum.TryParse(value.ToCamelCase(), out distribution);
                stackView.Distribution = distribution;
                return;
            }
        }

        const string DistributionPropertyName = "distribution";

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
         frame.verticalPadding.ToString(),
         frame.horizontalPadding.ToString(),
         frame.verticalPadding.ToString(),
         frame.horizontalPadding.ToString());
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
                var orientation = frame.LayoutMode == FigmaLayoutMode.Horizontal ?
             NSUserInterfaceLayoutOrientation.Horizontal : NSUserInterfaceLayoutOrientation.Vertical;
                code.WritePropertyEquality(codeNode.Name, nameof(NSStackView.Orientation), orientation.GetFullName());
                return;
            }

            if (propertyName == Properties.Distribution)
            {
                codeNode.Node.TryGetAttributeValue(DistributionPropertyName, out var value);
                NSStackViewDistribution distribution = NSStackViewDistribution.Fill;
                if (!string.IsNullOrEmpty(value))
                {
                    var parameter = typeof(NSStackViewDistribution).WithProperty(value.ToCamelCase());
                    Enum.TryParse(parameter, out distribution); ;
                }

                code.WritePropertyEquality(codeNode.Name, nameof(NSStackView.Distribution), distribution.GetFullName());
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
