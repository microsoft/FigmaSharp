using System;
using ExampleFigma;
using FigmaSharp;
using FigmaSharp.GtkSharp;
using Gtk;

namespace BasicRendering.Gtk
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            FigmaApplication.Init(Environment.GetEnvironmentVariable("TOKEN"));
            Application.Init();

            var window = new Window(WindowType.Toplevel);
            window.HeightRequest = window.WidthRequest = 700;

            var scrollWindow = new ScrolledWindow
            {
                HscrollbarPolicy = PolicyType.Always,
                VscrollbarPolicy = PolicyType.Always
            };

            window.Add(scrollWindow);

            var contentViewWrapper = new ViewWrapper(new Fixed());
            var scrollViewWrapper = new ScrollViewWrapper(scrollWindow);
            var manager = new ExampleViewManager(scrollViewWrapper, contentViewWrapper);
            manager.Initialize();

            window.ShowAll();

            Application.Run();
        }
    }
}
