// Authors:
//   jmedrano <josmed@microsoft.com>
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
using AppKit;
using CoreAnimation;
using CoreGraphics;
using FigmaSharp.Views.Graphics;
using Foundation;

namespace FigmaSharp.Views.Cocoa.Graphics
{
    public static class PathExtensions
    {
        public static CAShapeLayer ToShape(this GPath element, Svg context = null)
        {
            var shape = new CAShapeLayer();
            foreach (var item in element.Paths)
                if (item is GPath gp)
                    shape.AddSublayer(gp.ToShape(context));
                else if (item is Path pa)
                    shape.AddSublayer(pa.ToShape(context));
                else if (item is CirclePath cir)
                    shape.AddSublayer(cir.ToShape(context));
                else if (item is RectanglePath rec)
                    shape.AddSublayer(rec.ToShape(context));
                else if (item is LinePath line)
                    shape.AddSublayer(line.ToShape());
                else if (item is TextPath text)
                    shape.AddSublayer(text.ToShape());
            return shape;
        }

        public static CALayer ToShape(this RectanglePath element, Svg context)
        {
            CALayer shape;

            var width = element.Width - (element.RX * 2);
            var height = element.Height - (element.RX * 2);

            if (XExtensions.TryGetLinearColor(element, context,out var gradient))
            {
                var gradientLayer = new CAGradientLayer();
                var colors = new CGColor[gradient.Stops.Length];
                for (int i = 0; i < gradient.Stops.Length; i++)
                {
                    var stop = gradient.Stops[i];
                    if (XExtensions.TryConvertToNSColor(stop.Color, out var col))
                        colors[i] = col.CGColor;
                }

                gradientLayer.StartPoint = new CGPoint(gradient.X1 / width, gradient.Y1 / height);
                gradientLayer.EndPoint = new CGPoint(gradient.X2 / width, gradient.Y2 / height);
                gradientLayer.Colors = colors;
                gradientLayer.Locations = new NSNumber[] { 0.0, 1.0 };
                shape = gradientLayer;
            }
            else { 
                shape = new CALayer();
                if (XExtensions.TryConvertToNSColor(element.Fill, out var fillColor))
                    shape.BackgroundColor = fillColor.CGColor;
            }

            shape.CornerRadius = element.RX;
            shape.BorderWidth = element.StrokeWidth;

            if (XExtensions.TryConvertToNSColor(element.Stroke, out var color))
                shape.BorderColor = color.CGColor;

            //shape.BorderColor = NSColor.Red.CGColor;

            shape.Bounds = new CGRect(0, 0, width, height);
            return shape;
        }

        public static CAShapeLayer ToShape(this LinePath element, Svg context = null)
        {
            var line = new CAShapeLayer();

            var bezierPath = new NSBezierPath();
            bezierPath.MoveTo(new CGPoint(element.X1, element.Y1));
            bezierPath.LineTo (new CGPoint(element.X2, element.Y2));
            line.Path = bezierPath.ToCGPath();

            line.LineWidth = element.StrokeWidth;

            if (XExtensions.TryConvertToNSColor(element.Stroke, out var color))
                line.BorderColor = color.CGColor;

            if (XExtensions.TryConvertToNSColor(element.Fill, out var fillColor))
                line.FillColor = fillColor.CGColor;

            line.Bounds = line.Path.BoundingBox;

            return line;
        }

        public static CATextLayer ToShape(this TextPath element, Svg context = null)
        {
            var text = new CATextLayer();
            text.FontSize = element.FontSize;
            return text;
        }

        public static CAShapeLayer ToShape(this Path element, Svg context = null)
        {
            var shape = new CAShapeLayer();
         
            if (!string.IsNullOrEmpty(element.d))
                shape.Path = PathBuilder.Build(element.d);

            if (XExtensions.TryConvertToNSColor(element.Stroke, out var color))
                shape.BorderColor = color.CGColor;

            if (XExtensions.TryConvertToNSColor(element.Fill, out var fillColor))
                shape.FillColor = fillColor.CGColor;

            shape.LineWidth = element.StrokeWidth;

            shape.Bounds = shape.Path.BoundingBox;

            return shape;
        }

        public static CAShapeLayer ToShape(this CirclePath element, Svg context = null)
        {
            var border = element.StrokeWidth / 2f;
            var rect = new CGRect(border, border, element.Radio - element.StrokeWidth, element.Radio-element.StrokeWidth);

            var shape = new CAShapeLayer();
            shape.MasksToBounds = true;
            var bezierPath = NSBezierPath.FromOvalInRect(rect);
            shape.Path = bezierPath.ToCGPath();

            if (XExtensions.TryConvertToNSColor(element.Stroke, out var color))
                shape.BorderColor = color.CGColor;

            if (XExtensions.TryConvertToNSColor(element.Fill, out var fillColor))
                shape.FillColor = fillColor.CGColor;

            shape.LineWidth = element.StrokeWidth;

            //var diameter = element.Radio * 2;
            shape.Frame = new CGRect (0, 0, element.Radio , element.Radio);

            return shape;
        }
    }
}
