using FigmaSharp;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace BasicRendering.Forms
{
	public class NavigationMenuButtonConverter : FigmaViewConverter
	{
		public override bool CanConvert(FigmaNode currentNode) =>
		currentNode.name.EndsWith ("NavigationMenuButton", System.StringComparison.Ordinal);

		public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var button = new FigmaSharp.Forms.FigmaTransitionButton();
			button.Text = ":)";
			if (currentNode is FigmaFrameEntity figmaFrameEntity)
			{
				button.TransitionNodeID = figmaFrameEntity.transitionNodeID;
				button.TransitionDuration = figmaFrameEntity.transitionDuration;
				button.TransitionEasing = figmaFrameEntity.transitionEasing;
			}
			return button;
		}
		public override bool ScanChildren(FigmaNode currentNode) => false;
		public override string ConvertToCode(FigmaNode currentNode, FigmaCodeRendererService rendererService) => string.Empty;
	}
}
