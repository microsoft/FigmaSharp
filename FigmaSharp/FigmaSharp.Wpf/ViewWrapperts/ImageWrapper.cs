using System;
using System.Drawing;
using System.Windows.Media;

namespace FigmaSharp.Wpf
{
    public class ImageWrapper : IImageWrapper
    {
        public object NativeObject => image;

        protected ImageSource image;
        public ImageWrapper(ImageSource image)
        {
            this.image = image;
        }
    }
}
