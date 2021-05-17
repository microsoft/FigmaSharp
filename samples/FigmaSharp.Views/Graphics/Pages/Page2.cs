//using LiteForms;
//using LiteForms.Cocoa;

using FigmaSharp;
using FigmaSharp.Views.Cocoa;

namespace BasicGraphics.Cocoa
{
    public class Page2 : PageView
    {
        SvgView svgShapeView;
        ComboBox combo;
        Button button;
        TextBox textBox;
        public Page2(OptionsPanelGradienContentView actionContainerView) : base(actionContainerView)
        {
            textBox = new TextBox();

            combo = new ComboBox();
            foreach (var item in System.IO.Directory.GetFiles("/Users/jmedrano/FigmaSharp/samples/FigmaSharp.Views/Graphics/Resources/", "*.svg"))
            {
                combo.AddItem(System.IO.Path.GetFileName(item));
            }

            combo.SelectionIndexChanged += (s, e) =>
            {
                ShowSvg(combo.SelectedItem);
            };

            button = new Button() { Text = "Convert"};
            button.Clicked += Button_Clicked;
        }

        private void Button_Clicked(object sender, System.EventArgs e)
        {
            svgShapeView = new SvgView();

            svgShapeView.Allocation = new Rectangle(40, 40, 300, 300);
            svgShapeView.Load(textBox.Text);
            actionContainerView.SetView(svgShapeView);
        }

        public override void OnWindowResize(object s, System.EventArgs e)
        {
            base.OnWindowResize(s, e);
            button.Allocation = new Rectangle(300, 500, 100, 20);
            textBox.Allocation = new Rectangle(300, 0, 400, 500);
        }

        void ShowSvg(string name)
        {
            var path = "/Users/jmedrano/FigmaSharp/samples/FigmaSharp.Views/Graphics/Resources/" + name;
            var fullPath = System.IO.File.ReadAllText(path);

            textBox.Text = fullPath;

            svgShapeView = new SvgView ();
          
            svgShapeView.Allocation = new Rectangle(40, 40, 300, 300);
            svgShapeView.Load(fullPath);
            actionContainerView.SetView(svgShapeView);
        }

        public override void OnShown()
        {
            actionContainerView.AddChild(textBox);
           
            actionContainerView.AddChild(combo);

            Allocation = new Rectangle(120, 120, 30, 20);

            actionContainerView.AddChild(button);

            button.Allocation = new Rectangle(50, 50, 50, 20);

            //ShowSvg(Start);
        }

        public override void OnHide()
        {
            actionContainerView.RemoveChild(combo);
            actionContainerView.RemoveView();
        }
    }
}
