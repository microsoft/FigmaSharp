using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using System;
using AppKit;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Cocoa
{
    public class FigmaViewPropertySetter : ViewPropertyNodeConfigureBase
    {
        public override void Configure(string propertyName, IView view, FigmaNode currentNode, IView parent, FigmaNode parentNode, FigmaRendererService rendererService)
        {
            if (propertyName == PropertyNames.AddChild)
            {
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

                    //var parentNativeView = parent.NativeObject as AppKit.NSView;
                    var widthConstraint = nativeView.WidthAnchor.ConstraintEqualToConstant(Math.Max(absoluteBounding.absoluteBoundingBox.Width, 1));
                    widthConstraint.Active = true;

                    var constrainedNode = currentNode as IConstraints;
                    if (constrainedNode != null && constrainedNode.constraints.IsFlexibleHorizontal) 
                        widthConstraint.Priority = (float)NSLayoutPriority.DefaultLow;

                        var heightConstraint = nativeView.HeightAnchor.ConstraintEqualToConstant(Math.Max(absoluteBounding.absoluteBoundingBox.Height, 1));
                        heightConstraint.Active = true;

                        if (constrainedNode != null && constrainedNode.constraints.IsFlexibleVertical)
                            heightConstraint.Priority = (float)NSLayoutPriority.DefaultLow;

                }
                return;
            }
        }
    }
}