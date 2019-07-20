using LiteForms;
using LiteForms.Cocoa;

namespace BasicControls
{
    public class ButtonViewContent : ExampleBaseContent
    {
        public ButtonViewContent()
        {
            AddChild(new TextBox() { Text = "button" });
            BackgroundColor = Color.Red;
        }
    }
}
