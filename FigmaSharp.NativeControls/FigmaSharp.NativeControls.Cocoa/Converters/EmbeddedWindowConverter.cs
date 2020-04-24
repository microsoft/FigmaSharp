/* 
 * CustomTextFieldConverter.cs
 * 
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

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.NativeControls;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class EmbededSheetDialogConverter : FigmaViewConverter
	{
		public EmbededSheetDialogConverter(IFigmaFileProvider newWindowProvider)
		{

		}

		public override bool CanConvert (FigmaNode currentNode)
		{
			var isWindow = currentNode.IsDialogParentContainer (NativeControlType.WindowSheet) ||
			               currentNode.IsDialogParentContainer (NativeControlType.WindowPanel);
			return isWindow;
		}

		public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var frame = (FigmaFrameEntity)currentNode;

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

		public override string ConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			return string.Empty;
		}
	}

	public class EmbededWindowConverter : FigmaViewConverter
	{
		public bool LiveButtonAlwaysVisible { get; set; } = true;

		public event EventHandler LivePreviewLoading;
		public event EventHandler LivePreviewLoaded;

		readonly IFigmaFileProvider newWindowProvider;

		public EmbededWindowConverter(IFigmaFileProvider newWindowProvider)
		{
			this.newWindowProvider = newWindowProvider;
		}

		public override bool CanConvert (FigmaNode currentNode)
		{
			if (currentNode.IsWindowContent ()) {
				return currentNode.Parent != null && currentNode.Parent.IsDialogParentContainer (NativeControlType.WindowStandard);
			}

			var isWindow = currentNode.IsDialogParentContainer (NativeControlType.WindowStandard);
			return isWindow;
		}

		public override IView ConvertTo (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			string title = "";
			var frame = (FigmaFrameEntity)currentNode;

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

					FigmaText titleText = optionsNode.FindNode(s => s.name == "title" && s.visible) as FigmaText;

					if (titleText != null)
						nativeView.Title = titleText.characters;
				}
			}

			nativeView.LiveButton.Activated += async (s, e) => {
				var window = new Window(view.Allocation);

				LivePreviewLoading?.Invoke(this, EventArgs.Empty);

				await newWindowProvider.LoadAsync (rendererService.FileProvider.File);

				var secondaryRender = new NativeViewRenderingService(newWindowProvider);

				var options = new FigmaViewRendererServiceOptions() { GenerateMainView = false };
				secondaryRender.RenderInWindow(window, currentNode, options);

				var mainNodes = currentNode.GetChildren().ToArray();
				ProcessedNode[] processedNodes = secondaryRender.GetProcessedNodes(mainNodes);

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

		public override string ConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			return string.Empty;
		}
	}
}
