using System;
using System.Linq;
using System.Windows.Forms;

namespace FigmaSharp
{
    public static class ViewConfigureExtensions
    {
        public static void Configure(this TransparentControl view, FigmaFrameEntity child)
        {
            Configure(view, (FigmaNode)child);
            view.Opacity = child.opacity;
            view.BackColor = FigmaExtensions.ToColor (child.backgroundColor);
        }

        public static void Configure(this Control view, FigmaNode child)
        {
            view.Visible = child.visible;

            if (child is IAbsoluteBoundingBox container)
            {
                view.Width = (int) container.absoluteBoundingBox.width;
                view.Height = (int) container.absoluteBoundingBox.height;
            }
        }

        public static void Configure(this TransparentControl view, FigmaElipse elipse)
        {
            Configure(view, (FigmaVectorEntity)elipse);

            //var circleLayer = new CAShapeLayer();
            //var bezierPath = NSBezierPath.FromOvalInRect(new CGRect(0, 0, elipse.absoluteBoundingBox.width, elipse.absoluteBoundingBox.height));
            //circleLayer.Path = bezierPath.ToGCPath();

            //view.Layer.AddSublayer(circleLayer);

            //var fills = elipse.fills.OfType<FigmaPaint>().FirstOrDefault();
            //if (fills != null)
            //{
            //    circleLayer.FillColor = fills.color.ToNSColor().CGColor;
            //}

            //var strokes = elipse.strokes.FirstOrDefault();
            //if (strokes != null)
            //{
            //    if (strokes.color != null)
            //    {
            //        circleLayer.BorderColor = strokes.color.ToNSColor().CGColor;
            //    }
            //}
        }

        public static void Configure(this LineControl figmaLineView, FigmaLine figmaLine)
        {
            Configure(figmaLineView, (FigmaVectorEntity)figmaLine);

            var fills = figmaLine.fills.OfType<FigmaPaint> ().FirstOrDefault ();
            if (fills != null) {
                figmaLineView.BackColor = fills.color.ToColor ();
            }

            var absolute = figmaLine.absoluteBoundingBox;
            var lineWidth = absolute.width == 0 ? figmaLine.strokeWeight : absolute.width;

            figmaLineView.Width = (int)lineWidth;

            var lineHeight = absolute.height == 0 ? figmaLine.strokeWeight : absolute.height;

            figmaLineView.Height = (int)lineHeight;
        }

        public static void Configure(this TransparentControl view, FigmaVectorEntity child)
        {
            Configure(view, (FigmaNode)child);

            if (child.HasFills && child.fills[0].color != null)
            {
                view.BackColor = child.fills[0].color.ToColor();
            }

            //var currengroupView = new NSView() { TranslatesAutoresizingMaskIntoConstraints = false };
            //currengroupView.Configure(rectangleVector);

            var strokes = child.strokes.FirstOrDefault();
            if (strokes != null)
            {
                //if (strokes.color != null) {
                //    view .BorderColor = strokes.color.ToNSColor ().CGColor;
                //}
                //view.Layer.BorderWidth = child.strokeWeight;
            }
        }

        public static void Configure(this TransparentControl view, FigmaRectangleVector child)
        {
            Configure(view, (FigmaVectorEntity)child);

            //view.Layer.CornerRadius = child.cornerRadius;
        }

        public static void Configure(this Label label, FigmaText text)
        {
            Configure(label, (FigmaNode)text);

            label.TextAlign = text.style.textAlignHorizontal == "CENTER" ? System.Drawing.ContentAlignment.TopCenter : text.style.textAlignHorizontal == "LEFT" ? System.Drawing.ContentAlignment.TopLeft : System.Drawing.ContentAlignment.TopRight;
            //label.AlphaValue = text.opacity;
            //label.LineBreakMode = NSLineBreakMode.ByWordWrapping;
            //label.SetContentCompressionResistancePriority(250, NSLayoutConstraintOrientation.Horizontal);

            var fills = text.fills.FirstOrDefault();
            if (fills != null)
            {
                label.ForeColor = FigmaExtensions.ToColor(fills.color);
            }

            //if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0)
            //{
            //    var attributedText = new NSMutableAttributedString(label.AttributedStringValue);
            //    for (int i = 0; i < text.characterStyleOverrides.Length; i++)
            //    {
            //        var key = text.characterStyleOverrides[i].ToString();
            //        if (!text.styleOverrideTable.ContainsKey(key))
            //        {
            //            continue;
            //        }
            //        var element = text.styleOverrideTable[key];
            //        if (element.fontFamily == null)
            //        {
            //            continue;
            //        }
            //        var localFont = FigmaExtensions.ToNSFont(element);
            //        var range = new NSRange(i, 1);
            //        attributedText.AddAttribute(NSStringAttributeKey.Font, localFont, range);
            //        attributedText.AddAttribute(NSStringAttributeKey.ForegroundColor, label.TextColor, range);
            //    }

            //    label.AttributedStringValue = attributedText;
            //}
        }
    }
}
