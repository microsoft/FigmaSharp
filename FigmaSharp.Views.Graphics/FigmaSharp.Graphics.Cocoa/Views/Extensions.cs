namespace FigmaSharp.Views
{
	public static class Extensions
	{
		public static NGraphics.Color ToNGraphicColor(this FigmaSharp.Views.Color sender)
		{
			return new NGraphics.Color(sender.R, sender.G, sender.B, sender.A);
		}

		public static FigmaSharp.Views.Color ToLFormsColor(this NGraphics.Color sender)
		{
			return new FigmaSharp.Views.Color(sender.R, sender.G, sender.B, sender.A);
		}

		public static NGraphics.Size ToNGraphicSize(this FigmaSharp.Views.Size sender)
		{
			return new NGraphics.Size(sender.Width, sender.Height);
		}

		public static FigmaSharp.Views.Size ToLFormsSize(this NGraphics.Size sender)
		{
			return new FigmaSharp.Views.Size((float)sender.Width, (float)sender.Height);
		}

		public static NGraphics.Point ToNGraphicPoint(this FigmaSharp.Views.Point sender)
		{
			return new NGraphics.Point(sender.X, sender.Y);
		}

		public static FigmaSharp.Views.Point ToLFormsPoint(this NGraphics.Point sender)
		{
			return new FigmaSharp.Views.Point((float)sender.X, (float)sender.Y);
		}


		public static CoreGraphics.CGSize ToCGSize(this NGraphics.Size sender)
		{
			return new CoreGraphics.CGSize((float)sender.Width, (float)sender.Height);
		}

		public static CoreGraphics.CGPoint ToCGPoint(this NGraphics.Point sender)
		{
			return new CoreGraphics.CGPoint((float)sender.X, (float)sender.Y);
		}
	}
}
