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
using AppKit;
using CoreAnimation;
using CoreGraphics;
using FigmaSharp.Views.Graphics;

namespace FigmaSharp.Views.Cocoa.Graphics
{
    public class SvgNSView : NSView
    {
        public override bool IsFlipped => true;

        CAShapeLayer shapeLayer;
      
        Svg origSvg;

        public SvgNSView(string data)
        {
            Console.WriteLine(data);
            Initialize();
            Load(data);
        }

        public SvgNSView()
        {
            Initialize();
        }

        void Initialize ()
        {
            WantsLayer = true;
            
            shapeLayer = new CAShapeLayer();
            shapeLayer.MasksToBounds = true;
            Layer = shapeLayer;
        }

        public void Load (string data)
        {
            if (string.IsNullOrEmpty(data))
                return;

            Load(Svg.FromData (data));
        }

        public override void SetFrameSize(CGSize newSize)
        {
            base.SetFrameSize(newSize);
            Reload();
        }

        void Reload ()
        {
            //don't reload if no svg
            if (origSvg == null)
                return;

            //TODO: optimize
            if (shapeLayer.Sublayers != null)
            {
                foreach (var item in shapeLayer.Sublayers)
                    item.RemoveFromSuperLayer();
            }
            Load(origSvg);
        }

        public void Load (Svg svg)
        {
            origSvg = svg;

            RecursivelyAddSublayer(svg);
            if (svg.Svgs != null)
            {
                foreach (var item in svg.Svgs)
                    RecursivelyAddSublayer(item);
            }
        }

        void RecursivelyAddSublayer(Svg svg)
        {
            if (svg.Vectors == null)
                return;
            foreach (var vector in svg.Vectors)
            {
                if (vector is GPath gpath)
                    shapeLayer.AddSublayer(gpath.ToShape(svg));
                else if (vector is Path path)
                    Translate(path.ToShape(svg));
                else if (vector is CirclePath circlePath)
                    shapeLayer.AddSublayer(circlePath.ToShape(svg));
                else if (vector is RectanglePath rectanglePath)
                    shapeLayer.AddSublayer(rectanglePath.ToShape(svg));
                else if (vector is LinePath linePath)
                    shapeLayer.AddSublayer(linePath.ToShape(svg));
                else if (vector is TextPath textPath)
                    shapeLayer.AddSublayer(textPath.ToShape(svg));
            }
        }

        void Translate(CAShapeLayer layer)
        {
            shapeLayer.AddSublayer(layer);

            CGRect bounds = layer.Bounds; ;
            nfloat factorX = Frame.Width / bounds.Width;
            nfloat factorY = Frame.Height / bounds.Height;
            nfloat factor = (nfloat)Math.Min(factorX, factorY);

            nfloat width = bounds.Width * factor;
            nfloat height = bounds.Height * factor;

            var transform = CGAffineTransform.MakeTranslation(width, height);
            transform.Scale(factor, factor);
            layer.AffineTransform = transform;
        } 
    }
}
