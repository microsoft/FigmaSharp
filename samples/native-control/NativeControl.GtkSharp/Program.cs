using System;
using ExampleFigmaMac;
using FigmaSharp.GtkSharp;
using Gtk;
using System.Linq;
using FigmaSharp.NativeControls.GtkSharp;

namespace NativeControl.GtkSharp
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

            var scrollWindowFixed = new Fixed();
            scrollWindow.AddWithViewport(scrollWindowFixed);

            var figmaConverters = FigmaSharp.AppContext.Current.GetFigmaConverters().Union(Resources.GetConverters()).ToArray();
            var scrollViewWrapper = new ScrollViewWrapper(scrollWindow, null);
            var manager = new ExampleViewManager(scrollViewWrapper, figmaConverters);
            manager.Initialize();

            window.ShowAll();

            Application.Run();
        }
    }
}
