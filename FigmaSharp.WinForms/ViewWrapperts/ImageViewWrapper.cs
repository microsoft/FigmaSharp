using System;
using System.Windows.Forms;

namespace FigmaSharp
{
    public class ImageViewWrapper : ViewWrapper, IImageViewWrapper
    {
        public ImageViewWrapper(PictureBox pictureBox) : base(pictureBox)
        {
        }

        public FigmaPaint Data { get; set; }

        public void SetImage(IImageWrapper image)
        {
            ((PictureBox)nativeView).Image = image.NativeObject as System.Drawing.Image;
        }
    }
}
