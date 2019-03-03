using System;
using System.Text;
using AppKit;
using WebKit;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class WebViewConverter : CustomViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			return currentNode.name == "webView" && currentNode is IFigmaDocumentContainer;
		}

		public override IViewWrapper ConvertTo (FigmaNode currentNode, ProcessedNode parent)
		{
			var textField = new WebView ();
			textField.Configure (currentNode);
			return new ViewWrapper (textField);
		}

		public override string ConvertToCode (FigmaNode currentNode, ProcessedNode parent)
		{
			StringBuilder builder = new StringBuilder ();
			var name = "webView";
			builder.AppendLine ($"var {name} = new {nameof (WebKit)}.{nameof (WebView)}();");
			builder.Configure (name, currentNode);
			return builder.ToString ();
		}
	}
}
