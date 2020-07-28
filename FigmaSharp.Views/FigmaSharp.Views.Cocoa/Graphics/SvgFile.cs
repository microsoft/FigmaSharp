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

using AppKit;
using CoreAnimation;
using CoreGraphics;

using FigmaSharp.Views.Graphics;

namespace FigmaSharp.Views.Cocoa.Graphics
{
    public enum PathScaling
    {
        AspectFit,
        AspectFill,
        Fill,
        None
    }

    public class SvgNSView : NSView
    {
        public override bool IsFlipped => true;

        CAShapeLayer shapeLayer;
      
        Svg origSvg;

        PathScaling scaling;
        public PathScaling Scaling {
            get => scaling;
            set
            {
                scaling = value;
                Reload();
            }
        }

        public SvgNSView(string data)
        {
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
                    Add(gpath.ToShape());
                else if (vector is Path path)
                    Add(path.ToShape());
                else if (vector is CirclePath circlePath)
                    Add(circlePath.ToShape());
                else if (vector is RectanglePath rectanglePath)
                    Add(rectanglePath.ToShape());
                else if (vector is LinePath linePath)
                    Add(linePath.ToShape());
                else if (vector is TextPath textPath)
                    Add(textPath.ToShape());
            }
        }

        void Add (CALayer layer)
        {
            shapeLayer.AddSublayer(layer);

            if (layer is CAShapeLayer sh && sh.Path != null)
            {
                var bounds = sh.Path.BoundingBox;
               
                if (Scaling == PathScaling.AspectFit)
                {
                    nfloat factorX = Frame.Width / bounds.Width;
                    nfloat factorY = Frame.Height / bounds.Height;
                    nfloat factor = (nfloat) Math.Min(factorX, factorY);

                    nfloat width = bounds.Width * factor;
                    nfloat height = bounds.Height * factor;
                    nfloat translateX = (Frame.Width - width) / 2f;
                    nfloat translateY = (Frame.Height - height) / 2f;

                    var transform = CGAffineTransform.MakeTranslation(-bounds.X, -bounds.Y);
                    transform.Translate(translateX, translateY);
                    transform.Scale (factor, factor);
                    sh.AffineTransform = transform;
                }
                else if (Scaling == PathScaling.AspectFill)
                {
                    nfloat factorX = Frame.Width / bounds.Width;
                    nfloat factorY = Frame.Height / bounds.Height;
                    nfloat factor = (nfloat) Math.Max(factorX, factorY);

                    nfloat width = bounds.Width * factor;
                    nfloat height = bounds.Height * factor;
                    nfloat translateX = (Frame.Width - width) / 2f;
                    nfloat translateY = (Frame.Height - height) / 2f;

                    var transform = CGAffineTransform.MakeTranslation (-bounds.X, -bounds.Y);
                    transform.Translate(translateX, translateY, 0);
                    transform.Scale(factor, factor, 0);
                    sh.AffineTransform = transform;
                }
                else if (Scaling == PathScaling.Fill)
                {
                    var factorX = Frame.Width / bounds.Width;
                    var factorY = Frame.Height / bounds.Height;
                    var transform = CGAffineTransform.MakeScale (factorX, factorY);

                    var translateX = bounds.X * factorX;
                    var translateY = bounds.Y * factorY;
                    transform.Translate(translateX, translateY);
                    sh.AffineTransform = transform;
                }
                else
                {
                    var width = bounds.Width;
                    var height = bounds.Height;
                    var translateX = (Frame.Width - width) / 2;
                    var translateY = (Frame.Height - height) / 2;

                    var transform = CGAffineTransform.MakeTranslation (-bounds.X, -bounds.Y);
                    transform.Translate(translateX, translateY);
                    sh.AffineTransform = transform;
                }
            }
        }
    }
}
