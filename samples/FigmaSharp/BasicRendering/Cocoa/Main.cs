// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

using AppKit;

using FigmaSharp;
using FigmaSharp.Cocoa;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace LocalFile.Cocoa
{
    static class MainClass
	{
		//static ExampleViewManager manager;
		static IScrollView scrollView;

        const string fileName = "WOzDi5CXie6lQeXulTw1CO";

		static void Main (string[] args)
		{
			FigmaApplication.Init (Environment.GetEnvironmentVariable ("TOKEN"));

			NSApplication.Init ();
			NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Regular;

			var stackView = new StackView () { Orientation = LayoutOrientation.Vertical };

			scrollView = new ScrollView();

			var mainWindow = new Window (new Rectangle (0, 0, 540, 800)) {
				Content = scrollView
			};

			mainWindow.Closing += delegate { NSRunningApplication.CurrentApplication.Terminate (); };

			//TIP: the render consist in 2 steps:
			//1) generate all the views, decorating and calculate sizes
			//2) with this views we generate the hierarchy and position all the views based in the
			//native toolkit positioning system

			//in this case we want use a remote file provider (figma url from our document)
			var fileProvider = new RemoteNodeProvider();

			//we initialize our renderer service, this uses all the converters passed
			//and generate a collection of NodesProcessed which is basically contains <FigmaModel, IView, FigmaParentModel>
			var rendererService = new ViewRenderService (fileProvider);
			rendererService.Start (fileName, scrollView.ContentView);

			//now we have all the views processed and the relationship we can distribute all the views into the desired base view
			var layoutManager = new StoryboardLayoutManager();
			layoutManager.Run (scrollView.ContentView, rendererService);

			//NOTE: some toolkits requires set the real size of the content of the scrollview before position layers
			scrollView.AdjustToContent();

			mainWindow.Show ();
			//mainWindow.Title = manager.WindowTitle;

			NSApplication.SharedApplication.ActivateIgnoringOtherApps (true);
			NSApplication.SharedApplication.Run ();
		}
	}
}
