// Authors:
//   jmedrano <josmed@microsoft.com>
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
using System;
using System.Xml.Linq;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using FigmaSharp.Views.Graphics;

namespace FigmaSharp.Views.Cocoa.Graphics
{
    public class SvgFile : NSView
    {
        public override bool IsFlipped => true;

        CAShapeLayer shapeLayer;

        Svg svg;

        public SvgFile(string data)
        {
            Initialize();
            Load(data);
        }

        public SvgFile()
        {
            Initialize();
        }

        void Initialize ()
        {
            WantsLayer = true;
            shapeLayer = new CAShapeLayer();
            Layer = shapeLayer;
        }

        public void Load (string data)
        {
            Load(Svg.FromData (data));
        }

        public override CGSize IntrinsicContentSize => new CGSize(this.svg.Width, this.svg.Height);

        void ProcessLoad (Svg svg)
        {
            if (svg.Vectors == null)
                return;
            foreach (var vector in svg.Vectors)
            {
                if (vector is GPath gpath)
                    shapeLayer.AddSublayer(gpath.ToShape());
                else if (vector is Path path)
                    shapeLayer.AddSublayer(path.ToShape());
                else if (vector is CirclePath circlePath)
                    shapeLayer.AddSublayer(circlePath.ToShape());
            }
        }

        public void Load (Svg svg)
        {
            this.svg = svg;

            ProcessLoad(svg);

            if (svg.Svgs != null)
            {
                foreach (var item in svg.Svgs)
                    ProcessLoad(item);
            }
          

            //if (xml.Root.Name.LocalName != "svg")
            //    throw new Exception("not svg");

            //var width = xml.Root.GetWidth();
            //var heigh = xml.Root.GetHeight();
            //var viewBox = xml.Root.GetViewBox();


            //Layer.BackgroundColor = xml.Root.GetFill().CGColor;

            //foreach (var node in xml.Root.Elements())
            //{
            //    if (node.Name.LocalName == "path")
            //    {
            //        var shape = new CAShapeLayer();
            //        shape.Path = PathBuilder.Build(node.Attribute("d").Value);
            //        shape.FillColor = NSColor.Red.CGColor; // node.GetFill().CGColor;

            //        var stroke = node.Attribute("stroke")?.Value;
            //        if (string.IsNullOrEmpty(stroke))
            //            shape.StrokeColor = XExtensions.ConvertToNSColor(stroke).CGColor;

            //        shape.LineWidth = 2; // node.GetStrokeWidth();

            //        shapeLayer.AddSublayer(shape);

            //        continue;
            //    }
            //}
        }
    }
}
