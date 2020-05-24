using System;
using System.Drawing;
using System.Windows.Media;
using FigmaSharp.Models;
using FigmaSharp.Views;

namespace FigmaSharp.Views.Wpf
{
    public class Image : IImage
    {
        public object NativeObject => image;

        public Size Size
        {
            get => new Size((int)image.Width, (int)image.Height);
            set
            { 
                // not possible :(
                //image.Width = value.Width;
                //image.Height = value.Height;
            }
        }

        ImageSource image;
        public Image(ImageSource image)
        {
            this.image = image;
        }

        public void Dispose()
        {
        }
    }
}
