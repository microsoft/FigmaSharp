using System;
using System.Collections.Generic;
using AppKit;
using CoreAnimation;
using CoreGraphics;

namespace FigmaSharp.Views.Cocoa.Views
{
    public class GradientLayer : CAGradientLayer
    {
        public CAShapeLayer Gradient
        {
            get => (CAShapeLayer)Mask;
            set
            {
                Mask = value;
            }
        }

        public CGPath Path
        {
            get => Gradient.Path;
            set
            {
                Gradient.Path = value;
            }
        }

        public GradientLayer(CGRect rect)
        {
            var gradientMask = new CAShapeLayer();
            gradientMask.FillColor = NSColor.Clear.CGColor;
            gradientMask.Frame = rect;
            Frame = rect;
            Mask = gradientMask;
        }
    }
}
