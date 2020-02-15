using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using System;
using AppKit;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Cocoa
{
    public class FigmaViewPropertySetter : FigmaViewPropertySetterBase
    {
        public override void Configure(string propertyName, IView view, FigmaNode currentNode, IView parent, FigmaNode parentNode, FigmaRendererService rendererService)
        {
            if (propertyName == CodeProperties.AddChild)
            {
                parent?.AddChild(view);
                return;
            }
            if (propertyName == CodeProperties.Constraints)
            {
                if (rendererService.IsFirstNode (currentNode))
                {
                    return;
                }

                if (currentNode is IConstraints constrainedNode && view.NativeObject is AppKit.NSView nativeView && parent.NativeObject is AppKit.NSView parentNativeView)
                {
                    var constraints = constrainedNode.constraints;
                    var absoluteBoundingBox = ((IAbsoluteBoundingBox)currentNode)
                        .absoluteBoundingBox;
                    var absoluteBoundBoxParent = ((IAbsoluteBoundingBox)parentNode)
                        .absoluteBoundingBox;

                    nativeView.TranslatesAutoresizingMaskIntoConstraints = false;
                    parentNativeView.TranslatesAutoresizingMaskIntoConstraints = false;

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

            if (propertyName == CodeProperties.Frame)
            {
                if (currentNode is IAbsoluteBoundingBox absoluteBounding)
                {
                    var nativeView = view.NativeObject as AppKit.NSView;

                    //float y, x;
                    //if (parentNode is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
                    //{
                    //    x = Math.Max(absoluteBounding.absoluteBoundingBox.X - parentAbsoluteBoundingBox.absoluteBoundingBox.X, 0);

                    //    if (AppContext.Current.IsVerticalAxisFlipped)
                    //    {
                    //        var parentY = parentAbsoluteBoundingBox.absoluteBoundingBox.Y + parentAbsoluteBoundingBox.absoluteBoundingBox.Height;
                    //        var actualY = absoluteBounding.absoluteBoundingBox.Y + absoluteBounding.absoluteBoundingBox.Height;
                    //        y = parentY - actualY;
                    //    }
                    //    else
                    //    {
                    //        y = absoluteBounding.absoluteBoundingBox.Y - parentAbsoluteBoundingBox.absoluteBoundingBox.Y;
                    //    }

                    //    ((View)view).SetPosition(x, y);
                    //}

                    //var parentNativeView = parent.NativeObject as AppKit.NSView;
                    var widthConstraint = nativeView.WidthAnchor.ConstraintEqualToConstant(Math.Max(absoluteBounding.absoluteBoundingBox.Width, 1));
                    widthConstraint.Active = true;
                    var heightConstraint = nativeView.HeightAnchor.ConstraintEqualToConstant(Math.Max(absoluteBounding.absoluteBoundingBox.Height, 1));
                    heightConstraint.Active = true;
                    //nativeView.AddConstraints(new NSLayoutConstraint[] { widthConstraint , heightConstraint });

                    if (currentNode is IConstraints constrainedNode)
                    {
                        if (constrainedNode.constraints.IsFlexibleHorizontal)
                        {
                            widthConstraint.Priority = (float)NSLayoutPriority.DefaultLow;
                        }
                        if (constrainedNode.constraints.IsFlexibleVertical)
                        {
                            heightConstraint.Priority = (float)NSLayoutPriority.DefaultLow;
                        }
                    }
                }

                return;
            }
        }
    }
}