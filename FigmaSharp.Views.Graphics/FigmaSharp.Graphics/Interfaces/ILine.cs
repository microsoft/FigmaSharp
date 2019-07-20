namespace FigmaSharp.Graphics
{
	public interface ILine : IShapeView
	{
		Point Point1 { get; set; }
		Point Point2 { get; set; }
		int StrokeThickness { get; set; }
	}
}
