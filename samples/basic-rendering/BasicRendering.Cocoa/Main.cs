/* 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

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

namespace LocalFile.Cocoa
{
    static class MainClass
    {
        static ExampleViewManager manager;
        static IScrollViewWrapper scrollViewWrapper;
        static NSScrollView scrollView;

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
                AutohidesScrollers = false,
                AutomaticallyAdjustsContentInsets = false,
                BackgroundColor = NSColor.Black,
                HasHorizontalScroller = true,
                HasVerticalScroller = true,
                ScrollerStyle = NSScrollerStyle.Overlay
            };

            scrollViewWrapper = new ScrollViewWrapper(scrollView);
            stackView.AddArrangedSubview(scrollView);
            mainWindow.ContentView = stackView;

            var contentView = new NSView { Frame = new CGRect(CGPoint.Empty, mainWindow.Frame.Size) };
            var viewWrapper = new ViewWrapper(contentView);
            manager = new ExampleViewManager(scrollViewWrapper, viewWrapper);

            mainWindow.MakeKeyAndOrderFront(null);
            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            NSApplication.SharedApplication.Run();
        }
    }
}
