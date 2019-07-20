using FigmaSharp.Views;
using NGraphics;

namespace FigmaSharp.Graphics.Mac
{
	public class LineView : ShapeView, ILine
	{
		public int StrokeThickness { get; set; } = 1;

		readonly FigmaSharp.Views.Point point1 = new Point ();
		public FigmaSharp.Views.Point Point1 {
			get => point1;
			set
			{
				point1.X = value.X;
				point1.Y = value.Y;
			}
		}
		readonly FigmaSharp.Views.Point point2 = new Point();
		public FigmaSharp.Views.Point Point2
		{
			get => point2;
			set
			{
				point2.X = value.X;
				point2.Y = value.Y;
			}
		}

		protected override void OnDraw(IImageCanvas canvas)
		{
			canvas.DrawLine(
				point1.ToNGraphicPoint(),
				point2.ToNGraphicPoint(),
				new Pen(
						Color.ToNGraphicColor(),
						StrokeThickness
				));
		}
	}
}
