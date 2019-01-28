using System;
using System.Linq;
using System.Text;
using Gtk;

namespace FigmaSharp
{
    public static class ViewConfigureExtensions
    {
        public static void Configure(this Widget view, FigmaFrameEntity child)
        {
            Configure(view, (FigmaNode)child);
            //view.o = child.opacity;
            if (view is Fixed fixedView)
            {
                if (child.backgroundColor.a > 0)
                {
                    fixedView.HasWindow = true;
                    view.ModifyBg(StateType.Normal, child.backgroundColor.ToGdkColor());
                }
            }
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
                //builder.AppendLine(string.Format("{0}.SetFrameSize(new {1}({2}, {3}));", 
                    //name, 
                    //nameof (CGSize), 
                    //container.absoluteBoundingBox.width.ToDesignerString (), 
                    //container.absoluteBoundingBox.height.ToDesignerString ()
                    //));
            }
        }

        public static void Configure(this Widget view, FigmaNode child)
        {
            view.Visible = child.visible;
            if (child is IAbsoluteBoundingBox container)
            {
                view.WidthRequest = (int) container.absoluteBoundingBox.width;
                view.HeightRequest = (int)container.absoluteBoundingBox.height;
            }
        }

        public static void Configure(this Widget view, FigmaElipse elipse)
        {
            Configure(view, (FigmaVectorEntity)elipse);

            //var circleLayer = new CAShapeLayer();
            //var bezierPath = NSBezierPath.FromOvalInRect(new CGRect(0, 0, elipse.absoluteBoundingBox.width, elipse.absoluteBoundingBox.height));
            //circleLayer.Path = bezierPath.ToGCPath();

            //view.Layer.AddSublayer(circleLayer);

            var fills = elipse.fills.OfType<FigmaPaint>().FirstOrDefault();
            if (fills != null)
            {
                //circleLayer.FillColor = fills.color.ToNSColor().CGColor;
            }

            var strokes = elipse.strokes.FirstOrDefault();
            if (strokes != null)
            {
                if (strokes.color != null)
                {
                    //circleLayer.BorderColor = strokes.color.ToNSColor().CGColor;
                }
            }
        }

        public static void Configure(this Widget view, FigmaLine child)
        {
            Configure(view, (FigmaVectorEntity)child);

            var fills = child.fills.OfType<FigmaPaint>().FirstOrDefault();
            if (fills != null && child.fills.Length > 0)
            {
                if (child.fills[0].color.a > 0)
                {
                    if (view is Fixed fixedView)
                    {
                        fixedView.HasWindow = true;
                        fixedView.ModifyBg(StateType.Normal, child.fills[0].color.ToGdkColor());
                    }
                }
            }

            //var absolute = figmaLine.absoluteBoundingBox;
            //var lineWidth = absolute.width == 0 ? figmaLine.strokeWeight : absolute.width;

            //var constraintWidth = figmaLineView.WidthAnchor.ConstraintEqualToConstant(lineWidth);
            //constraintWidth.Priority = (uint)NSLayoutPriority.DefaultLow;
            //constraintWidth.Active = true;

            //var lineHeight = absolute.height == 0 ? figmaLine.strokeWeight : absolute.height;

            //var constraintHeight = figmaLineView.HeightAnchor.ConstraintEqualToConstant(lineHeight);
            //constraintHeight.Priority = (uint)NSLayoutPriority.DefaultLow;
            //constraintHeight.Active = true;
        }

        public static void Configure(this Widget view, FigmaVectorEntity child)
        {
            Configure(view, (FigmaNode)child);

            if (child.HasFills && child.fills[0].color != null)
            {
                if (child.fills[0].color.a > 0) {
                    if (view is Fixed fixedView)
                    {
                        fixedView.HasWindow = true;
                        view.ModifyBg(StateType.Normal, child.fills[0].color.ToGdkColor());
                    }
                }
            }

            var strokes = child.strokes.FirstOrDefault();
            if (strokes != null)
            {
                if (strokes.color != null)
                {
                    //view.Layer.BorderColor = strokes.color.ToNSColor().CGColor;
                }
                //view.Layer.BorderWidth = child.strokeWeight;
            }
        }

