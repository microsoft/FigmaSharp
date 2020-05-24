/* 
 * FigmaFrameEntityConverter.cs
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

using FigmaSharp.Converters; 
using FigmaSharp.Models;
using FigmaSharp.Views;
using FigmaSharp.Services;
using System.Windows;
using System;
using FigmaSharp.Views.Wpf;

namespace FigmaSharp.Wpf.Converters
{
    public class FigmaFrameEntityConverter : FigmaFrameEntityConverterBase
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(CanvasImage);

        public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            IView view;
            if (rendererService.ProcessesImageFromNode(currentNode))
                view = new ImageView();
            else
                view = new View();

            var currengroupView = view.NativeObject as FrameworkElement;
            var figmaFrameEntity = (FigmaFrameEntity)currentNode;
            currengroupView.Configure(currentNode);
            
            // TODO: Resolve alpha, background color
            //currengroupView.AlphaValue = figmaFrameEntity.opacity;

            if (figmaFrameEntity.HasFills)
            {
                foreach (var fill in figmaFrameEntity.fills)
                {
                    if (fill.type == "IMAGE")
                    {
                        //we need to add this to our service
                    }
                    else if (fill.type == "SOLID")
                    {
                        if (fill.visible)
                        {
                            //currengroupView.Layer.BackgroundColor = fill.color.ToCGColor();
                        }
                    }
                    else
                    {
                        Console.WriteLine($"NOT IMPLEMENTED FILL : {fill.type}");
                    }
                    //currengroupView.Layer.Hidden = !fill.visible;
                }
            }

            return view; 
        }
         
        public override string ConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            return string.Empty;
        }
    }
}
