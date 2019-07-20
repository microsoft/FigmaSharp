using System;
using FigmaSharp.Models;
using Xamarin.Forms;

namespace FigmaSharp.Forms
{
	public static class Extensions
	{
		public static Color ToColor(this FigmaColor color)
		{
			return new Color(color.r, color.g, color.b, color.a);
		}

		public static FigmaColor ToFigmaColor(this Color color)
		{
			return new FigmaColor() { a = (float)color.A, r = (float)color.R, g = (float)color.G, b = (float)color.B };
		}
	}
}
