// Authors:
//   Jose Medrano <josmed@microsoft.com>
//   Hylke Bons <hylbo@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
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
using System.Linq;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Controls.Cocoa
{
    public class EmbededSheetDialogConverter : NodeConverter
	{
		public override Type GetControlType(FigmaNode currentNode) => typeof(AppKit.NSView);

        public EmbededSheetDialogConverter(INodeProvider newWindowProvider)
		{

		}

		public override bool CanConvert (FigmaNode currentNode)
		{
			return currentNode.IsDialogParentContainer(FigmaControlType.WindowSheet);
		}

		public override IView ConvertToView (FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
		{
			var frame = (FigmaFrame)currentNode;

			var nativeView = new FakeSheetView();
			var view = new View(nativeView);
			nativeView.Configure(frame);

			/* TODO: Create a base FakeView class with common elements
			nativeView.LiveButton.Activated += (s, e) => {
				if (nativeView.Window == null)
					return;

				var window = new Window(view.Allocation);

				window.KeyDown += (send, eve) => {
					NSApplication.SharedApplication.EndSheet((NSWindow)window.NativeObject);
					window.Close();
				};

				newWindowProvider.Load(rendererService.FileProvider.File);
				var secondaryRender = new NativeViewRenderingService(newWindowProvider);
				////we want to include some special converters to handle windows like normal view containers
				//rendererService.CustomConverters.Add(new EmbededSheetDialogConverter());
				//rendererService.CustomConverters.Add(new EmbededWindowConverter());
				secondaryRender.RenderInWindow(window, currentNode);
				NSApplication.SharedApplication.BeginSheet((NSWindow)window.NativeObject, nativeView.Window);
			};
			*/

			return view;
		}

		public override string ConvertToCode (CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
		{
			return string.Empty;
		}
	}

	public class EmbededWindowConverter : NodeConverter
	{
        public override Type GetControlType(FigmaNode currentNode)
        => typeof(AppKit.NSView);

        public bool LiveButtonAlwaysVisible { get; set; } = true;

		public event EventHandler LivePreviewLoading;
		public event EventHandler LivePreviewLoaded;

		readonly INodeProvider newWindowProvider;

		public EmbededWindowConverter(INodeProvider newWindowProvider)
		{
			this.newWindowProvider = newWindowProvider;
		}

		public override bool CanConvert (FigmaNode currentNode)
		{
			if (currentNode.IsWindowContent ()) {
				return currentNode.Parent != null && currentNode.Parent.IsDialogParentContainer (FigmaControlType.Window);
			}

			var isWindow = currentNode.IsDialogParentContainer (FigmaControlType.Window);
			return isWindow;
		}

		public override IView ConvertToView (FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
		{
			string title = "";
			var frame = (FigmaFrame)currentNode;

			var nativeView = new FakeWindowView(title);
			nativeView.LiveButtonAlwaysVisible = LiveButtonAlwaysVisible;
			nativeView.Configure (currentNode);

			var view = new View(nativeView);

			var windowComponent = currentNode.GetDialogInstanceFromParentContainer();
			if (windowComponent != null) {

				var optionsNode = windowComponent.Options ();
				if (optionsNode is IFigmaNodeContainer figmaNodeContainer)
				{
					nativeView.CloseButtonHidden = (figmaNodeContainer.HasChildrenVisible("close") == false);
					nativeView.MinButtonHidden   = (figmaNodeContainer.HasChildrenVisible("min")   == false);
					nativeView.MaxButtonHidden   = (figmaNodeContainer.HasChildrenVisible("max")   == false);

					var titleText = (FigmaText) optionsNode.GetChildren().FirstOrDefault(s => s.name == "title" && s.visible);

					if (titleText != null)
						nativeView.Title = titleText.characters;
				}
			}

			nativeView.LiveButton.Activated += async (s, e) => {
				var window = new Window(view.Allocation);

				LivePreviewLoading?.Invoke(this, EventArgs.Empty);

				await newWindowProvider.LoadAsync (rendererService.NodeProvider.File);

				var secondaryRender = new ControlViewRenderingService(newWindowProvider);

				var options = new ViewRenderServiceOptions() { GenerateMainView = false };
				secondaryRender.RenderInWindow(window, currentNode, options);

				var mainNodes = currentNode.GetChildren()
					.ToArray();

				ViewNode[] processedNodes = secondaryRender.GetProcessedNodes(mainNodes);

				var layoutManager = new StoryboardLayoutManager() { UsesConstraints = true };
				layoutManager.Run(processedNodes, window.Content, secondaryRender);

				var nativeWindow = (NSWindow)window.NativeObject;
				nativeWindow.Appearance = nativeView.EffectiveAppearance;
				nativeWindow.ContentMinSize = nativeWindow.ContentView.Frame.Size;

				nativeWindow.Center();
				nativeWindow.MakeKeyAndOrderFront(null);

				LivePreviewLoaded?.Invoke(this, EventArgs.Empty);
			};

			return view;
		}

		public override string ConvertToCode (CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
		{
			return string.Empty;
		}
	}
}
