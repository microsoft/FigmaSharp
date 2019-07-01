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
using System.Linq;
using AppKit;
using CoreGraphics;
using FigmaSharp;
using FigmaSharp.Cocoa;
using LocalFile.Shared;

namespace LocalFile.Cocoa
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
            mainWindow.Title = "Cocoa Figma Local File Sample";

            scrollView = new NSScrollView();
            scrollViewWrapper = new ScrollViewWrapper(scrollView);

            mainWindow.ContentView = scrollView;

            var figmaConverters = FigmaSharp.NativeControls.Cocoa.Resources.GetConverters()
                .Union (FigmaSharp.AppContext.Current.GetFigmaConverters())
                .ToArray ();

            exampleViewManager = new ExampleViewManager(scrollViewWrapper, figmaConverters);

            var rendererService = exampleViewManager.RendererService;
            var urlTextField = rendererService.FindNativeViewByName<NSTextField>("FigmaUrlTextField");
            var bundleButton = rendererService.FindNativeViewByName<NSButton>("BundleButton");
            var cancelButton = rendererService.FindNativeViewByName<NSButton>("CancelButton");

            if (cancelButton != null)
            {
                cancelButton.Activated += (s, e) =>
                {
                    if (urlTextField != null)
                        urlTextField.StringValue = "You pressed cancel";
                };
            }

            if (bundleButton != null)
            {
                bundleButton.Activated += (s, e) =>
                {
                    if (urlTextField != null)
                        urlTextField.StringValue = "You pressed bundle";
                };
            }

            mainWindow.MakeKeyAndOrderFront(null);
            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            NSApplication.SharedApplication.Run();
        }
        static ExampleViewManager exampleViewManager;
        static NSScrollView scrollView;
    }
}

