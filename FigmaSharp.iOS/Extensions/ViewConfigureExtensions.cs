using System;
using CoreAnimation;
using UIKit;
using System.Linq;
using Foundation;
using CoreGraphics;

namespace FigmaSharp
{
    public static class ViewConfigureExtensions
    {
        public static void Configure(this UIView view, FigmaFrameEntity child)
        {
            Configure(view, (FigmaNode)child);

            view.Alpha = child.opacity;
            view.Layer.BackgroundColor = FigmaExtensions.ToUIColor(child.backgroundColor).CGColor;
        }

        public static void Configure(this UIView view, FigmaNode child)
        {
            view.Hidden = !child.visible;
            //view.WantsLayer = true;

            if (child is IFigmaDocumentContainer container)
            {
                view.Frame = container.absoluteBoundingBox.ToCGRect ();
            }
        }

        public static void Configure(this UIView view, FigmaElipse elipse)
        {
            Configure(view, (FigmaVectorEntity)elipse);

            var circleLayer = new CAShapeLayer();
            var bezierPath = UIBezierPath.FromOval (new CoreGraphics.CGRect(0, 0, elipse.absoluteBoundingBox.width, elipse.absoluteBoundingBox.height));
            //circleLayer.Path = bezierPath.ToGCPath();

            view.Layer.AddSublayer(circleLayer);

            var fills = elipse.fills.OfType<FigmaPaint>().FirstOrDefault();
            if (fills != null)
            {
                circleLayer.FillColor = fills.color.ToUIColor().CGColor;
            }

            var strokes = elipse.strokes.FirstOrDefault();
            if (strokes != null)
            {
                if (strokes.color != null)
                {
                    circleLayer.BorderColor = strokes.color.ToUIColor().CGColor;
                }
            }
        }

        //public static CGPath ToGCPath(this UIBezierPath bezierPath)
        //{
        //    var path = new CGPath();
        //    CGPoint[] points;
        //    for (int i = 0; i < (int)bezierPath.RetainCount; i++)
        //    {
        //        var type = bezierPath. ElementAt(i, out points);
        //        switch (type)
        //        {
        //            case UIBezierPathElement.MoveTo:
        //                path.MoveToPoint(points[0]);
        //                break;
        //            case NSBezierPathElement.LineTo:
        //                path.AddLineToPoint(points[0]);
        //                break;
        //            case NSBezierPathElement.CurveTo:
        //                path.AddCurveToPoint(points[2], points[1], points[0]);
        //                break;
        //            case NSBezierPathElement.ClosePath:
        //                path.CloseSubpath();
        //                break;
        //        }
        //    }
        //    return path;
        //}

        public static void Configure(this UIView figmaLineView, FigmaLine figmaLine)
        {
            Configure(figmaLineView, (FigmaVectorEntity)figmaLine);

            var fills = figmaLine.fills.OfType<FigmaPaint>().FirstOrDefault();
            if (fills != null)
            {
                figmaLineView.Layer.BackgroundColor = fills.color.ToUIColor().CGColor;
            }

            var absolute = figmaLine.absoluteBoundingBox;
            var lineWidth = absolute.width == 0 ? figmaLine.strokeWeight : absolute.width;

            var constraintWidth = figmaLineView.WidthAnchor.ConstraintEqualTo (lineWidth);
            constraintWidth.Priority = (uint)UILayoutPriority.DefaultLow;
            constraintWidth.Active = true;

            var lineHeight = absolute.height == 0 ? figmaLine.strokeWeight : absolute.height;

            var constraintHeight = figmaLineView.HeightAnchor.ConstraintEqualTo(lineHeight);
            constraintHeight.Priority = (uint)UILayoutPriority.DefaultLow;
            constraintHeight.Active = true;
        }

        public static void Configure(this UIView view, FigmaVectorEntity child)
        {
            Configure(view, (FigmaNode)child);

            if (child.HasFills && child.fills[0].color != null)
            {
                view.Layer.BackgroundColor = child.fills[0].color.ToUIColor().CGColor;
            }

            //var currengroupView = new UIView() { TranslatesAutoresizingMaskIntoConstraints = false };
            //currengroupView.Configure(rectangleVector);

            var strokes = child.strokes.FirstOrDefault();
            if (strokes != null)
            {
                if (strokes.color != null)
                {
                    view.Layer.BorderColor = strokes.color.ToUIColor().CGColor;
                }
                view.Layer.BorderWidth = child.strokeWeight;
            }
        }

        public static void Configure(this UIView view, FigmaRectangleVector child)
        {
            Configure(view, (FigmaVectorEntity)child);

            view.Layer.CornerRadius = child.cornerRadius;
        }

        public static void Configure(this UITextField label, FigmaText text)
        {
            Configure(label, (FigmaNode)text);

            label.TextAlignment = text.style.textAlignHorizontal == "CENTER" ? UITextAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? UITextAlignment.Left : UITextAlignment.Right;
            label.Alpha = text.opacity;
            //label.l = NSLineBreakMode.ByWordWrapping;
            //label.SetContentCompressionResistancePriority(250, NSLayoutConstraintOrientation.Horizontal);

            var fills = text.fills.FirstOrDefault();
            if (fills != null)
            {
                label.TextColor = FigmaExtensions.ToUIColor(fills.color);
            }

            if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0)
            {
                var attributedText = new NSMutableAttributedString(label.AttributedText);
                for (int i = 0; i < text.characterStyleOverrides.Length; i++)
                {
                    var key = text.characterStyleOverrides[i].ToString();
                    if (!text.styleOverrideTable.ContainsKey(key))
                    {
                        continue;
                    }
                    var element = text.styleOverrideTable[key];
                    if (element.fontFamily == null)
                    {
                        continue;
                    }
                    var localFont = FigmaExtensions.ToUIFont(element);
                    var range = new NSRange(i, 1);
                    attributedText.AddAttribute(UIStringAttributeKey.Font, localFont, range);
                    attributedText.AddAttribute(UIStringAttributeKey.ForegroundColor, label.TextColor, range);
                }

                label.AttributedText = attributedText;
            }
        }
    }
}
