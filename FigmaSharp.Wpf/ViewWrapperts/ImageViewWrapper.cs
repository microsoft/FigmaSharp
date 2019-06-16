using System;
using System.Windows.Controls;
using System.Windows.Media;

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
