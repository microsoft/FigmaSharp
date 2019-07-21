using AppKit;
using CoreGraphics;

namespace LiteForms.Cocoa
{
	public static class Extensions
	{
		public static CGRect ToCGRect(this Rectangle rectangle)
		{
			return new CGRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public static Rectangle ToRectangle (this CGRect rectangle)
		{
			return new Rectangle((float) rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
		}

		public static NSColor ToNSColor(this xColor color)
		{
			return NSColor.FromRgba(color.r, color.g, color.b, color.a);
		}

		public static xColor ToxColor(this NSColor color)
		{
			return new xColor() { a = (float)color.AlphaComponent, r = (float)color.RedComponent, g = (float)color.GreenComponent, b = (float)color.BlueComponent };
		}

		public static LayoutOrientation ToOrientation(this NSUserInterfaceLayoutOrientation orientation)
		{
			if (orientation == NSUserInterfaceLayoutOrientation.Horizontal)
			{
				return LayoutOrientation.Horizontal;
			}
			return LayoutOrientation.Vertical;
		}

		public static NSUserInterfaceLayoutOrientation  ToOrientation(this LayoutOrientation orientation)
		{
			if (orientation == LayoutOrientation .Horizontal)
			{
				return NSUserInterfaceLayoutOrientation.Horizontal;
			}
			return NSUserInterfaceLayoutOrientation.Vertical;
		}
	}
}
