using System;
using System.Drawing;
using System.Windows.Media;
using FigmaSharp.Models;

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
