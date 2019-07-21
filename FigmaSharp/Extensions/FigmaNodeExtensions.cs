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

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System;
using LiteForms;
using FigmaSharp.Models;

namespace FigmaSharp
{
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
