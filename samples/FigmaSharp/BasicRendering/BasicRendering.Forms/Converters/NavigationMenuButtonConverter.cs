using System;
using FigmaSharp;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Forms;

namespace BasicRendering.Forms
{
	public class NavigationMenuButtonConverter : NodeConverter
	{
		public override bool CanConvert(FigmaNode currentNode) =>
		currentNode.name.EndsWith ("NavigationMenuButton", System.StringComparison.Ordinal);

		public override IView ConvertToView (FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
		{
			var button = new TransitionButton();
			button.Text = ":)";
			if (currentNode is FigmaFrame figmaFrameEntity)
			{
				button.TransitionNodeID = figmaFrameEntity.transitionNodeID;
				button.TransitionDuration = figmaFrameEntity.transitionDuration;
				button.TransitionEasing = figmaFrameEntity.transitionEasing;
			}
			return button;
		}
		public override bool ScanChildren(FigmaNode currentNode) => false;
		public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService) => string.Empty;

		public override Type GetControlType(FigmaNode currentNode) => typeof(Xamarin.Forms.View);
    }
}
