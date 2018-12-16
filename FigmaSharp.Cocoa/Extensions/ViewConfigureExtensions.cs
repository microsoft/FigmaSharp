using System;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using System.Linq;

namespace FigmaSharp
{
    public static class ViewConfigureExtensions
    {
        public static void Configure(this NSView view, FigmaFrameEntity child)
        {
            Configure(view, (FigmaNode)child);

            view.AlphaValue = child.opacity;
            view.Layer.BackgroundColor = FigmaExtensions.ToNSColor(child.backgroundColor).CGColor;
        }

        public static void Configure(this NSView view, FigmaNode child)
        {
            view.Hidden = !child.visible;
            view.WantsLayer = true;

            if (child is IFigmaDocumentContainer container)
            {
                view.SetFrameSize(new CGSize(container.absoluteBoundingBox.width, container.absoluteBoundingBox.height));
            }
        }

        public static void Configure(this NSView view, FigmaElipse elipse)
        {
            Configure(view, (FigmaVectorEntity)elipse);

            var circleLayer = new CAShapeLayer();
            var bezierPath = NSBezierPath.FromOvalInRect(new CGRect(0, 0, elipse.absoluteBoundingBox.width, elipse.absoluteBoundingBox.height));
            circleLayer.Path = bezierPath.ToGCPath();

            view.Layer.AddSublayer(circleLayer);

            var fills = elipse.fills.OfType<FigmaPaint>().FirstOrDefault();
            if (fills != null)
            {
                circleLayer.FillColor = fills.color.ToNSColor().CGColor;
            }

            var strokes = elipse.strokes.FirstOrDefault();
            if (strokes != null)
            {
                if (strokes.color != null)
                {
                    circleLayer.BorderColor = strokes.color.ToNSColor().CGColor;
                }
            }
        }

        public static void Configure(this NSView figmaLineView, FigmaLine figmaLine)
        {
            Configure(figmaLineView, (FigmaVectorEntity)figmaLine);

            var fills = figmaLine.fills.OfType<FigmaPaint>().FirstOrDefault();
            if (fills != null)
            {
                figmaLineView.Layer.BackgroundColor = fills.color.ToNSColor().CGColor;
            }

            var absolute = figmaLine.absoluteBoundingBox;
            var lineWidth = absolute.width == 0 ? figmaLine.strokeWeight : absolute.width;

            var constraintWidth = figmaLineView.WidthAnchor.ConstraintEqualToConstant(lineWidth);
            constraintWidth.Priority = (uint)NSLayoutPriority.DefaultLow;
            constraintWidth.Active = true;

            var lineHeight = absolute.height == 0 ? figmaLine.strokeWeight : absolute.height;

            var constraintHeight = figmaLineView.HeightAnchor.ConstraintEqualToConstant(lineHeight);
            constraintHeight.Priority = (uint)NSLayoutPriority.DefaultLow;
            constraintHeight.Active = true;
        }

        public static void Configure(this NSView view, FigmaVectorEntity child)
        {
            Configure(view, (FigmaNode)child);

            if (child.HasFills && child.fills[0].color != null)
            {
                view.Layer.BackgroundColor = child.fills[0].color.ToNSColor().CGColor;
            }

            //var currengroupView = new NSView() { TranslatesAutoresizingMaskIntoConstraints = false };
            //currengroupView.Configure(rectangleVector);

            var strokes = child.strokes.FirstOrDefault();
            if (strokes != null)
            {
                if (strokes.color != null)
                {
                    view.Layer.BorderColor = strokes.color.ToNSColor().CGColor;
                }
                view.Layer.BorderWidth = child.strokeWeight;
            }
        }

        public static void Configure(this NSView view, FigmaRectangleVector child)
        {
            Configure(view, (FigmaVectorEntity)child);

            view.Layer.CornerRadius = child.cornerRadius;
        }

        public static void Configure(this NSTextField label, FigmaText text)
        {
            Configure(label, (FigmaNode)text);

            label.Alignment = text.style.textAlignHorizontal == "CENTER" ? NSTextAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? NSTextAlignment.Left : NSTextAlignment.Right;
            label.AlphaValue = text.opacity;
            label.LineBreakMode = NSLineBreakMode.ByWordWrapping;
            label.SetContentCompressionResistancePriority(250, NSLayoutConstraintOrientation.Horizontal);

            var fills = text.fills.FirstOrDefault();
            if (fills != null)
            {
                label.TextColor = FigmaExtensions.ToNSColor(fills.color);
            }

            if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0)
            {
                var attributedText = new NSMutableAttributedString(label.AttributedStringValue);
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
                    var localFont = FigmaExtensions.ToNSFont(element);
                    var range = new NSRange(i, 1);
                    attributedText.AddAttribute(NSStringAttributeKey.Font, localFont, range);
                    attributedText.AddAttribute(NSStringAttributeKey.ForegroundColor, label.TextColor, range);
                }

                label.AttributedStringValue = attributedText;
            }
        }
    }
}
