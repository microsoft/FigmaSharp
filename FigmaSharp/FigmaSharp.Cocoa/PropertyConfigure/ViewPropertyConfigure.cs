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

using AppKit;

using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Services;

namespace FigmaSharp.Cocoa.PropertyConfigure
{
    public class ViewPropertyConfigure : ViewPropertyConfigureBase
    {
        public override void Configure(string propertyName, ViewNode currentViewNode, ViewNode parentViewNode, NodeConverter converter, ViewRenderService rendererService)
        {
            var currentNode = currentViewNode.Node;
            var parentNode = parentViewNode?.Node;

            if (propertyName == PropertyNames.AddChild)
            {
                if (parentViewNode?.View != null)
                {
                    if (parentNode.IsStackView() && parentViewNode.View.NativeObject is AppKit.NSStackView stackView)
                        stackView.AddArrangedSubview(currentViewNode.View.NativeObject as NSView);
                    else
                        parentViewNode.View.AddChild(currentViewNode.View);
                }
                return;
            }
            if (propertyName == PropertyNames.Constraints)
            {
                if (!rendererService.HasConstraints(currentNode, converter))
                    return;

                if (currentNode is IConstraints constrainedNode && currentViewNode?.View?.NativeObject is AppKit.NSView nativeView && parentViewNode?.View?.NativeObject is AppKit.NSView parentNativeView)
                {
                    var constraints = constrainedNode.constraints;
                    var absoluteBoundingBox = ((IAbsoluteBoundingBox)currentNode)
                        .absoluteBoundingBox;
                    var absoluteBoundBoxParent = ((IAbsoluteBoundingBox)parentNode)
                        .absoluteBoundingBox;

                    if (constraints.horizontal.Contains("RIGHT") || constraints.horizontal == "SCALE")
                    {
                        var endPosition1 = absoluteBoundingBox.X + absoluteBoundingBox.Width;
                        var endPosition2 = absoluteBoundBoxParent.X + absoluteBoundBoxParent.Width;
                        var value = Math.Max(endPosition1, endPosition2) - Math.Min(endPosition1, endPosition2);

                        var rightConstraint = nativeView.TrailingAnchor.ConstraintEqualTo(parentNativeView.TrailingAnchor, -value);
                        rightConstraint.Active = true;
                    }

                    if (constraints.horizontal.Contains ("LEFT"))
                    {
                        var value2 = absoluteBoundingBox.X - absoluteBoundBoxParent.X;
                        nativeView.LeadingAnchor.ConstraintEqualTo(parentNativeView.LeadingAnchor, value2)
                            .Active = true;
                    }

                    if (constraints.vertical.Contains("BOTTOM") || constraints.horizontal == "SCALE")
                    {
                        var endPosition1 = absoluteBoundingBox.Y + absoluteBoundingBox.Height;
                        var endPosition2 = absoluteBoundBoxParent.Y + absoluteBoundBoxParent.Height;
                        var value2 = Math.Max(endPosition1, endPosition2) - Math.Min(endPosition1, endPosition2);

                        var bottomConstraint = nativeView.BottomAnchor.ConstraintEqualTo(parentNativeView.BottomAnchor, -value2);
                        bottomConstraint.Active = true;
                    }

                    if (constraints.vertical.Contains ("TOP"))
                    {
                        var value = absoluteBoundingBox.Y - absoluteBoundBoxParent.Y;
                        nativeView.TopAnchor.ConstraintEqualTo(parentNativeView.TopAnchor, value)
                            .Active = true;
                    }

                    if (constraints.horizontal == "CENTER" || constraints.horizontal == "SCALE")
                    {
                        var delta = absoluteBoundingBox.X - absoluteBoundBoxParent.X - absoluteBoundBoxParent.Center.X;
                        nativeView.LeadingAnchor.ConstraintEqualTo(parentNativeView.CenterXAnchor, delta)
                            .Active = true;
                    }

                    if (constraints.vertical == "CENTER" || constraints.vertical == "SCALE")
                    {
                        var delta = absoluteBoundingBox.Y - absoluteBoundBoxParent.Y - absoluteBoundBoxParent.Center.Y;
                        //var delta = absoluteBoundBoxParent.Center.Substract(absoluteBoundingBox.Origin).Y;
                        var test = nativeView.TopAnchor.ConstraintEqualTo(parentNativeView.CenterYAnchor, delta);
                        test.Active = true;
                    }
                }
                return;
            }

            if (propertyName == PropertyNames.Frame)
            {
                if (currentNode is IAbsoluteBoundingBox absoluteBounding)
                {
                    if (!rendererService.HasConstraints(currentNode, converter))
                        return;

                    var nativeView = currentViewNode?.View?.NativeObject as AppKit.NSView;
                    if (rendererService.HasWidthConstraint(currentNode, converter))
                    {
                        var widthConstraint = nativeView.WidthAnchor.ConstraintEqualTo(Math.Max(absoluteBounding.absoluteBoundingBox.Width, 1));
                        if (rendererService.IsFlexibleHorizontal(currentViewNode, converter))
                            widthConstraint.Priority = (float)NSLayoutPriority.DefaultLow;
                        widthConstraint.Active = true;
                    }

                    if (rendererService.HasHeightConstraint(currentNode, converter))
                    {
                        var heightConstraint = nativeView.HeightAnchor.ConstraintEqualTo(Math.Max(absoluteBounding.absoluteBoundingBox.Height, 1));
                        if (rendererService.IsFlexibleVertical(currentViewNode, converter))
                            heightConstraint.Priority = (float)NSLayoutPriority.DefaultLow;

                        heightConstraint.Active = true;
                    }
                }
                return;
            }
        }
    }
}