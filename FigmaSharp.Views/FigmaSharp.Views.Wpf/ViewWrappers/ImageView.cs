using System;
using System.Windows.Controls;
using System.Windows.Media;
using FigmaSharp.Models;
using FigmaSharp.Views; 

namespace FigmaSharp.Views.Wpf
{
    public class ImageView : View, IImageView
    {
        readonly System.Windows.Controls.Image imageView;
         
        public ImageView(System.Windows.Controls.Image imageView) : base(imageView)
        {
        }

        public ImageView() : this(new System.Windows.Controls.Image())
        {
        }

        IImage image;
        public IImage Image 
        {
            get => image;
            set
            {
                image = value;
                var nativeImage = (ImageSource)Image.NativeObject;
                imageView.Source = nativeImage;
            }
        }
    }
}
