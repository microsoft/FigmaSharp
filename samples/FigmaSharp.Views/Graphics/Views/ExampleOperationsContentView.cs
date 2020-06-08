using FigmaSharp;
using FigmaSharp.Views.Cocoa;

namespace BasicGraphics.Cocoa
{
	public class ExampleContentView : View
	{
		public ExampleContentView()
		{
			//BackgroundColor = Color //new Color(r: 0.08f, g: 0.08f, b: 0.08f);
			//BorderRadius = 10;
			//BorderColor = new Color(1, 1, 1, 0.08f);
			//BorderWidth = 1;
		}

		public override void OnChangeFrameSize(Size newSize)
		{
		
			base.OnChangeFrameSize(newSize);
		}
	}
}
