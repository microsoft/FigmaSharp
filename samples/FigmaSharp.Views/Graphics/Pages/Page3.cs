using LiteForms;
using LiteForms.Cocoa;

namespace BasicGraphics.Cocoa
{
	public class Page3 : PageView
	{
		LiteForms.Graphics.Mac.ShapeView shapeView;
		ComboBox combo;

		public Page3 (OptionsPanelGradienContentView actionContainerView) : base(actionContainerView)
		{
			combo = new ComboBox() { Allocation = new Rectangle(10, 10, 100, 20) };
			combo.AddItem("Line");
			combo.AddItem("Circle");
			combo.SelectionIndexChanged += (s,e) => SelectItem (combo.SelectedIndex);
		}

		public override void OnShown()
		{
			actionContainerView.AddChild(combo);
			SelectItem(0);
		}

		public override void OnHide()
		{
			actionContainerView.RemoveChild(combo);
			actionContainerView.RemoveView();
		}

		private void SelectItem (int index)
		{
			switch (index)
			{
				case 0:
					var lineView = new LiteForms.Graphics.Mac.LineView();
					lineView.Point1 = new Point(0, 0);
					lineView.Point2 = new Point(40, 40);
					lineView.Color = Color.Green;
					lineView.StrokeThickness = 3;

					ShowShape(lineView);

					break;


				case 1:
					var ellipseView = new LiteForms.Graphics.Mac.EllipseView();
					ellipseView.BackgroundColor = Color.Blue;
					ellipseView.BorderColor = Color.Red;
					ShowShape(ellipseView);
					break;
			}
		}

		void ShowShape(LiteForms.Graphics.Mac.ShapeView shape)
		{
			shapeView = shape;
			shapeView.Allocation = new Rectangle(40, 40, 300, 300);
			actionContainerView.SetView(shapeView);
		}

		public override void OnChangeFrameSize(Size newSize)
		{

		}
	}
}
