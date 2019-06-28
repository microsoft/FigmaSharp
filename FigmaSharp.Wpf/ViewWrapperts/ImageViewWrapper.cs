using System;
using System.Windows.Controls;
using System.Windows.Media;
using FigmaSharp.Models;

namespace FigmaSharp.Wpf
{
    public class ImageViewWrapper : ViewWrapper, IImageViewWrapper
    {
        public ImageViewWrapper(Image pictureBox) : base(pictureBox)
        {
        }

        public void SetImage(IImageWrapper image)
        {
            ((Image)nativeView).Source = image.NativeObject as ImageSource;
        }
    }
}
