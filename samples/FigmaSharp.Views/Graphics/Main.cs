using AppKit;
using LiteForms;
using LiteForms.Cocoa;
using System;
namespace BasicGraphics.Cocoa
{
	static class MainClass
    {
		static Page1 page1;
		static Page2 page2;
		static Page3 page3;

		static PageView selectedPage;

		static View pageContent;
		static StackView buttonContentStackView;
		static Window mainWindow;
		static OptionsPanelGradienContentView actionContainerView;

		static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Regular;

            mainWindow = new Window(new Rectangle(0, 0, 567, 675)) {
                Title = "Test Window",
				Resizable = false,
			};
			//mainWindow.Content.BackgroundColor = Color.Black;

			mainWindow.Content.BackgroundColor = Color.Transparent;
			mainWindow.Center();
			mainWindow.MovableByWindowBackground = mainWindow.Content.MovableByWindowBackground = true;
			mainWindow.Borderless = true;
			mainWindow.IsOpaque = false;
			mainWindow.BackgroundColor = Color.Transparent;

			mainWindow.IsFullSizeContentView = true;

			buttonContentStackView = new StackView() {
				Orientation = LayoutOrientation.Horizontal,
				Allocation = new Rectangle(0, 0, mainWindow.Size.Width, 20),
				MovableByWindowBackground = true
			};
			mainWindow.Content.AddChild(buttonContentStackView);


			actionContainerView = new OptionsPanelGradienContentView()
			{
				Allocation = new Rectangle(10, 25, mainWindow.Size.Width - 20, mainWindow.Size.Height - 35),
				MovableByWindowBackground = true
			};
			mainWindow.Content.AddChild(actionContainerView);

			page1 = new Page1(actionContainerView);
			page2 = new Page2(actionContainerView);
			page3 = new Page3(actionContainerView);

			var button = new Button() { Text = "Transformation" };
			buttonContentStackView.AddChild(button);
			button.Clicked += (s, e) => SelectPage(page1);

			var button2 = new Button() { Text = "Svg" };
			buttonContentStackView.AddChild(button2);
			button2.Clicked += (s, e) => SelectPage(page2);

			var button3 = new Button() { Text = "Shapes" };
			buttonContentStackView.AddChild(button3);
			button3.Clicked += (s, e) => SelectPage(page3);

			//scrollView.SetContentSize(800, 1000);

			mainWindow.Resize += (s, e) => Refresh();

			SelectPage(page1);

			mainWindow.Show();

            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            NSApplication.SharedApplication.Run();
        }


		static void Refresh ()
		{
			buttonContentStackView.Width = mainWindow.Size.Width;
			//pageContent.Size = new Size(mainWindow.Size.Width, mainWindow.Size.Height - 40);

			actionContainerView.Size = new Size(mainWindow.Size.Width - 20, mainWindow.Size.Height - 35);

			//selectedPage.OnWindowResize(null, EventArgs.Empty);
		}

		static void SelectPage (PageView page)
		{
			if (selectedPage != null)
				selectedPage.OnHide();

			selectedPage = page;

			selectedPage.OnShown();

			Refresh();
		}

    }
}