        public static void Configure(this StringBuilder builder, string name, FigmaVectorEntity child)
        {
            Configure(builder, name, (FigmaNode)child);

            //if (child.HasFills && child.fills[0].color != null)
            //{
            //    builder.AppendLine(string.Format("{0}.Layer.BackgroundColor = {1};", name, child.fills[0].color.ToDesignerString (true)));
            //}

            //var strokes = child.strokes.FirstOrDefault();
            //if (strokes != null)
            //{
            //    if (strokes.color != null)
            //    {
            //        builder.AppendLine(string.Format("{0}.Layer.BorderColor = {1};", name, strokes.color.ToDesignerString(true)));
            //    }
            //    builder.AppendLine(string.Format("{0}.Layer.BorderWidth = {1};", name, child.strokeWeight));
            //}
        }

        public static void Configure(this Widget view, FigmaRectangleVector child)
        {
            Configure(view, (FigmaVectorEntity)child);

            //view.Layer.CornerRadius = child.cornerRadius;
        }

        public static void Configure(this StringBuilder builder, string name, FigmaRectangleVector child)
        {
            Configure(builder, name, (FigmaVectorEntity)child);

            builder.AppendLine(string.Format("{0}.Layer.CornerRadius = {1};", name, child.cornerRadius.ToDesignerString ()));
        }

        public static void Configure(this StringBuilder builder, string name, FigmaText text)
        {
            Configure(builder, name, (FigmaNode)text);

            //var alignment = FigmaExtensions.ToNSTextAlignment(text.style.textAlignHorizontal);

            //builder.AppendLine(string.Format("{0}.Alignment = {1};", name, alignment.ToDesignerString ()));
            //builder.AppendLine(string.Format("{0}.AlphaValue = {1};", name, text.opacity.ToDesignerString ()));

            ////label.LineBreakMode = NSLineBreakMode.ByWordWrapping;
            ////label.SetContentCompressionResistancePriority(250, NSLayoutConstraintOrientation.Horizontal);

            //var fills = text.fills.FirstOrDefault () as FigmaPaint;
            //if (fills != null)
            //{
            //    builder.AppendLine(string.Format("{0}.TextColor = {1};", name, fills.color.ToDesignerString ()));
            //}

            //if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0)
            //{
            //    var attributedTextName = "attributedText" + DateTime.Now.ToString ("HHmmss");
            //    builder.AppendLine(string.Format("var {0} = new NSMutableAttributedString({1}.AttributedStringValue);", attributedTextName, name));

            //    //var attributedText = new NSMutableAttributedString(label.AttributedStringValue);
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


            //        builder.AppendLine(string.Format("{0}.AddAttribute(NSStringAttributeKey.Font, {1}, new NSRange({2}, 1));", attributedTextName, element.ToNSFontDesignerString(), i));

            //        string color = color = fills?.color?.ToDesignerString(); ;

            //        if (element.fills != null && element.fills.Any ())
            //        {
            //            if (element.fills.FirstOrDefault() is FigmaPaint paint)
            //            {
            //                color = paint.color.ToDesignerString();
            //            }
            //            builder.AppendLine(string.Format("{0}.AddAttribute(NSStringAttributeKey.ForegroundColor, {1}, new NSRange({2}, 1));", attributedTextName, color, i));
            //        }
            //    }
            //    builder.AppendLine(string.Format("{0}.AttributedStringValue = {1};", name, attributedTextName));
            //}
        }

        static float GetHorizontalAlignment (string value)
        {
            if (value == "CENTER")
            {
                return 0.5f;
            }
            if (value == "RIGHT")
            {
                return 1f;
            }
            return 0f;
        }

