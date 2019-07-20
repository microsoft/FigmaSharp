using NGraphics;
using System;
using System.Collections.Generic;

namespace FigmaSharp.Graphics.Mac
{
	public class SvgShapeView : ShapeView, ISvgShapeView
	{
		Graphic g;
		string data;

		List<Element> originals = new List<Element>();

		public void Load(string data)
		{
			this.data = data;

			var reader = new SvgReader(data);
			g = reader.Graphic;

			originals.Clear();
			foreach (var item in g.Children)
				originals.Add(item);

			RefreshDraw();

			OnChangeFrameSize(Size);
		}

		public SvgShapeView () : base (new AppKit.NSImageView ())
		{

		}

		public override void OnApplyTransform()
		{
			RefreshDraw();
		}

		public override void OnChangeFrameSize(FigmaSharp.Views.Size newSize)
		{
			if (g == null || newSize.Width == 0 || newSize.Height == 0)
			{
				return;
			}

			var deltaX = newSize.Width / g.SampleableBox.Width;
			var deltaY = newSize.Height / g.SampleableBox.Height;

			g.Children.Clear();
			foreach (var item in originals)
			{
				var cloned = item;
				if (cloned is NGraphics.Path path)
				{
					foreach (var op in path.Operations)
					{
						if (op is LineTo lineTo)
						{
							lineTo.Point.X *= deltaX;
							lineTo.Point.Y *= deltaY;
						}
						else if (op is MoveTo moveTo)
						{
							moveTo.Point.X *= deltaX;
							moveTo.Point.Y *= deltaY;
						}
						else if(op is CurveTo curveTo)
						{
							curveTo.Control2.X *= deltaX;
							curveTo.Control2.Y *= deltaY;

							curveTo.Control1.X *= deltaX;
							curveTo.Control1.Y *= deltaY;

							curveTo.Point.X *= deltaX;
							curveTo.Point.Y *= deltaY;
						} else
						{
							Console.WriteLine("");
						}
					}
				}
				g.Children.Add(cloned);
			}
			base.OnChangeFrameSize(newSize);
		}

		protected virtual void OnSvgDraw (IImageCanvas canvas, Graphic graphic)
		{
			
		}

		protected override void OnDraw(IImageCanvas canvas)
		{
			OnSvgDraw(canvas, g);
			g?.Draw(canvas);
		}
	}
}
