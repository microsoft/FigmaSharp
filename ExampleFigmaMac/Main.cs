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
           
            var stackView = new FlippedStackView() { Orientation = NSUserInterfaceLayoutOrientation.Vertical };
            var button = new NSButton() { Title = "Refresh" };
            stackView.AddArrangedSubview(button);

            scrollView = new FlippedScrollView()
            {
                HasVerticalScroller = true,
                HasHorizontalScroller = true,
                AutomaticallyAdjustsContentInsets = false
            };

            scrollViewWrapper = new ScrollViewWrapper(scrollView);

            stackView.AddArrangedSubview(scrollView);

            scrollView.AutohidesScrollers = false;
            scrollView.BackgroundColor = NSColor.Orange;
            scrollView.ScrollerStyle = NSScrollerStyle.Legacy;
            mainWindow.ContentView = stackView;

            var contentView = new FlippedView { Frame = new CGRect(CGPoint.Empty, mainWindow.Frame.Size) };
            contentView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;

            scrollView.DocumentView = contentView;

            Refresh(contentView);

            mainWindow.MakeKeyAndOrderFront(null);
            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            NSApplication.SharedApplication.Run();
        }

        static void Refresh(NSView contentView)
        {
            ReadStoryboardFigmaFile (); //Example reading storyboard figma
            //ReadRemoteFigmaFile (contentView); //Example reading remote file
            //ReadLocalFilePath(contentView); //Example reading local file
        }

        //Example 1
        static void ReadStoryboardFigmaFile()
        {
            var testStoryboard = new FigmaStoryboard();
            scrollView.DocumentView = testStoryboard.ContentView.NativeObject as NSView;
            //we need reload after set the content to ensure the scrollview
            testStoryboard.Reload(true);
            scrollViewWrapper.AdjustToContent();
        }

        static NSScrollView scrollView;
        static ExampleViewManager manager;

        //Example 2
        static void ReadRemoteFigmaFile(NSView contentView)
        {
            var fileName = Environment.GetEnvironmentVariable("FILE");
            manager = new ExampleViewManager(scrollViewWrapper, fileName);
            manager.Initialize();
        }
    }
}
