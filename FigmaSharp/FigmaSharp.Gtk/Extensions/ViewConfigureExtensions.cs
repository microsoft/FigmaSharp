using System;
using System.Linq;
using System.Text;
using FigmaSharp.Models;
using Gtk;
using FigmaSharp.Models;

namespace FigmaSharp.GtkSharp
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

        public static void Configure(this StringBuilder builder, string name, FigmaFrameEntity child)
        {
            Configure(builder, name, (FigmaNode)child);
            if (child.backgroundColor.a > 0)
            {
                builder.AppendLine(string.Format("{0}.HasWindow = true;", name));
                builder.AppendLine(string.Format("{0}.ModifyBg(Gtk.StateType.Normal, {1});", name, child.backgroundColor.ToDesignerString ()));
            }
        }

        public static void Configure(this StringBuilder builder, string name, FigmaNode child)
        {
            if (child is IAbsoluteBoundingBox container)
            {
                var width = (int)container.absoluteBoundingBox.width;
                var height = (int)container.absoluteBoundingBox.height;
                builder.AppendLine(string.Format("{0}.WidthRequest = {1};", name, width));
                builder.AppendLine(string.Format("{0}.HeightRequest = {1};", name, height));
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
           //TODO: NOT IMPLEMENTED
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
                        fixedView.ModifyBg(StateType.Normal, child.fills[0].color.ToGdkColor());
                    }
                }
            }

            var strokes = child.strokes.FirstOrDefault();
            if (strokes != null)
            {
                //TODO: NOT IMPLEMENTED
                //if (strokes.color != null)
                //{
                //    view.Layer.BorderColor = strokes.color.ToNSColor().CGColor;
                //}
                //view.Layer.BorderWidth = child.strokeWeight;
            }
        }

        public static void Configure(this StringBuilder builder, string name, FigmaVectorEntity child)
        {
            Configure(builder, name, (FigmaNode)child);

            if (child.HasFills && child.fills[0].color != null)
            {
                builder.AppendLine(string.Format("{0}.HasWindow = true;", name));
                builder.AppendLine(string.Format("{0}.ModifyBg(Gtk.StateType.Normal, {1});", name, child.fills[0].color.ToDesignerString ()));
            }

            //var strokes = child.strokes.FirstOrDefault();
            //if (strokes != null)
            //{
            //    //if (strokes.color != null)
            //    //{
            //    //    builder.AppendLine(string.Format("{0}.Layer.BorderColor = {1};", name, strokes.color.ToDesignerString(true)));
            //    //}
            //    builder.AppendLine(string.Format("{0}.Layer.BorderWidth = {1};", name, child.strokeWeight));
            //}
        }

        public static void Configure(this Widget view, FigmaRectangleVector child)
        {
            Configure(view, (FigmaVectorEntity)child);
            //TODO: NOT IMPLEMENTED
            //view.Layer.CornerRadius = child.cornerRadius;
        }

        public static void Configure(this StringBuilder builder, string name, FigmaRectangleVector child)
        {
            Configure(builder, name, (FigmaVectorEntity)child);
            //TODO: NOT IMPLEMENTED
            //builder.AppendLine(string.Format("{0}.Layer.CornerRadius = {1};", name, child.cornerRadius.ToDesignerString ()));
        }

        public static void Configure(this StringBuilder builder, string name, FigmaText text)
        {
            Configure(builder, name, (FigmaNode)text);
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
            label.Justify = GetJustification(text.style.textAlignHorizontal);
            label.Text = text.characters;
            label.Wrap = true;
			label.LineWrapMode = Pango.WrapMode.Word;
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
