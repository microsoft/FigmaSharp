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
using FigmaSharp.NativeControls;
using FigmaSharp.Views;

namespace FigmaSharp.Services
{
    public class NativeViewRenderingService : FigmaViewRendererService
	{
        public NativeViewRenderingService (IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters = null,
			FigmaViewPropertySetterBase propertySetter = null,
			IColorConverter colorConverter = null)
            : base (figmaProvider,
                  figmaViewConverters ?? NativeControlsContext.Current.GetConverters (),
                  propertySetter ?? NativeControlsContext.Current.GetViewPropertySetter(),
                  colorConverter)
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

		public override bool ProcessesImageFromNode (FigmaNode node)
		{
 			return node.IsImageNode () || node.IsFigmaImageViewNode ();
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

        protected override bool RendersConstraints(ProcessedNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            if (currentNode.FigmaNode.IsDialogParentContainer())
                return false;
            if (currentNode.FigmaNode.IsNodeWindowContent())
                return false;
            return base.RendersConstraints (currentNode, parent, rendererService);
        }

        protected override bool RendersSize(ProcessedNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            //if (currentNode.FigmaNode.IsDialogParentContainer())
            //    return false;

            //if (currentNode.FigmaNode.IsNodeWindowContent())
            //    return false;
            return true;
        }

        protected override bool RendersAddChild(ProcessedNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            return true;
        }

        protected override bool NodeScansChildren (FigmaNode currentNode, CustomViewConverter converter, FigmaViewRendererServiceOptions options)
		{
			if (currentNode.IsFigmaImageViewNode ())
				return false;

			return base.NodeScansChildren (currentNode, converter, options);
		}

		protected override bool SkipsNode (FigmaNode currentNode, ProcessedNode parent, FigmaViewRendererServiceOptions options)
		{
			if (currentNode.IsDialog ()) {
				return true;
			}
			return false;
		}

        internal ProcessedNode[] GetProcessedNodes(FigmaNode[] mainNodes)
		{
            ProcessedNode[] resultNodes = new ProcessedNode[mainNodes.Length];
			for (int i = 0; i < mainNodes.Length; i++)
			{
                var currentNode = mainNodes[i];
                resultNodes[i] = NodesProcessed.FirstOrDefault(s => s.FigmaNode == currentNode);
            }
            return resultNodes;
		}
	}
}
