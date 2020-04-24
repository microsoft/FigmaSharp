/* 
 * FigmaExtensions.cs - Extension methods for figma models
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
using System.Text;

using FigmaSharp.Models;

namespace FigmaSharp
{
	public static class FigmaServiceExtensions
	{
        public static bool HasChildrenVisible (this IFigmaNodeContainer optionsNode, string layerName)
        {
            return optionsNode.children.FirstOrDefault(s => s.name == layerName)
                ?.visible ?? false;
        }

        public static bool HasChildrenVisible(this FigmaNode figmaNode, string layerName)
        {
            return figmaNode is IFigmaNodeContainer nodeContainer && nodeContainer.HasChildrenVisible(layerName);
        }

        public static IEnumerable<FigmaFileVersion> GroupByCreatedAt (this IEnumerable<FigmaFileVersion> sender)
        {
            return sender.GroupBy(x => x.created_at)
                        .Select(group => group.First());
        }

        public static Rectangle GetCurrentBounds (this FigmaCanvas canvas)
        {
            Rectangle contentRect = Rectangle.Zero;
            for (int i = 0; i < canvas.children.Length; i++) {
                if (canvas.children[i] is IAbsoluteBoundingBox box) {
                    if (i == 0) {
                        contentRect = box.absoluteBoundingBox;
                    } else {
                        contentRect = contentRect.UnionWith (box.absoluteBoundingBox);
                    }
                }
            }
            return contentRect;
        }

        public static FigmaCanvas GetCurrentCanvas (this FigmaNode node)
        {
            if (node.Parent is FigmaCanvas figmaCanvas)
                return figmaCanvas;

            if (node.Parent == null)
                return null;

            return GetCurrentCanvas (node.Parent);
        }

        public static void AppendLineIfValue (this StringBuilder sender, string value)
        {
            if (!string.IsNullOrEmpty (value))
                sender.AppendLine (value);
        }

        public static bool IsVectorImage (this FigmaVectorEntity node)
		{
            return node.fills.OfType<FigmaPaint> ().Any (s => s.type == "IMAGE" && !string.IsNullOrEmpty (s.imageRef));
		}

        public static bool IsImageNode (this FigmaNode node)
        {
			if (node.GetType () == typeof (FigmaVectorEntity)) {
                return true;
			}
			if (node is FigmaVectorEntity vectorEntity && vectorEntity.IsVectorImage ()) {
                return true;
			}
            return false;
        }

        public static string GetNodeTypeName (this FigmaNode node)
		{
            var name = node.name;
            if (name.Length > 0 && name[0] == '!') {
                name = name.Substring(1);
            }
            var index = name.IndexOf (' ');
            if (index > -1 && index < name.Length - 1) {
                name = name.Substring (0,index);
            }
            return name;
        }

		static string RemoveIllegalCharacters (string name)
		{
            name = name.Replace ("-", "");
            return name;
		}

        public static bool TryGetCodeViewName (this FigmaNode node, out string customName)
        {
            if (node.TryGetNodeCustomName (out customName)) {
                customName = RemoveIllegalCharacters (customName);
                return true;
            };
            customName = RemoveIllegalCharacters (node.name);
            return false;
        }

        public static string GetClassName (this FigmaNode node)
        {
            var name = node.name;
            var index = name.IndexOf ('\"');
            if (index > -1 && index < name.Length - 1) {
                name = name.Substring (index + 1);
                index = name.IndexOf ('\"');
                if (index > -1 && index < name.Length) {
                    name = name.Substring (0, index);

                    return name
                .Replace (" ", string.Empty)
                .Replace (".", string.Empty);
                }
            }

            name = node.name;
            //HACK: we need to fix this
            index = name.IndexOf (' ');
            if (index > -1 && index < name.Length - 1) {
                name = name.Substring (index + 1);

				//names cannot be only integers, we get default name
				if (int.TryParse (name, out _))
                    name = node.name;
            }

            return name
                .Replace (" ", string.Empty)
                .Replace (".", string.Empty);
        }

        public static T FindNativeViewByName<T>(this Services.FigmaRendererService rendererService, string name)
		{
			foreach (var node in rendererService.NodesProcessed)
			{
				if (node.View.NativeObject is T && node.FigmaNode.name == name)
				{
					return (T)node.View.NativeObject;
				}
			}
			return default(T);
		}

		public static IEnumerable<T> FindNativeViewsByName<T>(this Services.FigmaRendererService rendererService, string name)
		{
			foreach (var node in rendererService.NodesProcessed)
			{
				if (node.View.NativeObject is T && node.FigmaNode.name == name)
				{
					yield return (T)node.View.NativeObject;
				}
			}
		}

		public static IEnumerable<T> FindNativeViewsStartsWith<T>(this Services.FigmaRendererService rendererService, string name, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
		{
			foreach (var node in rendererService.NodesProcessed)
			{
				if (node.View.NativeObject is T && node.FigmaNode.name.StartsWith(name, stringComparison))
				{
					yield return (T)node.View.NativeObject;
				}
			}
		}
	}

    public static class FigmaNodeExtensions
    {
        public static void CalculateBounds(this IFigmaNodeContainer figmaNodeContainer)
        {
            if (figmaNodeContainer is IAbsoluteBoundingBox calculatedBounds)
            {
                calculatedBounds.absoluteBoundingBox = Rectangle.Zero;
                foreach (var item in figmaNodeContainer.children)
                {
                    if (item is IAbsoluteBoundingBox itmBoundingBox)
                    {
                        calculatedBounds.absoluteBoundingBox.X = Math.Min(calculatedBounds.absoluteBoundingBox.X, itmBoundingBox.absoluteBoundingBox.X);
                        calculatedBounds.absoluteBoundingBox.Y = Math.Min(calculatedBounds.absoluteBoundingBox.Y, itmBoundingBox.absoluteBoundingBox.Y);

                        if (itmBoundingBox.absoluteBoundingBox.X + itmBoundingBox.absoluteBoundingBox.Width > calculatedBounds.absoluteBoundingBox.X + calculatedBounds.absoluteBoundingBox.Width)
                        {
                            calculatedBounds.absoluteBoundingBox.Width += (itmBoundingBox.absoluteBoundingBox.X + itmBoundingBox.absoluteBoundingBox.Width) - (calculatedBounds.absoluteBoundingBox.X + calculatedBounds.absoluteBoundingBox.Width);
                        }

                        if (itmBoundingBox.absoluteBoundingBox.Y + itmBoundingBox.absoluteBoundingBox.Height > calculatedBounds.absoluteBoundingBox.Y + calculatedBounds.absoluteBoundingBox.Height)
                        {
                            calculatedBounds.absoluteBoundingBox.Height += (itmBoundingBox.absoluteBoundingBox.Y + itmBoundingBox.absoluteBoundingBox.Height) - (calculatedBounds.absoluteBoundingBox.Y + calculatedBounds.absoluteBoundingBox.Height);
                        }
                    }
                }
            }
        }

		public static IEnumerable<FigmaVectorEntity> OfTypeImage(this FigmaNode child)
        {
            if (child.GetType () != typeof (FigmaText) && child is FigmaVectorEntity figmaVectorEntity)
            {
                yield return figmaVectorEntity;
            }

            if (child is IFigmaNodeContainer nodeContainer)
            {
                foreach (var item in nodeContainer.children)
                {
                    foreach (var resultItems in OfTypeImage(item))
                    {
                        yield return resultItems;
                    }
                }
            }
        }

        ////TODO: Change to async multithread
        //public static async Task SaveFigmaImageFiles(this FigmaPaint[] paints, string fileId, string directoryPath, string format = ".png")
        //{
        //    var ids = paints.Select(s => s.ID).ToArray();
        //    var query = new FigmaImageQuery(AppContext.Current.Token, fileId, ids);
        //    var images = FigmaApiHelper.GetFigmaImage(query);
        //    if (images != null)
        //    {
        //        var urls = paints.Select(s => images.images[s.ID]).ToArray();
        //        FileHelper.SaveFiles(directoryPath, format, urls);
        //    }
        //}

        public static IEnumerable<FigmaNode> FindImageNodes (this FigmaNode sender, Func<FigmaNode, bool> condition = null)
        {
			if (sender is IFigmaImage && (condition == null || condition (sender))) {
                yield return sender;
			}

            if (sender is IFigmaNodeContainer container) {
                foreach (var child in container.children) {
                    foreach (var image in child.FindImageNodes ()) {
                        yield return image;
                    }
                }
            } else if (sender is FigmaDocument document) {
                foreach (var child in document.children) {
                    foreach (var image in child.FindImageNodes ()) {
                        yield return image;
                    }
                }
            }
        }

        public static IEnumerable<FigmaNode> FindNode (this FigmaNode sender, Func<FigmaNode, bool> condition)
        {
            if (condition (sender)) {
                yield return sender;
            }

            if (sender is IFigmaNodeContainer container) {
                foreach (var child in container.children) {
                    foreach (var image in child.FindNode (condition)) {
                        yield return image;
                    }
                }
            } else if (sender is FigmaDocument document) {
                foreach (var child in document.children) {
                    foreach (var image in child.FindNode (condition)) {
                        yield return image;
                    }
                }
            }
        }

        public static Rectangle GetBoundRectangle (this IEnumerable<FigmaNode> customViews, bool includeHidden = false)
        {
            Rectangle point = null;
            FigmaNode item;
			for (int i = 0; i < customViews.Count (); i++) {
                item = customViews.ElementAt(i);
                if (!includeHidden && !item.visible)
                    continue;
                if (item is IAbsoluteBoundingBox boundingBox) {
                    if (point == null) {
                        point = boundingBox.absoluteBoundingBox.Copy();
                    } else {
                        if (boundingBox.absoluteBoundingBox.Left < point.X)
                            point.X = boundingBox.absoluteBoundingBox.Left;
                        if (boundingBox.absoluteBoundingBox.Top < point.Y)
                            point.Y = boundingBox.absoluteBoundingBox.Top;

                        if (boundingBox.absoluteBoundingBox.Right > point.Right)
                            point.Width = boundingBox.absoluteBoundingBox.Right - point.Left;
                        if (boundingBox.absoluteBoundingBox.Bottom > point.Bottom)
                            point.Height = boundingBox.absoluteBoundingBox.Bottom - point.Top;
                    }
                }
			}
            return point;
        }

        public static void Recursively(this FigmaNode[] customViews, string filter, List<FigmaNode> viewsFound)
        {
            foreach (var item in customViews)
            {
                Recursively(item, filter, viewsFound);
            }
        }

        public static void Recursively(this FigmaNode customView, string filter, List<FigmaNode> viewsFound)
        {
            if (customView.name == filter)
            {
                viewsFound.Add(customView);
            }

            if (customView is IFigmaNodeContainer container)
            {
                foreach (var item in container.children)
                {
                    Recursively(item, filter, viewsFound);
                }
            }
        }
    }
}
