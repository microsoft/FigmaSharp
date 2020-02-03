using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Linq;
using System.Text;
using FigmaSharp.Models;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Cocoa
{
    public static class ViewConfigureExtensions
    {
        public static string GetFullName (this Enum myEnum)
        {
            return string.Format ("{0}.{1}", myEnum.GetType ().Name, myEnum.ToString ());
        }

        public static void Configure(this NSView view, FigmaFrameEntity child)
        {
            Configure(view, (FigmaNode)child);
		
			view.AlphaValue = child.opacity;
			view.Layer.BackgroundColor = child.backgroundColor.MixOpacity(child.opacity).ToCGColor();
		}

        public static void Configure(this StringBuilder builder, string name, FigmaNode child, bool drawFrameSize = true)
        {
            if (!child.visible)
            {
                builder.AppendLine(string.Format("{0}.Hidden = {1};", name, (!child.visible).ToDesignerString()));
            }

            builder.AppendLine(string.Format ("{0}.WantsLayer = {1};", name, true.ToDesignerString ()));
            if (drawFrameSize && child is IAbsoluteBoundingBox container)
            {
                builder.AppendLine(string.Format("{0}.SetFrameSize(new {1}({2}, {3}));", 
                    name, 
                    typeof (CGSize).FullName, 
                    container.absoluteBoundingBox.Width.ToDesignerString (), 
                    container.absoluteBoundingBox.Height.ToDesignerString ()
                    ));
            }
        }

        public static void Configure(this NSView view, FigmaNode child)
        {
            view.Hidden = !child.visible;
            view.WantsLayer = true;

            if (child is IAbsoluteBoundingBox container)
            {
                view.SetFrameSize(new CGSize(container.absoluteBoundingBox.Width, container.absoluteBoundingBox.Height));
            }
        }

        public static void Configure(this NSView view, FigmaElipse elipse)
        {
            Configure(view, (FigmaNode)elipse);

            //var circleLayer = new CAShapeLayer();
            //var bezierPath = NSBezierPath.FromOvalInRect(new CGRect(0, 0, elipse.absoluteBoundingBox.Width, elipse.absoluteBoundingBox.Height));
            //circleLayer.Path = bezierPath.ToCGPath();

            //view.Layer.AddSublayer(circleLayer);

            //var fills = elipse.fills.OfType<FigmaPaint>().FirstOrDefault();
            //if (fills != null && fills.color != null)
            //{
            //    circleLayer.FillColor = fills.color.MixOpacity(fills.opacity).ToNSColor().CGColor;
            //} else
            //{
            //    circleLayer.FillColor = NSColor.Clear.CGColor;
            //}

            //var strokes = elipse.strokes.FirstOrDefault();
            //if (strokes != null)
            //{
            //    if (strokes.color != null)
            //    {
            //        circleLayer.StrokeColor = strokes.color.MixOpacity(strokes.opacity).ToNSColor().CGColor;
            //    }
            //}

            //if (elipse.strokeDashes != null)
            //{
            //    var number = new NSNumber[elipse.strokeDashes.Length];
            //    for (int i = 0; i < elipse.strokeDashes.Length; i++)
            //    {
            //        number[i] = elipse.strokeDashes[i];
            //    }
            //    circleLayer.LineDashPattern = number;
            //}

            //circleLayer.BackgroundColor = NSColor.Clear.CGColor;
            //circleLayer.LineWidth = elipse.strokeWeight;

            view.AlphaValue = elipse.opacity;
        }

        public static void Configure(this NSView figmaLineView, FigmaLine figmaLine)
        {
            //we draw a figma line from images or svg
            Configure(figmaLineView, (FigmaNode)figmaLine);

            var absolute = figmaLine.absoluteBoundingBox;
            var lineWidth = absolute.Width == 0 ? figmaLine.strokeWeight : absolute.Width;

            var constraintWidth = figmaLineView.WidthAnchor.ConstraintEqualToConstant(lineWidth);
            constraintWidth.Priority = (uint)NSLayoutPriority.DefaultLow;
            constraintWidth.Active = true;

            var lineHeight = absolute.Height == 0 ? figmaLine.strokeWeight : absolute.Height;

            var constraintHeight = figmaLineView.HeightAnchor.ConstraintEqualToConstant(lineHeight);
            constraintHeight.Priority = (uint)NSLayoutPriority.DefaultLow;
            constraintHeight.Active = true;

            figmaLineView.AlphaValue = figmaLine.opacity;
        }
		
        public static void Configure(this NSView view, FigmaVectorEntity child)
        {
            Configure(view, (FigmaNode)child);
            view.AlphaValue = child.opacity;
        }

        public static void Configure(this StringBuilder builder, string name, FigmaVectorEntity child)
        {
            Configure(builder, name, (FigmaNode)child);

            var fills = child.fills.FirstOrDefault ();
            if (fills != null && fills.visible && fills.color != null)
            {
                builder.AppendLine(string.Format("{0}.Layer.BackgroundColor = {1};", name, fills.color.ToDesignerString (true)));
            }

            var strokes = child.strokes.FirstOrDefault();
            if (strokes != null && strokes.visible)
            {
                if (strokes.color != null)
                {
                    builder.AppendLine(string.Format("{0}.Layer.BorderColor = {1};", name, strokes.color.ToDesignerString(true)));
                }
                builder.AppendLine(string.Format("{0}.Layer.BorderWidth = {1};", name, child.strokeWeight));
            }
        }

        static Color MixOpacity (this Color color, float opacity)
        {
            return new Color { A = Math.Min(color.A, opacity), R = color.R, G = color.G, B = color.B };
        }

        public static void Configure(this NSView view, RectangleVector child)
        {
            Configure(view, (FigmaVectorEntity)child);

            //var shapeLayer = new CAShapeLayer
            //{
            //    Path = NSBezierPath.FromRect(view.Bounds).ToCGPath(),
            //    Frame = view.Layer.Frame
            //};
            //view.Layer = shapeLayer;

            //var fill = child.fills?.FirstOrDefault();
            //if (fill != null && fill.visible && fill.color != null)
            //{
            //    shapeLayer.FillColor = fill.color.MixOpacity (fill.opacity).ToNSColor().CGColor;
            //} else
            //{
            //    shapeLayer.FillColor = NSColor.Clear.CGColor;
            //}
            
            //if (child.strokeDashes != null)
            //{
            //    var number = new NSNumber[child.strokeDashes.Length];
            //    for (int i = 0; i < child.strokeDashes.Length; i++)
            //    {
            //        number[i] = child.strokeDashes[i];
            //    }
            //    shapeLayer.LineDashPattern = number;
            //}

            ////shapeLayer.BackgroundColor = child.col
            //shapeLayer.LineWidth = child.strokeWeight * 2;

            //var strokes = child.strokes.FirstOrDefault();
            //if (strokes != null && strokes.visible)
            //{
            //    if (strokes.color != null)
            //    {
            //        shapeLayer.StrokeColor = strokes.color.MixOpacity (strokes.opacity).ToNSColor().CGColor;
            //        if (shapeLayer.LineWidth == 0)
            //        {
            //            shapeLayer.LineWidth = 1f;
            //        }
            //    }
            //}
            
            //shapeLayer.CornerRadius = child.cornerRadius;
            //view.AlphaValue = child.opacity;
        }

        public static void Configure(this StringBuilder builder, string name, RectangleVector child)
        {
            Configure(builder, name, (FigmaVectorEntity)child);

            builder.AppendLine(string.Format("{0}.Layer.CornerRadius = {1};", name, child.cornerRadius.ToDesignerString ()));
        }

        public static void Configure(this StringBuilder builder, string name, FigmaText text)
        {
            //Configure(builder, name, (FigmaNode)text);

            var alignment = FigmaExtensions.ToNSTextAlignment(text.style.textAlignHorizontal);

            var propertyAttributedStringValue = $"{name}.{nameof (NSTextField.AttributedStringValue)}";
            var propertyTextColor = $"{name}.{nameof (NSTextField.TextColor)}";
            var propertyFont = $"{name}.{nameof (NSTextField.Font)}";
            var propertyAlignment = $"{name}.{nameof (NSTextField.Alignment)}";
            var propertyAlphaValue = $"{name}.{nameof (NSTextField.AlphaValue)}";

            builder.AppendLine (string.Format ("{0} = {1};", propertyFont, text.style.ToNSFontDesignerString ()));
            builder.AppendLine(string.Format("{0} = {1};", propertyAlignment, alignment.ToDesignerString ()));
            builder.AppendLine(string.Format("{0} = {1};", propertyAlphaValue, text.opacity.ToDesignerString ()));

            //label.LineBreakMode = NSLineBreakMode.ByWordWrapping;
            //label.SetContentCompressionResistancePriority(250, NSLayoutConstraintOrientation.Horizontal);

            var fills = text.fills.FirstOrDefault () as FigmaPaint;
            if (fills != null)
            {
                builder.AppendLine(string.Format("{0} = {1};", propertyTextColor, fills.color.ToDesignerString ()));
            }

            if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0) {

                var attributedTextName = string.Format ("{0}AttributedText", Resources.Ids.Conversion.NameIdentifier);
                builder.AppendLine (string.Format ("var {0} = new {1} ({2});", attributedTextName, typeof (NSMutableAttributedString).FullName, propertyAttributedStringValue));

                var attributedStringKey = typeof (NSStringAttributeKey).FullName;
                var attributtedStringForegroundColor = $"{attributedStringKey}.{nameof (NSStringAttributeKey.ForegroundColor)}";
                var attributtedStringFont = $"{attributedStringKey}.{nameof (NSStringAttributeKey.Font)}";

                //var attributedText = new NSMutableAttributedString (label.AttributedStringValue);
                for (int i = 0; i < text.characterStyleOverrides.Length; i++) {
					
                    var range = $"new {typeof (NSRange).FullName} ({i}, 1)";

                    var key = text.characterStyleOverrides[i].ToString ();
                    if (!text.styleOverrideTable.ContainsKey (key)) {
                        //we want the default values
                        //builder.AppendLine (string.Format ("{0}.AddAttribute(AppKit.NSStringAttributeKey.Font, {1}, new NSRange({2}, 1));", attributedTextName, element.ToNSFontDesignerString (), i));

                        builder.AppendLine (string.Format ("{0}.AddAttribute({1}, {2}, {3});", attributedTextName, attributtedStringForegroundColor, propertyTextColor, range));
                        builder.AppendLine (string.Format ("{0}.AddAttribute({1}, {2}, {3});", attributedTextName, attributtedStringFont, propertyTextColor, range));
                        continue;
                    }

					//if there is a style to override
                    var styleOverrided = text.styleOverrideTable[key];

                    //set the color

                    string fontColorOverrided;
                    var fillOverrided = styleOverrided.fills?.FirstOrDefault ();
                    if (fillOverrided != null && fillOverrided.visible)
                        fontColorOverrided = fillOverrided.color.ToDesignerString ();
                    else {
                        fontColorOverrided = propertyTextColor;
                    }

                    builder.AppendLine (string.Format ("{0}.AddAttribute({1}, {2}, {3});", attributedTextName, attributtedStringForegroundColor, fontColorOverrided, range));

                    //TODO: we can improve this
                    //set the font for this character
                    string fontOverrided;
                    if (styleOverrided?.fontFamily != null) {
                        fontOverrided = styleOverrided.ToNSFontDesignerString ();
                    } else {
                        fontOverrided = propertyFont;
                    }

                    builder.AppendLine (string.Format ("{0}.AddAttribute ({1}, {2}, {3});", attributedTextName,  attributtedStringFont, fontOverrided, range));
                }

                builder.AppendLine (string.Format ("{0} = {1};", propertyAttributedStringValue, attributedTextName));
               
            }
        }

        public static void Configure(this NSTextField label, FigmaText text)
        {
            label.Alignment = text.style.textAlignHorizontal == "CENTER" ? NSTextAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? NSTextAlignment.Left : NSTextAlignment.Right;
            label.AlphaValue = text.opacity;
            //label.LineBreakMode = NSLineBreakMode.ByWordWrapping;
            //label.SetContentCompressionResistancePriority(250, NSLayoutConstraintOrientation.Horizontal);
            if (label.Cell is VerticalAlignmentTextCell cell)
            {
                cell.VerticalAligment = text.style.textAlignVertical == "CENTER" ? VerticalTextAlignment.Center : text.style.textAlignVertical == "TOP" ? VerticalTextAlignment.Top : VerticalTextAlignment.Bottom;
            }

            var fills = text.fills.FirstOrDefault();
            if (fills != null && fills.visible)
            {
                label.TextColor = fills.color.ToNSColor();
            }

            if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0)
            {
                var attributedText = new NSMutableAttributedString(label.AttributedStringValue);
                for (int i = 0; i < text.characterStyleOverrides.Length; i++)
                {
                    var range = new NSRange(i, 1);

                    var key = text.characterStyleOverrides[i].ToString();
                    if (!text.styleOverrideTable.ContainsKey(key) )
                    {
                        //we want the default values
                        attributedText.AddAttribute(NSStringAttributeKey.ForegroundColor, label.TextColor, range);
                        attributedText.AddAttribute(NSStringAttributeKey.Font, label.Font, range);
                        continue;
                    }

                    //if there is a style to override
                    var styleOverrided = text.styleOverrideTable[key];

                    //set the color
                    NSColor fontColorOverrided = label.TextColor;
                    var fillOverrided = styleOverrided.fills?.FirstOrDefault();
                    if (fillOverrided != null && fillOverrided.visible)
                        fontColorOverrided = fillOverrided.color.ToNSColor();

                    attributedText.AddAttribute(NSStringAttributeKey.ForegroundColor, fontColorOverrided, range);

                    //TODO: we can improve this
                    //set the font for this character
                    NSFont fontOverrided = label.Font;
                    if (styleOverrided.fontFamily != null)
                    {
                        fontOverrided = FigmaExtensions.ToNSFont(styleOverrided);
                    }
                    attributedText.AddAttribute(NSStringAttributeKey.Font, fontOverrided, range);
                }

                label.AttributedStringValue = attributedText;
            }
        }
    }
}
