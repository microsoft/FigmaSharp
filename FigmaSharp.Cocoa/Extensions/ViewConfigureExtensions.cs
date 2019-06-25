using System;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using System.Linq;
using System.Text;

namespace FigmaSharp.Cocoa
{
    public static class ViewConfigureExtensions
    {
        public static void Configure(this NSView view, FigmaFrameEntity child)
        {
            Configure(view, (FigmaNode)child);

            view.AlphaValue = child.opacity;
            view.Layer.BackgroundColor = FigmaExtensions.ToNSColor(child.backgroundColor).CGColor;
        }

        public static void Configure(this StringBuilder builder, string name, FigmaNode child)
        {
            if (!child.visible)
            {
                builder.AppendLine(string.Format("{0}.Hidden = {1};", name, (!child.visible).ToDesignerString()));
            }

            builder.AppendLine(string.Format ("{0}.WantsLayer = {1};", name, true.ToDesignerString ()));
            if (child is IAbsoluteBoundingBox container)
            {
                builder.AppendLine(string.Format("{0}.SetFrameSize(new {1}({2}, {3}));", 
                    name, 
                    nameof (CGSize), 
                    container.absoluteBoundingBox.width.ToDesignerString (), 
                    container.absoluteBoundingBox.height.ToDesignerString ()
                    ));
            }
        }

        public static void Configure(this NSView view, FigmaNode child)
        {
            view.Hidden = !child.visible;
            view.WantsLayer = true;

            if (child is IAbsoluteBoundingBox container)
            {
                view.SetFrameSize(new CGSize(container.absoluteBoundingBox.width, container.absoluteBoundingBox.height));
            }
        }

        public static void Configure(this NSView view, FigmaElipse elipse)
        {
            Configure(view, (FigmaNode)elipse);

            var circleLayer = new CAShapeLayer();
            var bezierPath = NSBezierPath.FromOvalInRect(new CGRect(0, 0, elipse.absoluteBoundingBox.width, elipse.absoluteBoundingBox.height));
            circleLayer.Path = bezierPath.ToCGPath();

            view.Layer.AddSublayer(circleLayer);

            var fills = elipse.fills.OfType<FigmaPaint>().FirstOrDefault();
            if (fills != null && fills.color != null)
            {
                circleLayer.FillColor = fills.color.ToNSColor().CGColor;
            }

            var strokes = elipse.strokes.FirstOrDefault();
            if (strokes != null)
            {
                if (strokes.color != null)
                {
                    circleLayer.StrokeColor = strokes.color.ToNSColor().CGColor;
                }
            }

            circleLayer.BackgroundColor = NSColor.Clear.CGColor;
            circleLayer.LineWidth = elipse.strokeWeight;
        }

        public static void Configure(this NSView figmaLineView, FigmaLine figmaLine)
        {
            Configure(figmaLineView, (FigmaNode)figmaLine);

            var fills = figmaLine.fills.OfType<FigmaPaint>().FirstOrDefault();
            //if (fills != null)
            //{
            //    figmaLineView.Layer.BackgroundColor = fills.color.ToNSColor().CGColor;
            //}

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
        }

        public static void Configure(this StringBuilder builder, string name, FigmaVectorEntity child)
        {
            Configure(builder, name, (FigmaNode)child);

            if (child.HasFills && child.fills[0].color != null)
            {
                builder.AppendLine(string.Format("{0}.Layer.BackgroundColor = {1};", name, child.fills[0].color.ToDesignerString (true)));
            }

            var strokes = child.strokes.FirstOrDefault();
            if (strokes != null)
            {
                if (strokes.color != null)
                {
                    builder.AppendLine(string.Format("{0}.Layer.BorderColor = {1};", name, strokes.color.ToDesignerString(true)));
                }
                builder.AppendLine(string.Format("{0}.Layer.BorderWidth = {1};", name, child.strokeWeight));
            }
        }

