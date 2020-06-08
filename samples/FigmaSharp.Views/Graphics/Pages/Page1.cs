using System;
using System.Threading;
using System.Threading.Tasks;
using AppKit;
//using LiteForms;
//using LiteForms.Cocoa;
//using LiteForms.Graphics.Mac;

namespace BasicGraphics.Cocoa
{
	public class Page1 : PageView
	{
		RectangleExampleView rectangleExampleView;
	
		public Page1 (OptionsPanelGradienContentView actionContainerView) : base (actionContainerView)
		{
			rectangleExampleView = new RectangleExampleView();
		}

		public override void OnShown()
		{
			actionContainerView.SetView(rectangleExampleView);
			
		}

		public override void OnHide()
		{
			actionContainerView.RemoveView();
		}
	}
}
