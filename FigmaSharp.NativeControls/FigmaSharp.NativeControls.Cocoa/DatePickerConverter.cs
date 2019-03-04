using System;
using System.Text;
using AppKit;

namespace FigmaSharp.NativeControls
{
	public class DatePickerConverter : CustomViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			return ContainsType (currentNode, "datePicker");
		}

		public override IViewWrapper ConvertTo (FigmaNode currentNode, ProcessedNode parent)
		{
			var view = new AppKit.NSDatePicker ();
			view.Configure (currentNode);

			var keyValues = GetKeyValues (currentNode);
			foreach (var key in keyValues) {
				if (key.Key == "type") {
					continue;
				}
				if (key.Key == "size") {
					view.ControlSize = ToEnum<NSControlSize> (key.Value);
				}
			}
			return new ViewWrapper (view);
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
