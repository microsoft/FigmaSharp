using System.Text;
using AppKit;

namespace FigmaSharp.NativeControls
{
	public class CheckBoxConverter : CustomViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			return ContainsType (currentNode, "checkbox");
		}

		public override IViewWrapper ConvertTo (FigmaNode currentNode, ProcessedNode parent)
		{
			var view = new NSButton ();
			view.SetButtonType (NSButtonType.Switch);
			view.Title = "";
			view.Configure (currentNode);

			var keyValues = GetKeyValues (currentNode);
			foreach (var key in keyValues) {
				if (key.Key == "type") {
					continue;
				}
				if (key.Key == "value") {
					view.State = key.Value == "true" ? NSCellStateValue.On : NSCellStateValue.Off;
				} else if (key.Key == "size") {
					view.ControlSize = ToEnum<NSControlSize> (key.Value);
				}
			}
			return new ViewWrapper (view);
		}

		public override string ConvertToCode (FigmaNode currentNode, ProcessedNode parent)
		{
			StringBuilder builder = new StringBuilder ();
			var name = "checkbox";
			builder.AppendLine ($"var {name} = new {nameof (NSButton)}();");
			builder.AppendLine ($"{name}.SetButtonType ({nameof (NSButtonType)}.({nameof (NSButtonType.Switch)}));");
			builder.Configure (name, currentNode);
			return builder.ToString ();
		}
	}
}
