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
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.Views.Cocoa
{
	public class ImageView : View, IImageView
	{
		readonly NSImageView imageView;
		CALayer imageLayer;

		public ImageView () : this (new FNSImageView ())
		{

		}

		public ImageView (NSImageView imageView) : base (imageView)
		{
			this.imageView = imageView;
			this.imageView.WantsLayer = true;
            this.imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            //imageLayer = new CALayer();
            //imageLayer.BackgroundColor = NSColor.Blue.CGColor;
            //imageView.Layer.AddSublayer(imageLayer);
        }

		IImage image;
		public IImage Image {
			get { return image; }
			set {
				image = value;
				var nativeImage = (NSImage)Image.NativeObject;
				//imageView.Layer.Contents = nativeImage.CGImage;
				imageView.Image = nativeImage;
			}
		}
	}
}
