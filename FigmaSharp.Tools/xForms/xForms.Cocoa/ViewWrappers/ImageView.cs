/* 
 * FigmaImageView.cs - NSImageView which stores it's associed Figma Id
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
using AppKit;
using CoreAnimation;
using CoreGraphics;

namespace LiteForms.Cocoa
{
	public class ImageView : View, IImageView
    {
        readonly NSImageView imageView;
        CALayer imageLayer;

        public ImageView(NSImageView imageView) : base(imageView)
        {
            this.imageView = imageView;
            this.imageView.WantsLayer = true;

            imageLayer = new CALayer();
            imageView.Layer.AddSublayer(imageLayer);
        }

        nfloat GetProportionalSecondSize (nfloat proportionalFirstSize, nfloat originalFirstSize,  nfloat originalSecondSize)
        {
            nfloat delta = proportionalFirstSize / originalFirstSize;
            return delta * originalSecondSize;
        }


        public void SetImage(IImage Image)
        {
            var image = ((NSImage)Image.NativeObject);
            imageLayer.Contents = image.CGImage;

            imageLayer.Frame = new CGRect(0, 0, Width, Height);
            imageLayer.AnchorPoint = new CGPoint(0.5f, 0.5f);
            imageLayer.Position = new CGPoint(imageView.Layer.Bounds.GetMidX(), imageView.Layer.Bounds.GetMidY());
        }
    }
}
