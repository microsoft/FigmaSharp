﻿/* 
 * ViewPropertyConfigure.cs 
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
using AppKit;
using FigmaSharp.Models;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace FigmaSharp.Cocoa.PropertyConfigure
{
    public class ViewPropertyConfigure : ViewPropertyConfigureBase
    {
        public override void Configure(string propertyName, IView view, FigmaNode currentNode, IView parent, FigmaNode parentNode, ViewRenderService rendererService)
        {
            if (propertyName == PropertyNames.AddChild)
            {
                if (parent?.NativeObject is AppKit.NSStackView stackView)
                    stackView.AddArrangedSubview(view.NativeObject as NSView);
                else
                    parent?.AddChild(view);
                return;
            }
            if (propertyName == PropertyNames.Constraints)
            {
                if (currentNode is IConstraints constrainedNode && view.NativeObject is AppKit.NSView nativeView && parent.NativeObject is AppKit.NSView parentNativeView)
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

                        var rightConstraint = nativeView.RightAnchor.ConstraintEqualToAnchor(parentNativeView.RightAnchor, -value);
                        rightConstraint.Active = true;
                    }

                    if (constraints.horizontal.Contains ("LEFT"))
                    {
                        var value2 = absoluteBoundingBox.X - absoluteBoundBoxParent.X;
                        nativeView.LeftAnchor.ConstraintEqualToAnchor(parentNativeView.LeftAnchor, value2)
                            .Active = true;
                    }

                    if (constraints.vertical.Contains("BOTTOM") || constraints.horizontal == "SCALE")
                    {
                        var endPosition1 = absoluteBoundingBox.Y + absoluteBoundingBox.Height;
                        var endPosition2 = absoluteBoundBoxParent.Y + absoluteBoundBoxParent.Height;
                        var value2 = Math.Max(endPosition1, endPosition2) - Math.Min(endPosition1, endPosition2);

                        var bottomConstraint = nativeView.BottomAnchor.ConstraintEqualToAnchor(parentNativeView.BottomAnchor, -value2);
                        bottomConstraint.Active = true;
                    }

                    if (constraints.vertical.Contains ("TOP"))
                    {
                        var value = absoluteBoundingBox.Y - absoluteBoundBoxParent.Y;
                        nativeView.TopAnchor.ConstraintEqualToAnchor(parentNativeView.TopAnchor, value)
                            .Active = true;
                    }

                    if (constraints.horizontal == "CENTER" || constraints.horizontal == "SCALE")
                    {
                        var delta = absoluteBoundingBox.X - absoluteBoundBoxParent.X - absoluteBoundBoxParent.Center.X;
                        nativeView.LeftAnchor.ConstraintEqualToAnchor(parentNativeView.CenterXAnchor, delta)
                            .Active = true;
                    }

                    if (constraints.vertical == "CENTER" || constraints.vertical == "SCALE")
                    {
                        var delta = absoluteBoundingBox.Y - absoluteBoundBoxParent.Y - absoluteBoundBoxParent.Center.Y;
                        //var delta = absoluteBoundBoxParent.Center.Substract(absoluteBoundingBox.Origin).Y;
                        var test = nativeView.TopAnchor.ConstraintEqualToAnchor(parentNativeView.CenterYAnchor, delta);
                        test.Active = true;
                    }
                }
                return;
            }

            if (propertyName == PropertyNames.Frame)
            {
                if (currentNode is IAbsoluteBoundingBox absoluteBounding)
                {
                    var nativeView = view.NativeObject as AppKit.NSView;

                    var parentNativeView = parent.NativeObject as AppKit.NSView;
                    var widthConstraint = nativeView.WidthAnchor.ConstraintEqualToConstant(Math.Max(absoluteBounding.absoluteBoundingBox.Width, 1));

                    var constrainedNode = currentNode as IConstraints;
                    if (constrainedNode != null && constrainedNode.constraints.IsFlexibleHorizontal)
                        widthConstraint.Priority = (float)NSLayoutPriority.DefaultLow;

                    widthConstraint.Active = true;

                    var heightConstraint = nativeView.HeightAnchor.ConstraintEqualToConstant(Math.Max(absoluteBounding.absoluteBoundingBox.Height, 1));

                    if (constrainedNode != null && constrainedNode.constraints.IsFlexibleVertical)
                        heightConstraint.Priority = (float)NSLayoutPriority.DefaultLow;

                    heightConstraint.Active = true;

                }
                return;
            }
        }
    }
}