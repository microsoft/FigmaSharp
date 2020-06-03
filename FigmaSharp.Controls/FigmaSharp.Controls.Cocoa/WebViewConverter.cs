using System;
using System.Text;
using AppKit;
using WebKit;

namespace FigmaSharp.NativeControls
{
	public class WebViewConverter : CustomViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			return ContainsType (currentNode, "webView");
		}

		public override IViewWrapper ConvertTo (FigmaNode currentNode, ProcessedNode parent)
		{
			var view = new WebView ();
			view.Configure (currentNode);

			var keyValues = GetKeyValues (currentNode);
			foreach (var key in keyValues) {
				if (key.Key == "type") {
					continue;
				}
			}
			return new ViewWrapper (view);
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
