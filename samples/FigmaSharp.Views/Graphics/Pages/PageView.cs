using System;
using FigmaSharp;
using FigmaSharp.Views.Cocoa;
//using LiteForms;
//using LiteForms.Cocoa;
//using LiteForms.Graphics.Mac;

namespace BasicGraphics.Cocoa
{
	public class PageView : View, IPage
	{
		protected OptionsPanelGradienContentView actionContainerView;
	
		public PageView (OptionsPanelGradienContentView actionContainerView)
		{
			this.actionContainerView = actionContainerView;

		
		}


		public virtual void OnWindowResize(object s, EventArgs e)
		{
			actionContainerView.CenterView();
		}

		public override void OnChangeFrameSize(Size newSize)
		{

		}

		public virtual void OnShown()
		{

		}

		public virtual void OnHide()
		{

		}
	}
}
