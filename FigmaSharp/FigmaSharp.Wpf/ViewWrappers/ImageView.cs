using System;
using System.Windows.Controls;
using System.Windows.Media;
using FigmaSharp.Models;
using FigmaSharp.Views;
using FigmaSharp.Wpf.Converters;

namespace FigmaSharp.Wpf
{
    public class ImageView : View, IImageView
    {
        public ImageView(CanvasImage pictureBox) : base(pictureBox)
        {
        }

        public ImageView() : this(new CanvasImage ())
        {
        }

        public void SetImage(IImageView image)
        {
            ((CanvasImage)nativeView).SetImage (image.NativeObject as ImageSource);
        } 
    }
}
