using System;
using System.Text;
using AppKit;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class ListViewConverter : CustomViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			return currentNode.name == "webView" && currentNode is IFigmaDocumentContainer;
		}

		public override IViewWrapper ConvertTo (FigmaNode currentNode, ProcessedNode parent)
		{
			var textField = new NSGridView ();
			textField.Configure (currentNode);
			return new ViewWrapper (textField);
		}

		public override string ConvertToCode (FigmaNode currentNode, ProcessedNode parent)
		{
			StringBuilder builder = new StringBuilder ();
			var name = "listView";
			builder.AppendLine ($"var {name} = new {nameof (NSGridView)}();");
			builder.Configure (name, currentNode);
			return builder.ToString ();
		}
	}
}
