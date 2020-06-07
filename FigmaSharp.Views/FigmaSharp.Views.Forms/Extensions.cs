using System;
namespace FigmaSharp.Views.Forms
{
	public static class Extensions
	{
		public static Color ToLiteColor(this Xamarin.Forms.Color color)
		{
			return new Color((float) color.R, (float)color.G, (float)color.B, (float)color.A);
		}

		public static Xamarin.Forms.Color ToFormsColor(this Color color)
		{
			return new Xamarin.Forms.Color (color.R, color.G, color.B, color.A);
		}

		public static Xamarin.Forms.Rectangle ToFormsRectangle(this Rectangle rectangle)
		{
			return new Xamarin.Forms.Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public static Rectangle ToLiteRectangle(this Xamarin.Forms.Rectangle rectangle)
		{
			return new Rectangle((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
		}
	}
}
