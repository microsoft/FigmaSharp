using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

using AppKit;
using CoreGraphics;
using Foundation;

using FigmaSharp;
using FigmaSharp.Cocoa;
using FigmaSharp.Services;

using ExampleFigma;

namespace ExampleFigmaMac
{
    static class MainClass
    {
        static IScrollViewWrapper scrollViewWrapper;
         
        static void Main(string[] args)
        {
            FigmaApplication.Init(Environment.GetEnvironmentVariable("TOKEN"));

            NSApplication.Init();
            NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Regular;          

            var mainWindow = new NSWindow(new CGRect(0, 0, 800, 600), NSWindowStyle.Titled | NSWindowStyle.Resizable | NSWindowStyle.Closable, NSBackingStore.Buffered, false);
            mainWindow.Title = "Cocoa Basic Rendering Sample";
			mainWindow.Center();

            var stackView = new NSStackView() { Orientation = NSUserInterfaceLayoutOrientation.Vertical };
            scrollView = new NSScrollView()
            {
                HasVerticalScroller = true,
                HasHorizontalScroller = true,
                AutomaticallyAdjustsContentInsets = false
            };

            scrollViewWrapper = new ScrollViewWrapper(scrollView);

            stackView.AddArrangedSubview(scrollView);

            scrollView.AutohidesScrollers = false;
            scrollView.BackgroundColor = NSColor.Black;
            scrollView.ScrollerStyle = NSScrollerStyle.Legacy;
            mainWindow.ContentView = stackView;

            var contentView = new NSView { Frame = new CGRect(CGPoint.Empty, mainWindow.Frame.Size) };
            contentView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;

            scrollView.DocumentView = contentView;

            manager = new ExampleViewManager(scrollViewWrapper);
            manager.Initialize();

            mainWindow.MakeKeyAndOrderFront(null);
            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            NSApplication.SharedApplication.Run();
        }

        static NSScrollView scrollView;
        static ExampleViewManager manager;
    }
}
