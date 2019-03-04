using System.Text;
using AppKit;
using System.Linq;

namespace FigmaSharp.NativeControls
{
	public class PopupButtonConverter : CustomViewConverter
	{
		public override bool CanConvert (FigmaNode currentNode)
		{
			return ContainsType (currentNode, "popupbutton");
		}

		public override IViewWrapper ConvertTo (FigmaNode currentNode, ProcessedNode parent)
		{
			var view = new NSPopUpButton ();
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

			var textFieldView = ((IFigmaDocumentContainer)currentNode).children.OfType<FigmaText> ().Where (s => s.name == "value");
			foreach (var item in textFieldView) {
				view.AddItem (item.characters);
			}

			return new ViewWrapper (view);
		}

		public override string ConvertToCode (FigmaNode currentNode, ProcessedNode parent)
		{
			StringBuilder builder = new StringBuilder ();
			var name = "nsPopupButton";
			builder.AppendLine ($"var {name} = new {nameof (NSPopUpButton)}();");
			builder.AppendLine ($"{name}.SetButtonType ({nameof (NSButtonType)}.({nameof (NSButtonType.Switch)}));");
			builder.Configure (name, currentNode);
			return builder.ToString ();
		}
	}
}
