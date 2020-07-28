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

using AppKit;

using FigmaSharp.Views.Cocoa.Graphics;
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.Views.Cocoa
{
	public class ImageView : View, IImageView
	{
		readonly NSImageView imageView;
		//CALayer imageLayer;

        PathScaling Scaling = PathScaling.AspectFill;

        public ImageView () : this (new FNSImageView ())
		{

		}

		public ImageView (NSImageView imageView) : base (imageView)
		{
			this.imageView = imageView;
			//this.imageView.WantsLayer = true;
            this.imageView.TranslatesAutoresizingMaskIntoConstraints = false;
            //imageLayer = new CALayer();
        }

		//void Refresh(CGRect bounds)
		//{
  //          //imageLayer.BackgroundColor = NSColor.Blue.CGColor;
  //          if (Scaling == PathScaling.AspectFit)
  //          {
  //              nfloat factorX = imageView.Frame.Width / bounds.Width;
  //              nfloat factorY = imageView.Frame.Height / bounds.Height;
  //              nfloat factor = (nfloat)Math.Min(factorX, factorY);

  //              nfloat width = bounds.Width * factor;
  //              nfloat height = bounds.Height * factor;
  //              nfloat translateX = (imageView.Frame.Width - width) / 2f;
  //              nfloat translateY = (imageView.Frame.Height - height) / 2f;

  //              var transform = CGAffineTransform.MakeTranslation(-bounds.X, -bounds.Y);
  //              transform.Translate(translateX, translateY);
  //              transform.Scale(factor, factor);
  //              imageLayer.AffineTransform = transform;
  //          }
  //          else if (Scaling == PathScaling.AspectFill)
  //          {
  //              nfloat factorX = imageView.Frame.Width / bounds.Width;
  //              nfloat factorY = imageView.Frame.Height / bounds.Height;
  //              nfloat factor = (nfloat)Math.Max(factorX, factorY);

  //              nfloat width = bounds.Width * factor;
  //              nfloat height = bounds.Height * factor;
  //              nfloat translateX = (imageView.Frame.Width - width) / 2f;
  //              nfloat translateY = (imageView.Frame.Height - height) / 2f;

  //              var transform = CGAffineTransform.MakeTranslation(-bounds.X, -bounds.Y);
  //              transform.Translate(translateX, translateY);
  //              transform.Scale(factor, factor);
  //              imageLayer.AffineTransform = transform;
  //          }
  //          else if (Scaling == PathScaling.Fill)
  //          {
  //              var factorX = imageView.Frame.Width / bounds.Width;
  //              var factorY = imageView.Frame.Height / bounds.Height;
  //              var transform = CGAffineTransform.MakeScale(factorX, factorY);

  //              var translateX = bounds.X * factorX;
  //              var translateY = bounds.Y * factorY;
  //              transform.Translate(translateX, translateY);
  //              imageLayer.AffineTransform = transform;
  //          }
  //          else
  //          {
  //              var width = bounds.Width;
  //              var height = bounds.Height;
  //              var translateX = (imageView.Frame.Width - width) / 2;
  //              var translateY = (imageView.Frame.Height - height) / 2;

  //              var transform = CGAffineTransform.MakeTranslation(-bounds.X, -bounds.Y);
  //              transform.Translate(translateX, translateY);
  //              imageLayer.AffineTransform = transform;
  //          }
  //          //imageView.Layer.AddSublayer(imageLayer);
  //      }


        IImage image;
		public IImage Image {
			get { return image; }
			set {
				image = value;
				var nativeImage = (NSImage)Image.NativeObject;
				//imageView.Layer.Contents = nativeImage.CGImage;

    //            Refresh(new CGRect(0, 0, nativeImage.CGImage.Width, nativeImage.CGImage.Height));
                imageView.Image = nativeImage;
            }
		}
	}
}
