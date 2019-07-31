using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace FigmaSharp.WinForms
{
    public class ImageTransparentControl : TransparentControl
    {
        System.Drawing.Image image;
        public System.Drawing.Image Image
        {
            get => image;
            set
            {
                image = value;
                Invalidate();
            }
        }

        public ImageTransparentControl()
        {
            ResizeRedraw = true;
        }

        protected override void CustomDraw(Graphics g)
        {
            base.CustomDraw(g);

            if (image != null)
            {
                var destRect = new Rectangle(0, 0, Width, Height);

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
        }
    }
}
