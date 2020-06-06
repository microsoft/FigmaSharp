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
using System.Collections.Generic;
using System.Linq;

using FigmaSharp.Models;
using FigmaSharp.Controls;
using FigmaSharp.Views;

namespace FigmaSharp.Services
{
    public class NativeViewRenderingService : FigmaViewRendererService
	{
		public NativeViewRenderingService (IFigmaFileProvider figmaProvider, ViewConverter[] figmaViewConverters = null, ViewPropertyNodeConfigureBase propertySetter = null)
            : base (figmaProvider,
                  figmaViewConverters ?? FigmaControlsContext.Current.GetConverters (),
                  propertySetter ?? FigmaControlsContext.Current.GetViewPropertySetter()
                  )
		{

		}

		protected override IEnumerable<FigmaNode> GetCurrentChildren(FigmaNode currentNode, FigmaNode parentNode, CustomViewConverter converter, FigmaViewRendererServiceOptions options)
		{
            var windowContent = currentNode.GetWindowContent();
            if (windowContent != null && windowContent is IFigmaNodeContainer nodeContainer) {
                return nodeContainer.children;
            }
			return base.GetCurrentChildren(currentNode, parentNode, converter, options);
		}

        public override void RenderInWindow(IWindow mainWindow, FigmaNode node, FigmaViewRendererServiceOptions options = null)
        {
            if (node is IAbsoluteBoundingBox bounNode) {
                mainWindow.Size = new Size(bounNode.absoluteBoundingBox.Width, bounNode.absoluteBoundingBox.Height);
            }

            if (options == null) {
                options = new FigmaViewRendererServiceOptions() { GenerateMainView = false };
            }

            var content = node.GetWindowContent() ?? node;
            ProcessFromNode(content, mainWindow.Content, options);
            var processedNode = FindProcessedNodeById(content.id);
            
            RecursivelyConfigureViews(processedNode, options);

            var windowComponent = node.GetDialogInstanceFromParentContainer();
            if (windowComponent != null) {
                var optionsNode = windowComponent.Options();
                if (optionsNode is IFigmaNodeContainer figmaNodeContainer) {
                    mainWindow.IsClosable = figmaNodeContainer.HasChildrenVisible("close");
                    mainWindow.Resizable = figmaNodeContainer.HasChildrenVisible("resize");
                    mainWindow.ShowMiniaturizeButton = figmaNodeContainer.HasChildrenVisible("min");
                    mainWindow.ShowZoomButton = figmaNodeContainer.HasChildrenVisible("max");
                }

                var titleText = optionsNode.FirstChild (s => s.name == "title" && s.visible) as FigmaText;
                if (titleText != null)
                    mainWindow.Title = titleText.characters;
            }
         }

        protected override bool RendersConstraints(ViewNode currentNode, ViewNode parentNode, FigmaRendererService rendererService)
        {
            if (currentNode.FigmaNode.IsDialogParentContainer())
                return false;
            if (currentNode.FigmaNode.IsNodeWindowContent())
                return false;
            return base.RendersConstraints (currentNode, parentNode, rendererService);
        }

        protected override bool RendersSize(ViewNode currentNode, ViewNode parentNode, FigmaRendererService rendererService)
        {
            //if (currentNode.FigmaNode.IsDialogParentContainer())
            //    return false;

            //if (currentNode.FigmaNode.IsNodeWindowContent())
            //    return false;
            return true;
        }

        protected override bool RendersAddChild(ViewNode currentNode, ViewNode parentNode, FigmaRendererService rendererService)
        {
            return true;
        }

        protected override bool NodeScansChildren (FigmaNode currentNode, CustomViewConverter converter, FigmaViewRendererServiceOptions options)
		{
			if (fileProvider.RendersAsImage (currentNode))
				return false;

			return base.NodeScansChildren (currentNode, converter, options);
		}

		protected override bool SkipsNode (FigmaNode currentNode, ViewNode parentNode, FigmaViewRendererServiceOptions options)
		{
			if (currentNode.IsDialog ()) {
				return true;
			}
			return false;
		}

        internal ViewNode[] GetProcessedNodes(FigmaNode[] mainNodes)
		{
            ViewNode[] resultNodes = new ViewNode[mainNodes.Length];
			for (int i = 0; i < mainNodes.Length; i++)
			{
                var currentNode = mainNodes[i];
                resultNodes[i] = NodesProcessed.FirstOrDefault(s => s.FigmaNode == currentNode);
            }
            return resultNodes;
		}
	}
}
