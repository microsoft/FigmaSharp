using FigmaSharp.Cocoa;
using FigmaSharp.Views;

namespace FigmaSharp.Cocoa
{
	public class ExtendedView : FigmaSharp.Cocoa.View
	{
		public float BorderRadius
		{
			get
			{
				if (nativeView.Layer is CoreAnimation.CAGradientLayer gradientLayer)
				{
					if (gradientLayer.Mask is CoreAnimation.CAShapeLayer shapeLayer)
						return (float)shapeLayer.CornerRadius;
				}
				else if (nativeView.Layer is CoreAnimation.CAShapeLayer shapeLayer)
					return (float)shapeLayer.CornerRadius;

				return (float)nativeView.Layer.CornerRadius;
			}
			set
			{
				if (nativeView.Layer is CoreAnimation.CAGradientLayer gradientLayer)
				{
					if (gradientLayer.Mask is CoreAnimation.CAShapeLayer shapeLayer)
						shapeLayer.CornerRadius = value;
				}
				else if (nativeView.Layer is CoreAnimation.CAShapeLayer shapeLayer)
					shapeLayer.CornerRadius = value;
				else
					nativeView.Layer.CornerRadius = value;
			}
		}

		public Color BorderColor
		{
			get
			{
				if (nativeView.Layer is CoreAnimation.CAGradientLayer gradientLayer)
				{
					if (gradientLayer.Mask is CoreAnimation.CAShapeLayer shapeLayer)
						return shapeLayer.StrokeColor.ToColor();
				}
				else if (nativeView.Layer is CoreAnimation.CAShapeLayer shapeLayer)
					return shapeLayer.StrokeColor.ToColor();

				return nativeView.Layer.BorderColor.ToColor();
			}
			set
			{
				if (nativeView.Layer is CoreAnimation.CAGradientLayer gradientLayer)
				{
					if (gradientLayer.Mask is CoreAnimation.CAShapeLayer shapeLayer)
						shapeLayer.StrokeColor = value.ToCGColor();
				}
				else if (nativeView.Layer is CoreAnimation.CAShapeLayer shapeLayer)
					shapeLayer.StrokeColor = value.ToCGColor();
				else
					nativeView.Layer.BorderColor = value.ToCGColor();
			}
		}

		public float BorderWidth
		{
			get
			{
				if (nativeView.Layer is CoreAnimation.CAGradientLayer gradientLayer)
				{
					if (gradientLayer.Mask is CoreAnimation.CAShapeLayer shapeLayer)
						return (float)shapeLayer.LineWidth;
				}
				else if (nativeView.Layer is CoreAnimation.CAShapeLayer shapeLayer)
					return (float)shapeLayer.LineWidth;

				return (float)nativeView.Layer.BorderWidth;
			}
			set
			{
				if (nativeView.Layer is CoreAnimation.CAGradientLayer gradientLayer)
				{
					if (gradientLayer.Mask is CoreAnimation.CAShapeLayer shapeLayer)
						shapeLayer.LineWidth = value;
				}
				else if (nativeView.Layer is CoreAnimation.CAShapeLayer shapeLayer)
					shapeLayer.LineWidth = value;
				else
					nativeView.Layer.BorderWidth = value;
			}
		}

		public void SetGradientLayer(FigmaSharp.Views.Color from, FigmaSharp.Views.Color to)
		{
			var gradientLayer = new CoreAnimation.CAGradientLayer();
			gradientLayer.Colors = new CoreGraphics.CGColor[] {
				from.ToCGColor (), to.ToCGColor ()
			};
			gradientLayer.StartPoint = new CoreGraphics.CGPoint(0.0, 0.5);
			gradientLayer.EndPoint = new CoreGraphics.CGPoint(1.0, 0.5);

			//var shapeLayer = new CoreAnimation.CAShapeLayer();
			//shapeLayer.Path = AppKit.NSBezierPath.FromRect(this.nativeView.Bounds).ToCGPath();
			//shapeLayer.FillColor = AppKit.NSColor.Clear.CGColor;
			//shapeLayer.StrokeColor = AppKit.NSColor.Clear.CGColor;
			//gradientLayer.Mask = shapeLayer;

			//var borderWidth = BorderWidth;
			//var borderColor = BorderColor;
			//var borderRadius = BorderRadius;

			this.nativeView.Layer = gradientLayer;

			//BorderWidth = borderWidth;
			////BorderColor = borderColor;
			//BorderRadius = borderRadius;
		}

		public override void OnChangeFrameSize(Size newSize)
		{
			base.OnChangeFrameSize(newSize);

			if (nativeView.Layer is CoreAnimation.CAGradientLayer gradientLayer)
			{
				SetGradientLayer(gradientLayer.Colors[0].ToColor(), gradientLayer.Colors[1].ToColor());
			}
		}
	}

	//public class Transition
	//{
	//	public TransitionType TransitionType { get; set; }
	//	public float Stiffness { get; set; }
	//	public float Damping { get; set; }

	//	public TransitionBehaviour Behaviour { get; set; }
	//	public float StaggerChildren { get; set; }
	//}

}

