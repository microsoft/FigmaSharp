using FigmaSharp.Views;

namespace FigmaSharp.Graphics
{
	public interface IShapeView : IView
	{
		Color Color { get; set; }
		Color BorderColor { get; set; }
	}
}
