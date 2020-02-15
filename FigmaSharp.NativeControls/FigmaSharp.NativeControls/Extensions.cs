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

using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace FigmaSharp
{
    public static class Extensions
    {
		public static bool IsFigmaImageViewNode (this FigmaNode node)
		{
			if (node.name.Contains ("!image")) {
                return true;
			}
            return false;
        }

		public static void RenderInWindow(this FigmaViewRendererService sender, IWindow mainWindow,  params string[] path)
		{
			RenderInWindow(sender, mainWindow, new FigmaViewRendererServiceOptions(), path);
		}

		public static void RenderInWindow(this FigmaViewRendererService sender, IWindow mainWindow, FigmaViewRendererServiceOptions options, params string[] path)
        {
            //var windowFigmaNode = sender.FileProvider.FindByName (path[0]);

            //sender.RenderInWindow (mainWindow, windowFigmaNode, options);

            //var windowBoundingBox = (IAbsoluteBoundingBox)windowFigmaNode;

            ////var contentPath = path.Concat(new[] { "content" }).ToArray();

            ////var contentFigmaNode = (IAbsoluteBoundingBox)windowFigmaNode;
            //var contentView = sender.RenderByNode<IView>(windowFigmaNode, options);
            //mainWindow.Content = contentView;

            //mainWindow.Size = windowBoundingBox.absoluteBoundingBox.Size;

            //var windowNodeContainer = (IFigmaNodeContainer)windowFigmaNode;
            //var windowComponent = windowNodeContainer.children
            //    .OfType<FigmaInstance>()
            //    .FirstOrDefault(s => s.Component.key == "b666bac2b68d976c9e3e5b52a0956a9f1857c1f2");

            //var windowLabel = windowComponent.children
            //    .OfType<FigmaText>()
            //    .FirstOrDefault();

            //mainWindow.Title = windowLabel.characters;

			//var difX = contentFigmaNode.absoluteBoundingBox.X - windowBoundingBox.absoluteBoundingBox.X;
			//var difY = contentFigmaNode.absoluteBoundingBox.Y - windowBoundingBox.absoluteBoundingBox.Y;

			//contentView.SetPosition (difX, difY);
		}
    }
}