        public static void Configure(this NSView view, FigmaRectangleVector child)
        {
            Configure(view, (FigmaVectorEntity)child);

            var shapeLayer = new CAShapeLayer
            {
                Path = NSBezierPath.FromRect(view.Bounds).ToCGPath(),
                Frame = view.Layer.Frame
            };

            view.Layer = shapeLayer;

            if (child.HasFills && child.fills[0].color != null)
            {
                shapeLayer.FillColor = child.fills[0].color.ToNSColor().CGColor;
            } else
            {
                shapeLayer.FillColor = NSColor.Clear.CGColor;
            }

            if (child.strokeDashes != null)
            {
                var number = new NSNumber[child.strokeDashes.Length];
                for (int i = 0; i < child.strokeDashes.Length; i++)
                {
                    number[i] = child.strokeDashes[i];
                }
                shapeLayer.LineDashPattern = number;
            }

            var strokes = child.strokes.FirstOrDefault();
            if (strokes != null && strokes.color != null)
            {
                shapeLayer.StrokeColor = strokes.color.ToNSColor().CGColor;
            }

            shapeLayer.BackgroundColor = NSColor.Clear.CGColor;
            shapeLayer.LineWidth = child.strokeWeight;
            shapeLayer.CornerRadius = child.cornerRadius;
        }

        public static void Configure(this StringBuilder builder, string name, FigmaRectangleVector child)
        {
            Configure(builder, name, (FigmaVectorEntity)child);

            builder.AppendLine(string.Format("{0}.Layer.CornerRadius = {1};", name, child.cornerRadius.ToDesignerString ()));
        }

        public static void Configure(this StringBuilder builder, string name, FigmaText text)
        {
            Configure(builder, name, (FigmaNode)text);

            var alignment = FigmaExtensions.ToNSTextAlignment(text.style.textAlignHorizontal);

            builder.AppendLine(string.Format("{0}.Alignment = {1};", name, alignment.ToDesignerString ()));
            builder.AppendLine(string.Format("{0}.AlphaValue = {1};", name, text.opacity.ToDesignerString ()));

            //label.LineBreakMode = NSLineBreakMode.ByWordWrapping;
            //label.SetContentCompressionResistancePriority(250, NSLayoutConstraintOrientation.Horizontal);

            var fills = text.fills.FirstOrDefault () as FigmaPaint;
            if (fills != null)
            {
                builder.AppendLine(string.Format("{0}.TextColor = {1};", name, fills.color.ToDesignerString ()));
            }

            if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0)
            {
                var attributedTextName = "attributedText" + DateTime.Now.ToString ("HHmmss");
                builder.AppendLine(string.Format("var {0} = new NSMutableAttributedString({1}.AttributedStringValue);", attributedTextName, name));

                //var attributedText = new NSMutableAttributedString(label.AttributedStringValue);
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
                    builder.AppendLine(string.Format("{0}.AddAttribute(NSStringAttributeKey.Font, {1}, new NSRange({2}, 1));", attributedTextName, element.ToNSFontDesignerString(), i));

                    string color = color = fills?.color?.ToDesignerString(); ;

                    if (element.fills != null && element.fills.Any ())
                    {
                        if (element.fills.FirstOrDefault() is FigmaPaint paint)
                        {
                            color = paint.color.ToDesignerString();
                        }
                        builder.AppendLine(string.Format("{0}.AddAttribute(NSStringAttributeKey.ForegroundColor, {1}, new NSRange({2}, 1));", attributedTextName, color, i));
                    }
                }
                builder.AppendLine(string.Format("{0}.AttributedStringValue = {1};", name, attributedTextName));
            }
        }

        public static void Configure(this NSTextField label, FigmaText text)
        {
            Configure(label, (FigmaNode)text);

            label.Alignment = text.style.textAlignHorizontal == "CENTER" ? NSTextAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? NSTextAlignment.Left : NSTextAlignment.Right;
            label.AlphaValue = text.opacity;
            //label.LineBreakMode = NSLineBreakMode.ByWordWrapping;
            //label.SetContentCompressionResistancePriority(250, NSLayoutConstraintOrientation.Horizontal);
            if (label.Cell is VerticalAlignmentTextCell cell)
            {
                cell.VerticalAligment = text.style.textAlignVertical == "CENTER" ? VerticalTextAlignment.Center : text.style.textAlignVertical == "TOP" ? VerticalTextAlignment.Top : VerticalTextAlignment.Bottom;
            }

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
