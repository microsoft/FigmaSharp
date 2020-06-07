using FigmaSharp;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Forms;
using System;
using System.Linq;

namespace BasicRendering.Forms
{
	public class CustomButtonConverter : NodeConverter
	{
		public override bool CanConvert(FigmaNode currentNode) =>
			currentNode.name.EndsWith ("CustomButton", System.StringComparison.Ordinal);

        public override IView ConvertToView (FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
		{
			Button button;
			if (currentNode is FigmaFrame figmaFrameEntity && !string.IsNullOrEmpty (figmaFrameEntity.transitionNodeID))  {
				button = new TransitionButton() {
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
		public override string ConvertToCode(CodeNode currentNode, CodeNode parent, CodeRenderService rendererService) => string.Empty;

		public override Type GetControlType(FigmaNode currentNode) => typeof(Xamarin.Forms.View);
	}
}
