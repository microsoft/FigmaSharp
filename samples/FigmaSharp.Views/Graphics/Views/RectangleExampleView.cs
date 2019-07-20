using LiteForms;
using LiteForms.Cocoa;

namespace BasicGraphics.Cocoa
{
	public class RectangleExampleView : ExtendedView
	{
		public RectangleExampleView()
		{
			Size = new Size(120, 120);
			BorderRadius = 20;
			BackgroundColor = Color.White;
		}
	}
}
