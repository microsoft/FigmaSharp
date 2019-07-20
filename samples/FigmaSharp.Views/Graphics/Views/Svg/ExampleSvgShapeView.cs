using LiteForms.Graphics.Mac;

namespace BasicGraphics.Cocoa
{
	class ExampleSvgShapeView : SvgShapeView
	{
		public ExampleSvgShapeView(string file)
		{
			var svgData = FileHelper.GetFileDataFromBundle(file);
			Load(svgData);
		}

		protected override void OnSvgDraw(NGraphics.IImageCanvas canvas, NGraphics.Graphic graphic)
		{

		}
	}
}
