﻿/* 
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
using LocalFile;
using LocalFile.Shared;

namespace LocalFile.Cocoa
{
    static class MainClass
    {
        static DocumentExample documentExample;

        static void Main(string[] args)
        {
            FigmaApplication.Init(Environment.GetEnvironmentVariable("TOKEN"));

            NSApplication.Init();
            NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Regular;

            var mainWindow = new NSWindow(new CGRect(0, 0, 800, 600), NSWindowStyle.Titled | NSWindowStyle.Resizable | NSWindowStyle.Closable, NSBackingStore.Buffered, false);
            mainWindow.Title = "Cocoa Figma Local File Sample";
			mainWindow.Center();

            var stackView = new NSStackView() { Orientation = NSUserInterfaceLayoutOrientation.Vertical };
            mainWindow.ContentView = stackView;

            scrollView = new NSScrollView();
            stackView.AddArrangedSubview(scrollView);

            var scrollViewWrapper = new ScrollViewWrapper(scrollView);
            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();
            var storyboard = new FigmaStoryboard (converters);

            documentExample = new DocumentExample(scrollViewWrapper, storyboard);

            mainWindow.MakeKeyAndOrderFront(null);
            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            NSApplication.SharedApplication.Run();
        }

        static NSScrollView scrollView;
    }
}
