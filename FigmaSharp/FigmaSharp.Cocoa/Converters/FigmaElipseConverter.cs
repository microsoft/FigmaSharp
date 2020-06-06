/* 
 * FigmaElipseConverter.cs 
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
using System.Text;
using System.Linq;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;
using Foundation;

namespace FigmaSharp.Cocoa.Converters
{
    public class FigmaElipseConverter : ElipseConverterBase
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSView);

        public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var elipseView = new NSView();
            elipseView.Configure(currentNode);

            var elipseNode = (FigmaElipse)currentNode;
            elipseView.AlphaValue = elipseNode.opacity;

            var circleLayer = new CAShapeLayer();
            var bezierPath = NSBezierPath.FromOvalInRect(
                new CGRect(elipseNode.strokeWeight, elipseNode.strokeWeight,
                elipseNode.absoluteBoundingBox.Width - (elipseNode.strokeWeight*2), elipseNode.absoluteBoundingBox.Height - ((elipseNode.strokeWeight * 2))));
            circleLayer.Path = bezierPath.ToCGPath();

            elipseView.Layer.AddSublayer(circleLayer);
             
            //to define system colors
            var fills = elipseNode.fills.OfType<FigmaPaint>().FirstOrDefault();
            if (fills != null && fills.color != null)
                circleLayer.FillColor = fills.color.ToCGColor();
            else
                circleLayer.FillColor = NSColor.Clear.CGColor;

            var strokes = elipseNode.strokes.FirstOrDefault();
            if (strokes?.color != null)
                circleLayer.StrokeColor = strokes.color.MixOpacity(strokes.opacity).ToNSColor().CGColor;

            if (elipseNode.strokeDashes != null)
            {
                var number = new NSNumber[elipseNode.strokeDashes.Length];
                for (int i = 0; i < elipseNode.strokeDashes.Length; i++)
                    number[i] = elipseNode.strokeDashes[i];
                circleLayer.LineDashPattern = number;
            }

            circleLayer.LineWidth = elipseNode.strokeWeight;
          
            return new View (elipseView);
        }

        public override string ConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            StringBuilder builder = new StringBuilder();

            if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
                builder.WriteConstructor (currentNode.Name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

            builder.Configure(currentNode.Node, currentNode.Name);

            return builder.ToString();
        }
    }
}
