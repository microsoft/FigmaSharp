using System;
using FigmaSharp;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Forms;

namespace BasicRendering.Forms
{
    public class CustomLinkConverter : NodeConverter
	{
		public override bool CanConvert(FigmaNode currentNode) => currentNode.name.EndsWith("CustomLink", StringComparison.Ordinal);

		public override IView ConvertToView (FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
		{
			var button = new TransitionImageButton();
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
		public override string ConvertToCode(CodeNode currentNode, CodeNode parent, CodeRenderService rendererService) => string.Empty;

		public override Type GetControlType(FigmaNode currentNode) => typeof(Xamarin.Forms.View);
    }
}
