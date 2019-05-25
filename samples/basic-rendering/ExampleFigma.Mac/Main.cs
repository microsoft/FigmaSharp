using System;
using System.Collections.Generic;
using System.IO;
using AppKit;
using CoreGraphics;
using FigmaSharp;
using System.Net;
using Foundation;
using FigmaSharp.Services;
using ExampleFigma;
using FigmaSharp.Cocoa;

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
          
            var xPos = NSScreen.MainScreen.Frame.Width / 2;
            var yPos = NSScreen.MainScreen.Frame.Height / 2;

            var mainWindow = new NSWindow(new CGRect(xPos, yPos, 300, 368), NSWindowStyle.Titled | NSWindowStyle.Resizable | NSWindowStyle.Closable, NSBackingStore.Buffered, false);
            mainWindow.Title = "Cocoa Basic Rendering Sample";

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

            Refresh(contentView);

            mainWindow.MakeKeyAndOrderFront(null);
            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            NSApplication.SharedApplication.Run();
        }

        static void Refresh(NSView contentView)
        {
            ReadRemoteFigmaFile (contentView); //Example reading remote file
        }

        static NSScrollView scrollView;
        static ExampleViewManager manager;

        //Example 2
        static void ReadRemoteFigmaFile(NSView contentView)
        {
            var fileName = "nWFYvx7YMBEtPAgAydX66e";
            manager = new ExampleViewManager(scrollViewWrapper, fileName);
            manager.Initialize();
        }
    }
}
