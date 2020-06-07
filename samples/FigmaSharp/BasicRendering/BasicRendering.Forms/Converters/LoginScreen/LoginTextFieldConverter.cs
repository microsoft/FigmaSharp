using System;
using System.Linq;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Forms;

namespace BasicRendering.Forms
{
    public class LoginTextFieldConverter : NodeConverter
	{
		public override bool CanConvert(FigmaNode currentNode) =>
			currentNode.name.In ("EmailTextField", "PasswordTextField");

		public override IView ConvertToView (FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
		{
			var entry = new Xamarin.Forms.Entry();

			if (currentNode is IFigmaNodeContainer nodeContainer)
			{
				var text = nodeContainer.children
					.OfType<FigmaText>()
					.FirstOrDefault(s => s.name == "placeholderstring");
				if (text != null) {
					entry.Placeholder = text.characters;
				}
			}

			var view = new View(entry);
			return view;
		}
		public override bool ScanChildren(FigmaNode currentNode) => false;
		public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService) => string.Empty;

		public override Type GetControlType(FigmaNode currentNode) => typeof(Xamarin.Forms.View);
	}
}
