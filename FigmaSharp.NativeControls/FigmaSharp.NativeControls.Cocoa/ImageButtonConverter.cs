using System;
using System.Text;
using AppKit;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class ImageButtonConverter : CustomViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			return currentNode.name == "imageButton" && currentNode is IFigmaDocumentContainer;
		}

		public override IViewWrapper ConvertTo (FigmaNode currentNode, ProcessedNode parent)
		{
			var textField = new AppKit.NSButton ();
			textField.Configure (currentNode);
			return new ViewWrapper (textField);
		}

		public override string ConvertToCode (FigmaNode currentNode, ProcessedNode parent)
		{
			StringBuilder builder = new StringBuilder ();
			var name = "imageButtonView";
			builder.AppendLine ($"var {name} = new {nameof (NSButton)}();");
			builder.Configure (name, currentNode);
			return builder.ToString ();
		}
	}
}
