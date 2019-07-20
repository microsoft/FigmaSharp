using FigmaSharp.Views;
using NGraphics;

namespace FigmaSharp.Graphics.Mac
{
	public class EllipseView : ShapeView, IEllipse
	{
		public int StrokeThickness { get; set; } = 1;

		protected override void OnDraw(IImageCanvas canvas)
		{
			canvas.DrawEllipse(
				new Rect(0, 0, Width, Height),
				new Pen(
					Color.ToNGraphicColor(),
					StrokeThickness
				));
		}
	}
}
