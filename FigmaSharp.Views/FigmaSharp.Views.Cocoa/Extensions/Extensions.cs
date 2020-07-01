using System;
using AppKit;
using CoreGraphics;
using FigmaSharp.Views;

namespace FigmaSharp
{
    public static class ConversionExtensions
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

		public static CGColor ToCGColor (this Color color, float opacity = -1)
		{
			return color.ToNSColor(opacity).CGColor;
		}

		public static Color ToColor (this CGColor color)
		{
			if (color.Components.Length == 4) {
				return new Color ((float)color.Components[0], (float)color.Components[1], (float)color.Components[2], (float)color.Components[3]);
			}
			return new Color ();
		}

		public static NSColor ToNSColor (this Color color, float opacity = -1)
		{
			if (color.R == 0 && color.G == 0 && color.B == 0 && color.A == 0)
				return NSColor.Clear;
			var alpha = (opacity == -1 ? color.A : opacity);
			return NSColor.FromDeviceRgba ((nfloat)color.R, (nfloat)color.G, (nfloat)color.B, (nfloat)alpha);
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

	}
}
