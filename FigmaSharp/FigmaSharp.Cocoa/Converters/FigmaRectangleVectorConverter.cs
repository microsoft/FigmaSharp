/* 
 * RectangleVectorConverter.cs
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
using System.Text;
using AppKit;

using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Cocoa.Converters
{
    public class FigmaRectangleVectorConverter : RectangleVectorConverterBase
    {
        public override Type GetControlType(FigmaNode currentNode)
            => typeof(NSView);

        public override IView ConvertTo(FigmaNode currentNode, ViewNode parent, FigmaRendererService rendererService)
        {
            var vectorEntity = (RectangleVector)currentNode;
            IView view;
            if (rendererService.FileProvider.RendersAsImage (currentNode))
                view = new ImageView();
            else
                view = new View();

            var currengroupView = (NSView) view.NativeObject;
            currengroupView.Configure (currentNode);

            if (vectorEntity.HasFills) {
                foreach (var fill in vectorEntity.fills) {
                    if (fill.type == "IMAGE") {
                        //we need to add this to our service
                    } else if (fill.type == "SOLID") {
                        if (fill.visible && fill.color != null) {
                            currengroupView.Layer.BackgroundColor = fill.color.ToCGColor (fill.opacity);
                        }
                    } else {
                        Console.WriteLine ($"NOT IMPLEMENTED FILL : {fill.type}");
                    }
                }
            }

            currengroupView.Layer.CornerRadius = vectorEntity.cornerRadius;
          
            var stroke = vectorEntity.strokes?.FirstOrDefault ();
            if (stroke != null) {
                currengroupView.Layer.BorderWidth = vectorEntity.strokeWeight;
                if (stroke.visible && stroke.color != null) {
                    currengroupView.Layer.BorderColor = stroke.color.ToCGColor (stroke.opacity);
                }
            }
            //view.layer.borderColor = UIColor (red: 1, green: 1, blue: 1, alpha: 1).cgColor
            return view;
        }

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            StringBuilder builder = new StringBuilder();

            if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
                builder.WriteConstructor (currentNode.Name, GetControlType (currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

            builder.Configure((RectangleVector)currentNode.Node, Resources.Ids.Conversion.NameIdentifier);
            return builder.ToString();
        }
    }
}
