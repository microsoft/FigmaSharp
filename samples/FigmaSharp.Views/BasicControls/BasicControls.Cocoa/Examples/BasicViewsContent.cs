using System;
using LiteForms.Cocoa;

namespace BasicControls
{

    public class BasicViewsContent : ExampleBaseContent
    {
        public BasicViewsContent()
        {
            AddChild(new TextBox() { Text = "dd" });
            AddChild(new TextBox() { Text = "dd" });
            
        }
    }
}
