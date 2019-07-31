using System;
using System.Windows.Controls;
using System.Windows.Media;
using FigmaSharp.Models;
using FigmaSharp.Wpf.Converters;

namespace FigmaSharp.Wpf
{
    public class ImageViewWrapper : ViewWrapper, IImageViewWrapper
    {
        public ImageViewWrapper(CanvasImage pictureBox) : base(pictureBox)
        {
        }

        public ImageViewWrapper() : this(new CanvasImage ())
        {
        }

        public void SetImage(IImageWrapper image)
        {
            ((CanvasImage)nativeView).SetImage (image.NativeObject as ImageSource);
        }
    }
}
