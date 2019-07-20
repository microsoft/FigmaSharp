using FigmaSharp;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Forms;
using System.Linq;

namespace BasicRendering.Forms
{
	public class CustomButtonConverter : FigmaViewConverter
	{
		public override bool CanConvert(FigmaNode currentNode) =>
			currentNode.name.EndsWith ("CustomButton", System.StringComparison.Ordinal);

        public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			Button button;
			if (currentNode is FigmaFrameEntity figmaFrameEntity && !string.IsNullOrEmpty (figmaFrameEntity.transitionNodeID))  {
				button = new FigmaSharp.Forms.FigmaTransitionButton() {
					TransitionNodeID = figmaFrameEntity.transitionNodeID,
					TransitionDuration = figmaFrameEntity.transitionDuration,
					TransitionEasing = figmaFrameEntity.transitionEasing,
				};
			} else {
				button = new Button();
			}

			if (currentNode is IFigmaNodeContainer nodeContainer)
			{
				var text = nodeContainer.children
					.OfType<FigmaText>()
					.FirstOrDefault ();

				if (text != null)
					button.Text = text.characters;

				var backgroundColor = nodeContainer.children.OfType<RectangleVector>().FirstOrDefault();
				if (backgroundColor != null && backgroundColor.HasFills)
					button.BackgroundColor = backgroundColor.fills[0].color;
			}

			button.TextColor = Color.White;
			return button;
		}
		public override bool ScanChildren(FigmaNode currentNode) => false;
		public override string ConvertToCode(FigmaNode currentNode, FigmaCodeRendererService rendererService) => string.Empty;
	}
}