        static Justification GetJustification(string value)
        {
            if (value == "CENTER")
            {
                return Justification.Center;
            }
            if (value == "RIGHT")
            {
                return Justification.Right;
            }
            return Justification.Left;
        }

        static string GetFontName (FigmaTypeStyle style)
        {
            string family = style.fontFamily;
            if (family == "SF UI Text")
            {
                family = ".SF NS Text";
            }
            else if (family == "SF Mono")
            {
                family = ".SF NS Display";
            }
            else
            {
                Console.WriteLine("FONT: {0} - {1}", family, style.fontPostScriptName);
            }
            return family;
        }

        static void AppendSpan (StringBuilder builder, string color, string fontName, int weight, int size, string text)
        {
            builder.Append("<span");
            if (!string.IsNullOrEmpty(color)) {
                builder.Append($" foreground=\"#{color}\"");
            }

            if (!string.IsNullOrEmpty(fontName))
                builder.Append($" font=\"{fontName}\"");

            builder.Append($" font_weight=\"{weight}\"");
                
            builder.Append($" size=\"{size}\"");

            builder.Append(">");
            builder.Append(text);
            builder.Append("</span>");
        }

        public static void Configure(this Label label, FigmaText text)
        {
            Configure(label, (FigmaNode)text);
            //label.al
            //label.a = text.style.textAlignHorizontal == "CENTER" ? NSTextAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? NSTextAlignment.Left : NSTextAlignment.Right;
            //label.AlphaValue = text.opacity;
            //label.LineBreakMode = NSLineBreakMode.ByWordWrapping;
            //label.SetContentCompressionResistancePriority(250, NSLayoutConstraintOrientation.Horizontal);
            float textHorizontalAlign = GetHorizontalAlignment (text.style.textAlignHorizontal);
            label.SetAlignment(textHorizontalAlign, 0.5f);
            label.Justify = GetJustification(text.style.textAlignHorizontal);
            label.Text = text.characters;
            label.Wrap = true;

            var fills = text.fills.FirstOrDefault();
            if (fills == null)
            {
                return;
            }

            label.UseMarkup = true;
            StringBuilder builder = new StringBuilder();

            var defaultColor = ((int)(fills.color.r * 255)).ToString("X2") + ((int)(fills.color.g * 255)).ToString("X2") + ((int)(fills.color.b * 255)).ToString("X2");
            var defaultFontName = GetFontName(text.style);
            var defaultSize = text.style.fontSize * 1024;

            if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0)
            {
                for (int i = 0; i < text.characterStyleOverrides.Length; i++)
                {
                    var key = text.characterStyleOverrides[i].ToString();
                    if (!text.styleOverrideTable.ContainsKey(key))
                    {
                        AppendSpan(builder, defaultColor, defaultFontName, text.style.fontWeight, defaultSize, text.characters[i].ToString ());
                        continue;
                    }
                    var element = text.styleOverrideTable[key];
                    if (element.fontFamily == null)
                    {
                        AppendSpan(builder, defaultColor, defaultFontName, text.style.fontWeight, defaultSize, text.characters[i].ToString ());
                        continue;
                    }

                    var color = defaultColor;
                    if (element.fills != null && element.fills.Any())
                    {
                        if (element.fills.FirstOrDefault() is FigmaPaint paint)
                        {
                            color = ((int)(paint.color.r * 255)).ToString("X2") + ((int)(paint.color.g * 255)).ToString("X2") + ((int)(paint.color.b * 255)).ToString("X2");
                        }
                    }

                    var fontName = GetFontName(element);
                    var size = element.fontSize * 1024;
                    AppendSpan(builder, color, defaultFontName, element.fontWeight, size, text.characters[i].ToString ());
                }
            }
            else
            {
                AppendSpan(builder, defaultColor, defaultFontName, text.style.fontWeight, defaultSize, text.characters);
            }

            label.Markup = builder.ToString();
        }
    }
}
