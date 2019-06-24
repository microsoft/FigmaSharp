using System;
using ExampleFigma;
using FigmaSharp;
using FigmaSharp.GtkSharp;
using Gtk;

namespace BasicRendering.Gtk
{
    class MainClass
    {
        private static ExampleViewManager exampleViewManager;

        public static void Main(string[] args)
        {
            FigmaApplication.Init(Environment.GetEnvironmentVariable("TOKEN"));
            Application.Init();

            var window = new Window(WindowType.Toplevel);
            window.HeightRequest = window.WidthRequest = 700;

            var scrollWindow = new ScrolledWindow();
            window.Add(scrollWindow);

            var scrollViewWrapper = new ScrollViewWrapper(scrollWindow);
            exampleViewManager = new ExampleViewManager(scrollViewWrapper);

            window.ShowAll();

            Application.Run();
        }
    }
}
