using LiteForms;
using LiteForms.Cocoa;

namespace BasicGraphics.Cocoa
{
	public class Page2 : PageView
	{
		const string Start = "star.svg";
		const string Arrow = "arrow.svg";
		const string Symbol = "symbol.svg";
		const string Machine = "machine.svg";
		
		ExampleSvgShapeView svgShapeView;
		ComboBox combo;

		public Page2(OptionsPanelGradienContentView actionContainerView) : base (actionContainerView)
		{
			combo = new ComboBox() { Allocation = new Rectangle (10,10,100,20) };

			combo.AddItem(Start);
			combo.AddItem(Arrow);
			combo.AddItem(Symbol);
			combo.AddItem(Machine);
			combo.SelectionIndexChanged += (s, e) => {
				ShowSvg(combo.SelectedItem);
			};
		}

		void ShowSvg (string name)
		{
			svgShapeView = new ExampleSvgShapeView(name);
			svgShapeView.Allocation = new Rectangle(40, 40, 300, 300);

			actionContainerView.SetView(svgShapeView);
		}

		public override void OnShown()
		{
			actionContainerView.AddChild(combo);
			ShowSvg(Start);
		}

		public override void OnHide()
		{
			actionContainerView.RemoveChild(combo);
			actionContainerView.RemoveView();
		}
	}
}
