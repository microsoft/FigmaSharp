using LiteForms;
using LiteForms.Cocoa;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicControls
{
    enum Samples
    {
        BasicViews,
        Buttons,
    }

    public class MainSample
    {
        const int menuWidth = 120;

        ExampleBaseContent contentView;
        Window mainWindow;
        StackView menuStackView;

        public MainSample()
        {
            mainWindow = new Window(new Rectangle(0, 0, 540, 800)) {
                Title = "Test Window", 
            };

            menuStackView = new StackView {
                Orientation = LayoutOrientation.Vertical,
                Padding = new Padding(10),
                Allocation = new Rectangle(0, 0, menuWidth, mainWindow.Content.Allocation.Height),
                BackgroundColor = Color.Green
            };
            mainWindow.Content.AddChild(menuStackView);

            //contentView = new View() {
            //    Allocation = new Rectangle(menuWidth, 0, mainWindow.Content.Allocation.Width - menuWidth, mainWindow.Content.Allocation.Height),
            //    BackgroundColor = Color.Red
            //};

            mainWindow.Resize += (s, e) => Refresh();

            //examples
            foreach (var example in examples)
            {
                var example1Button = new Button() { Text = example.name };
                menuStackView.AddChild(example1Button);
                example1Button.Clicked += (s, e) => Select(example.sample);
            }

            //mainWindow.Content.AddChild(contentView);
            mainWindow.Show();
        }

        void Refresh ()
        {
            menuStackView.SetAllocation(0, 0, menuWidth, mainWindow.Content.Allocation.Height);
            contentView?.SetAllocation(menuWidth, 0, mainWindow.Content.Allocation.Width - menuWidth, mainWindow.Content.Allocation.Height);
        }

        static List<(string name, Samples sample)> examples = new List<(string name, Samples sample)>
        {
             ("Example 1", Samples.BasicViews),
             ("Example 2", Samples.Buttons)
        };

        void Select (Samples screen)
        {
            if (contentView != null)
                mainWindow.Content.RemoveChild(contentView);
            contentView = null;

            switch (screen)
            {
                case Samples.BasicViews:
                    contentView = new BasicViewsContent();
                    break;
                case Samples.Buttons:
                    contentView = new ButtonViewContent();
                    break;
            }

            if (contentView != null)
                mainWindow.Content.AddChild(contentView);
            Refresh();
        }
    }
}
