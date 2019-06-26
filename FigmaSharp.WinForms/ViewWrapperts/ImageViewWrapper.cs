using System;
using System.Windows.Forms;

namespace FigmaSharp.WinForms
{
    public class ImageViewWrapper : ViewWrapper, IImageViewWrapper
    {
        public ImageViewWrapper(ImageTransparentControl pictureBox) : base(pictureBox)
        {
        }

        public void SetImage(IImageWrapper image)
        {
            ((ImageTransparentControl)nativeView).Image = image.NativeObject as System.Drawing.Image;
        }
    }
}
