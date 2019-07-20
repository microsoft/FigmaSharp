using FigmaSharp;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Forms;
using System.Linq;

namespace BasicRendering.Forms
{
	public class LoginTextFieldConverter : FigmaViewConverter
	{
		public override bool CanConvert(FigmaNode currentNode) =>
			currentNode.name.In ("EmailTextField", "PasswordTextField");

		public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
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
		public override string ConvertToCode(FigmaNode currentNode, FigmaCodeRendererService rendererService) => string.Empty;
	}
}
