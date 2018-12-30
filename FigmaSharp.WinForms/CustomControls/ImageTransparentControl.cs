using System.Drawing;

namespace FigmaSharp
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
            }
        }

        public ImageTransparentControl()
        {
        }

        protected override void CustomDraw(Graphics g)
        {
            base.CustomDraw(g);

            if (image != null)
            {
                g.DrawImage(image, Location.X, Location.Y);
            }
        }
    }
}
