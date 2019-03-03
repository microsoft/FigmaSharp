using System;
using System.Text;
using AppKit;

namespace FigmaSharp.NativeControls
{
	public class DatePickerConverter : CustomViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			return currentNode.name == "datePicker" && currentNode is IFigmaDocumentContainer;
		}

		public override IViewWrapper ConvertTo (FigmaNode currentNode, ProcessedNode parent)
		{
			var textField = new AppKit.NSDatePicker ();
			textField.Configure (currentNode);
			return new ViewWrapper (textField);
		}

		public override string ConvertToCode (FigmaNode currentNode, ProcessedNode parent)
		{
			StringBuilder builder = new StringBuilder ();
			var name = "datePickerView";
			builder.AppendLine ($"var {name} = new {nameof (NSDatePicker)}();");
			builder.Configure (name, currentNode);
			return builder.ToString ();
		}
	}
}
