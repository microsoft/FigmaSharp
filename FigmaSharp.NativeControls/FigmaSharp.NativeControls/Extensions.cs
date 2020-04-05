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

using System.Linq;
using System;

using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace FigmaSharp
{
    public static class Extensions
    {
        const string a11yLabel = "label:\"";
        const string a11yHelp = "help:\"";
        const string a11yRole = "role:\"";

        const string a11yRoleGroup = "group";

        const string a11yNodeName = "!a11y";

        static bool TrySearchParameter(FigmaNode node, string parameter, out string value)
        {
            value = node.name;
            try
            {
                var index = value.IndexOf(parameter);
                if (index > -1 && index < value.Length)
                {
                    value = value.Substring(index + parameter.Length);
                    index = value.IndexOf("\"");
                    if (index > -1 && index < value.Length)
                    {
                        value = value.Substring(0, index);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            value = null;
            return false;
        }

        public static bool TryGetPropertyValue (this FigmaNode node, string property, out string value)
		{
            if (node is IFigmaNodeContainer container)
            {
                foreach (var item in container.children) {
                   if (TrySearchParameter (item, property, out value)) {
                        return true;
				   }
                }
            }
            value = null;
            return false;
        }

        public static bool IsA11Group (this FigmaNode node)
        {
            if (TryGetPropertyValue (node.GetA11Node(), a11yRole, out var value) && value == a11yRoleGroup) {
                return true;
            }
            return false;
        }

        public static FigmaNode GetA11Node (this FigmaNode node)
        {
            return (node as IFigmaNodeContainer)?.children.FirstOrDefault (s => s.name == a11yNodeName);
        }

        public static bool TrySearchA11Label(this FigmaNode node, out string label)
        {
			if (TryGetPropertyValue(node.GetA11Node(), a11yLabel, out label)) {
                return true;
            }
            return false;
        }

        public static bool TrySearchA11Help(this FigmaNode node, out string label)
        {
            if (TryGetPropertyValue(node.GetA11Node(), a11yHelp, out label))
            {
                return true;
            }
            return false;
        }

        public static bool IsFigmaImageViewNode (this FigmaNode node)
		{
			if (node.name.Contains ("!image")) {
                return true;
			}
            return false;
        }

		public static void RenderInWindow(this FigmaViewRendererService sender, IWindow mainWindow, string windowLayerName, FigmaViewRendererServiceOptions options = null)
        {
            var windowFigmaNode = sender.FileProvider.FindByName (windowLayerName);

            FigmaNode content = null;

            if (windowFigmaNode is IFigmaNodeContainer figmaNodeContainer)
            {
                content = figmaNodeContainer.children.FirstOrDefault(s => s.IsNodeWindowContent());

                var windowComponent = windowFigmaNode.GetDialogInstanceFromParentContainer();

                if (options == null)
                {
                    options = new FigmaViewRendererServiceOptions();
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
