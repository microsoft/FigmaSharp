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
using LiteForms;
using LiteForms.Cocoa;

namespace LocalFile.Cocoa
{

	static class MainClass
    {
        static IScrollView scrollView;
		
        static void Main(string[] args)
        {
            FigmaApplication.Init(Environment.GetEnvironmentVariable("TOKEN"));

            NSApplication.Init();
            NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Regular;

			scrollView = new ScrollView ();

			var mainWindow = new Window(new Rectangle(0, 0, 300, 368))
			{
				Title = "Cocoa Figma Local File Sample",
				Content = scrollView
			};

			var figmaConverters = FigmaSharp.NativeControls.Cocoa.Resources.GetConverters()
                .Union (FigmaSharp.AppContext.Current.GetFigmaConverters())
                .ToArray ();

            exampleViewManager = new ExampleViewManager(scrollView, figmaConverters);

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

			mainWindow.Show ();

            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            NSApplication.SharedApplication.Run();
        }

        static ExampleViewManager exampleViewManager;
    }
}

