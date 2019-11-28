using System;
using AppKit;
using CoreGraphics;
using FigmaSharp.Views;

namespace FigmaSharp
{
	public static class Extensions
	{
		public static CGSize ToCGSize (this Size size)
		{
			return new CGSize (size.Width, size.Height);
		}

		public static Size ToLiteSize (this CGSize size)
		{
			return new Size ((float)size.Width, (float)size.Height);
		}

		public static CGRect ToCGRect (this Rectangle rectangle)
		{
			return new CGRect (rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public static Rectangle ToRectangle (this CGRect rectangle)
		{
			return new Rectangle ((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
		}

		public static CGColor ToCGColor (this Color color)
		{
			if (color.R == 0 && color.G == 0 && color.R == 0 && color.A == 0)
				return NSColor.Clear.CGColor;
			return new CGColor (color.R, color.G, color.B, color.A);
		}

		public static Color ToColor (this CGColor color)
		{
			if (color.Components.Length == 4) {
				return new Color ((float)color.Components[0], (float)color.Components[1], (float)color.Components[2], (float)color.Components[3]);
			}
			return new Color ();
		}

		public static NSColor ToNSColor (this Color color)
		{
			if (color.R == 0 && color.G == 0 && color.R == 0 && color.A == 0)
				return NSColor.Clear;
			return NSColor.FromRgba (color.R, color.G, color.B, color.A);
		}

		public static Color ToColor (this NSColor color)
		{
			return new Color () { A = (float)color.AlphaComponent, R = (float)color.RedComponent, G = (float)color.GreenComponent, B = (float)color.BlueComponent };
		}

		public static LayoutOrientation ToOrientation (this NSUserInterfaceLayoutOrientation orientation)
		{
			if (orientation == NSUserInterfaceLayoutOrientation.Horizontal) {
				return LayoutOrientation.Horizontal;
			}
			return LayoutOrientation.Vertical;
		}

		public static NSUserInterfaceLayoutOrientation ToOrientation (this LayoutOrientation orientation)
		{
			if (orientation == LayoutOrientation.Horizontal) {
				return NSUserInterfaceLayoutOrientation.Horizontal;
			}
			return NSUserInterfaceLayoutOrientation.Vertical;
		}

		//TODO: we should move this to a shared place
		public static CGPath ToCGPath (this NSBezierPath path)
		{
			var numElements = path.ElementCount;
			if (numElements == 0) {
				return null;
			}

			CGPath result = new CGPath ();
			bool didClosePath = true;


			for (int i = 0; i < numElements; i++) {
				CGPoint[] points;
				var element = path.ElementAt (i, out points);
				if (element == NSBezierPathElement.MoveTo) {
					result.MoveToPoint (points[0].X, points[0].Y);
				} else if (element == NSBezierPathElement.LineTo) {
					result.AddLineToPoint (points[0].X, points[0].Y);
					didClosePath = false;

				} else if (element == NSBezierPathElement.CurveTo) {
					result.AddCurveToPoint (points[0].X, points[0].Y,
											points[1].X, points[1].Y,
											points[2].X, points[2].Y);
					didClosePath = false;
				} else if (element == NSBezierPathElement.ClosePath) {
					result.CloseSubpath ();
				}
			}

			// Be sure the path is closed or Quartz may not do valid hit detection.
			if (!didClosePath) {
				result.CloseSubpath ();
			}
			return result;
		}
	}
}
