using System;
using FigmaSharp;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace BasicRendering.Forms
{
	public class CustomLinkConverter : FigmaViewConverter
	{
		public override bool CanConvert(FigmaNode currentNode) => currentNode.name.EndsWith("CustomLink", StringComparison.Ordinal);

		public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var button = new FigmaSharp.Forms.FigmaTransitionImageButton();
			if (currentNode is FigmaText figmaFrameEntity)
			{
				button.TransitionNodeID = figmaFrameEntity.transitionNodeID;
				button.TransitionDuration = figmaFrameEntity.transitionDuration;
				button.Text = figmaFrameEntity.characters;

			}
			button.TextHorizontalAlignment = TextAlignment.Start;
			button.TextColor = Color.White;
			button.BorderWidth = 0;
			return button;
		}
		public override bool ScanChildren(FigmaNode currentNode) => false;
		public override string ConvertToCode(FigmaNode currentNode, FigmaCodeRendererService rendererService) => string.Empty;
	}
}
