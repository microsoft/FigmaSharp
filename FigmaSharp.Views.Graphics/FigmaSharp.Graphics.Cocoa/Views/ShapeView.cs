using FigmaSharp.Views;
using NGraphics;

namespace FigmaSharp.Graphics.Mac
{
	public abstract class ShapeView : FigmaSharp.Cocoa.ImageView, IShapeView
	{
        IImageCanvas canvas;

		public FigmaSharp.Views.Color Color { get; set; } = FigmaSharp.Views.Color.Black;
		public FigmaSharp.Views.Color BorderColor { get; set; } = FigmaSharp.Views.Color.Black;

		public ShapeView()
        {
            var size = new NGraphics.Size(1, 1);
            canvas = Platforms.Current.CreateImageCanvas(size, scale: 2);
            Refresh(size);
        }

		public ShapeView(AppKit.NSImageView imageView) : base (imageView)
		{
			var size = new NGraphics.Size(1, 1);
			canvas = Platforms.Current.CreateImageCanvas(size, scale: 2);
			Refresh(size);
		}

		internal void RefreshDraw()
		{
			Refresh(canvas.Size);
		}

		public override void OnChangeFrameSize(FigmaSharp.Views.Size newSize)
        {
			var size = newSize.ToNGraphicSize();
			canvas = Platforms.Current.CreateImageCanvas(size, scale: 2);
            Refresh(size);
			base.OnChangeFrameSize(newSize);
        }

        void Refresh (NGraphics.Size size)
        {
            OnDraw(canvas);

            var imageNative = canvas.GetImage().GetNSImage();
			imageNative.Size = size.ToCGSize();
            Image = new FigmaSharp.Cocoa.Image(imageNative);
        }

        protected abstract void OnDraw(IImageCanvas canvas);
    }
}
