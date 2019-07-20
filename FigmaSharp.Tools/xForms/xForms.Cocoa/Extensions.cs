using System;
using AppKit;
using FigmaSharp.Models;

namespace FigmaSharp.Cocoa
{
	public static class Extensions
	{
		public static NSColor ToNSColor(this FigmaColor color)
		{
			return NSColor.FromRgba(color.r, color.g, color.b, color.a);
		}

		public static FigmaColor ToFigmaColor(this NSColor color)
		{
			return new FigmaColor() { a = (float)color.AlphaComponent, r = (float)color.RedComponent, g = (float)color.GreenComponent, b = (float)color.BlueComponent };
		}
	}
}
