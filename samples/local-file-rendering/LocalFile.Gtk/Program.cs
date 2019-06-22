using System;
using FigmaSharp.GtkSharp;
using Gtk;
using LocalFile.Shared;

namespace LocalFile.Gtk
{
    class MainClass
    {
        static DocumentExample documentExample;

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

            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();
            var storyboard = new FigmaStoryboard(converters);

            var scrollViewWrapper = new ScrollViewWrapper(scrollWindow);
            documentExample = new DocumentExample(scrollViewWrapper, storyboard);

            window.ShowAll();

            Application.Run();
        }
    }
}
