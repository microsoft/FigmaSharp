﻿using System;
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

            var scrollWindowFixed = new Fixed();
            scrollWindow.AddWithViewport(scrollWindowFixed);

            var scrollViewWrapper = new ScrollViewWrapper(scrollWindow, null);
            var manager = new ExampleViewManager(scrollViewWrapper);
            manager.Initialize();

            window.ShowAll();

            Application.Run();
        }
    }
}
