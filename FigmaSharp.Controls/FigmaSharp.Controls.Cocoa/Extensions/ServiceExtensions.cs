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

using System.Linq;

using FigmaSharp.Controls.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace FigmaSharp.Controls
{
    public static class ServiceExtensions
    {
        public static FigmaInstance GetBaseComponentNode(this NodeProvider fileProvider, FigmaNode node)
        {
            var figmaInstance = node.GetDialogInstanceFromParentContainer();
            if (figmaInstance != null) {
                foreach (var item in fileProvider.GetMainLayers ())
                {
                    var instance = item.GetDialogInstanceFromParentContainer();
                    if (instance != null && instance.id == figmaInstance.id)
                        return instance;
                }
            }
            return null;
        }

        public static void RenderInWindow(this ViewRenderService sender, IWindow mainWindow, string windowLayerName, ViewRenderServiceOptions options = null)
        {
            var windowFigmaNode = sender.NodeProvider.FindByName (windowLayerName);

            FigmaNode content = null;

            if (windowFigmaNode is IFigmaNodeContainer figmaNodeContainer)
            {
                content = figmaNodeContainer.children.FirstOrDefault(s => s.IsNodeWindowContent());

                var windowComponent = windowFigmaNode.GetDialogInstanceFromParentContainer();

                if (options == null)
                {
                    options = new ViewRenderServiceOptions();
                    options.AreImageProcessed = false;
                }

                options.ToIgnore = new FigmaNode[] { windowComponent };

                if (windowComponent != null)
                {
                    var windowLabel = windowComponent.children
                        .OfType<FigmaText>()
                        .FirstOrDefault();
                    if (windowLabel != null)
                        mainWindow.Title = windowLabel.characters;
                }
            }

            if (windowFigmaNode is IAbsoluteBoundingBox box)
                mainWindow.Size = box.absoluteBoundingBox.Size;

            if (content == null)
            {
                content = windowFigmaNode;
            }

            var renderContent = sender.RenderByNode(content, mainWindow.Content, options);
            mainWindow.Content = renderContent;
		}
    }
}
