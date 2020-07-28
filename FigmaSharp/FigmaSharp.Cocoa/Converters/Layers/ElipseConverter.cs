// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
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

using System;
using System.Linq;

using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;

using FigmaSharp.Cocoa.CodeGeneration;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Cocoa.Converters
{
    public class ElipseConverter : ElipseConverterBase
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSView);

        #region View

        public override IView ConvertToView(FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var view = new View();

            CreateMember(currentNode, view);
            ConfigureProperty(Properties.Opacity, currentNode, view);
            ConfigureProperty(Properties.FillColor, currentNode, view);
            ConfigureProperty(Properties.StrokeColor, currentNode, view);
            ConfigureProperty(Properties.StrokeDashes, currentNode, view);
            ConfigureProperty(Properties.LineWidth, currentNode, view);
            return view;
        }

        public void CreateMember(FigmaNode currentNode, IView view)
        {
            var elipseView = (NSView)view.NativeObject;
            elipseView.Configure(currentNode);

            var elipseNode = (FigmaElipse)currentNode;
            var circleLayer = new CAShapeLayer();
            elipseView.Layer.AddSublayer(circleLayer);

            var bezierPath = NSBezierPath.FromOvalInRect(
                new CGRect(elipseNode.strokeWeight, elipseNode.strokeWeight,
                elipseNode.absoluteBoundingBox.Width - (elipseNode.strokeWeight * 2), elipseNode.absoluteBoundingBox.Height - ((elipseNode.strokeWeight * 2))));
            circleLayer.Path = bezierPath.ToCGPath();
        }

        public void ConfigureProperty(string propertyName, FigmaNode node, IView view)
        {
            var elipseNode = (FigmaElipse)node;
            var elipseView = (NSView)view.NativeObject;
            var circleLayer = (CAShapeLayer)elipseView.Layer.Sublayers[0];

            if (propertyName == nameof (Properties.FillColor))
            {
                //to define system colors
                var fills = elipseNode.fills.OfType<FigmaPaint>().FirstOrDefault();
                if (fills != null && fills.color != null)
                    circleLayer.FillColor = fills.color.ToCGColor();
                else
                    circleLayer.FillColor = NSColor.Clear.CGColor;
                return;
            }

            if (propertyName == nameof(Properties.StrokeColor))
            {
                var strokes = elipseNode.strokes.FirstOrDefault();
                if (strokes?.color != null)
                    circleLayer.StrokeColor = strokes.color.MixOpacity(strokes.opacity).ToNSColor().CGColor;
                return;
            }

            if (propertyName == nameof(Properties.StrokeDashes))
            {
                if (elipseNode.strokeDashes != null)
                {
                    var number = new NSNumber[elipseNode.strokeDashes.Length];
                    for (int i = 0; i < elipseNode.strokeDashes.Length; i++)
                        number[i] = elipseNode.strokeDashes[i];
                    circleLayer.LineDashPattern = number;
                }
                return;
            }

            if (propertyName == nameof(Properties.LineWidth))
            {
                circleLayer.LineWidth = elipseNode.strokeWeight;
                return;
            }

            if (propertyName == nameof(Properties.Opacity))
            {
                elipseView.AlphaValue = elipseNode.opacity;
                return;
            }
        }

        #endregion

        #region Code

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            var builder = new CocoaStringBuilder();

            CreateCodeMember(
                currentNode,
                builder,
                rendererService.NeedsRenderConstructor(currentNode, parentNode),
                rendererService.NodeRendersVar(currentNode, parentNode)
                );
            ConfigureCodeProperty(Properties.Opacity, currentNode, builder);
            ConfigureCodeProperty(Properties.FillColor, currentNode, builder);
            ConfigureCodeProperty(Properties.StrokeColor, currentNode, builder);
            ConfigureCodeProperty(Properties.StrokeDashes, currentNode, builder);
            ConfigureCodeProperty(Properties.LineWidth, currentNode, builder);

            return builder.ToString();
        }

        public void CreateCodeMember(CodeNode codeNode, CocoaStringBuilder builder, bool constructor, bool renderVar)
        {
            var viewObject = new CocoaNodeStringObject(codeNode, typeof(NSView));
            var elipseNode = (FigmaElipse)codeNode.Node;

            if (constructor)
                builder.WriteConstructor(viewObject, renderVar);

            builder.WritePropertyEquality(viewObject, nameof(NSView.WantsLayer), true);
            builder.WritePropertyEquality(viewObject, nameof(NSView.TranslatesAutoresizingMaskIntoConstraints), false);

            var circleShareLayerObject = viewObject.CreateChildStringObject("ElipseNode", typeof(CAShapeLayer));
            builder.WriteConstructor(circleShareLayerObject, includesVar: true);

            builder.WriteMethod(
                viewObject.CreatePropertyName(nameof(NSView.Layer)),
                nameof(NSView.Layer.AddSublayer),
                circleShareLayerObject);

            var rectangle = new CGRect(elipseNode.strokeWeight, elipseNode.strokeWeight,
       elipseNode.absoluteBoundingBox.Width - (elipseNode.strokeWeight * 2), elipseNode.absoluteBoundingBox.Height - (elipseNode.strokeWeight * 2));

            var bezierPathObject = new CocoaStringObject(Members.Draw.BezierPath.FromOvalInRect(rectangle), typeof(NSBezierPath));
            bezierPathObject.AddEnclose();

            builder.WriteEquality(
                circleShareLayerObject.CreatePropertyName(nameof(CAShapeLayer.Path)),
                bezierPathObject.Draw.ToCGPath()
                );
        }

        public void ConfigureCodeProperty(string propertyName, CodeNode codeNode, CocoaStringBuilder code)
        {
            var elipseNode = (FigmaElipse)codeNode.Node;

            var circleLayer = new CocoaNodeStringObject(codeNode, typeof(NSView))
                .AddChild(nameof(NSView.Layer))
                .AddArrayChild(nameof(NSView.Layer.Sublayers), 0);
            circleLayer.AddCast(typeof(CAShapeLayer));
            circleLayer.AddEnclose();

            if (propertyName == nameof(Properties.FillColor))
            {
                //to define system colors
                var fills = elipseNode.fills.OfType<FigmaPaint>().FirstOrDefault();
                if (fills != null && fills.color != null)
                    code.WritePropertyEquality(circleLayer.ToString(), nameof(CAShapeLayer.FillColor), fills.color.ToDesignerString(true));
                else
                    code.WritePropertyEquality(circleLayer.ToString(), nameof(CAShapeLayer.FillColor), Members.Colors.Clear);

                return;
            }

            if (propertyName == nameof(Properties.StrokeColor))
            {
                var strokes = elipseNode.strokes.FirstOrDefault();
                if (strokes?.color != null)
                    code.WritePropertyEquality(circleLayer.ToString (),
                        nameof(CAShapeLayer.StrokeColor),
                        strokes.color.MixOpacity(strokes.opacity).ToDesignerString(true)
                        );
                return;
            }

            if (propertyName == nameof(Properties.StrokeDashes))
            {
                if (elipseNode.strokeDashes != null)
                {
                    var number = new NSNumber[elipseNode.strokeDashes.Length];
                    for (int i = 0; i < elipseNode.strokeDashes.Length; i++)
                        number[i] = elipseNode.strokeDashes[i];

                    code.WritePropertyEquality(circleLayer.ToString (),
                       nameof(CAShapeLayer.LineDashPattern),
                       number.ToDesignerString()
                       );
                }
                return;
            }

            if (propertyName == nameof(Properties.LineWidth))
            {
                code.WritePropertyEquality(circleLayer.ToString (),
                     nameof(CAShapeLayer.LineWidth),
                     elipseNode.strokeWeight.ToString()
                     );
                return;
            }

            if (propertyName == nameof(Properties.Opacity))
            {
                code.WritePropertyEquality(codeNode.Name,
                   nameof(NSView.AlphaValue),
                   elipseNode.opacity.ToString()
                   );
                return;
            }
        }

        #endregion
    }
}
