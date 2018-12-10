using System;
using System.Collections.Generic;
using System.IO;
using AppKit;
using CoreGraphics;
using FigmaSharp;
using MonoDevelop.Inspector.Mac;
using System.Linq;
using System.Net;
using Foundation;

namespace ExampleFigmaMac
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Regular;
          
            var xPos = NSScreen.MainScreen.Frame.Width / 2;
            var yPos = NSScreen.MainScreen.Frame.Height / 2;

            var mainWindow = new MacAccInspectorWindow(new CGRect(xPos, yPos, 300, 368), NSWindowStyle.Titled | NSWindowStyle.Resizable | NSWindowStyle.Closable, NSBackingStore.Buffered, false);
            var token = Environment.GetEnvironmentVariable("TOKEN");
            FigmaEnvirontment.SetAccessToken(token);

            var stackView = new NSStackView() { Orientation = NSUserInterfaceLayoutOrientation.Vertical };
            var button = new NSButton() { Title = "Refresh" };
            stackView.AddArrangedSubview(button);

            scrollView = new NSScrollView()
            {
                HasVerticalScroller = true,
                HasHorizontalScroller = true,
                AutomaticallyAdjustsContentInsets = false
            };
            stackView.AddArrangedSubview(scrollView);

            scrollView.AutohidesScrollers = false;
            scrollView.BackgroundColor = NSColor.Orange;
            scrollView.ScrollerStyle = NSScrollerStyle.Legacy;
            mainWindow.ContentView = stackView;

            var contentView = new NSView { Frame = new CGRect(CGPoint.Empty, mainWindow.Frame.Size) };
            contentView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;

            scrollView.DocumentView = contentView;

            Refresh(contentView);

            button.Activated += (sender, e) =>
            {
                //Refresh (contentView);
            };

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
            scrollView.DocumentView = testStoryboard.ContentView;
            //we need reload after set the content to ensure the scrollview
            testStoryboard.Reload(true);
        }

        static NSScrollView scrollView;

        //Example 2
        static void ReadRemoteFigmaFile(NSView contentView)
        {
            var viewName = "";
            var fileName = Environment.GetEnvironmentVariable("FILE");
            var nodeName = "";

            contentView.LoadFigmaFromUrlFile(fileName, out List<FigmaImageView> figmaImageViews, viewName, nodeName);
            figmaImageViews.Load(fileName);
        }

        //Example 3
        static void ReadLocalFilePath(NSView contentView)
        {
            var fileName = Environment.GetEnvironmentVariable("FILE");
            var figmaFile = "FigmaStoryboard.figma";

            var currentLocation = Path.GetDirectoryName(typeof(MainClass).Assembly.Location);
            var resourcesLocation = Path.Combine(Path.GetDirectoryName(currentLocation), "Resources");
            var figmaFilePath = Path.Combine(resourcesLocation, figmaFile);

            contentView.LoadFigmaFromResource(figmaFile, out List<FigmaImageView> figmaImageViews);
            figmaImageViews.LoadFromLocalImageResources();
        }
    }
}
