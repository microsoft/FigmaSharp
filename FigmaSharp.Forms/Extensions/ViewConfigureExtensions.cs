using System;
using System.Linq;
using Xamarin.Forms;

namespace FigmaSharp.Forms
{
    public static class ViewConfigureExtensions
    {
        public static void Configure(this View view, FigmaFrameEntity child)
        {
            Configure(view, (FigmaNode)child);
            view.Opacity = child.opacity;
            view.BackgroundColor = child.backgroundColor.ToColor();
        }

        public static void Configure(this View view, FigmaNode child)
        {
            view.IsVisible = child.visible;
            if (child is IAbsoluteBoundingBox container)
            {
                AbsoluteLayout.SetLayoutBounds(view, new Rectangle(0, 0, container.absoluteBoundingBox.width, container.absoluteBoundingBox.height));
            }
        }

        public static void Configure(this View view, FigmaElipse elipse)
        {
            Configure(view, (FigmaVectorEntity)elipse);
        }

        public static void Configure(this View view, FigmaLine figmaLine)
        {
            Configure(view, (FigmaVectorEntity)figmaLine);

            var fills = figmaLine.fills.OfType<FigmaPaint>().FirstOrDefault();
            if (fills != null)
            {
                view.BackgroundColor = fills.color.ToColor();
            }

            AbsoluteLayout.SetLayoutBounds(view, new Rectangle(0, 0, figmaLine.absoluteBoundingBox.width, figmaLine.absoluteBoundingBox.height));
        }

        public static void Configure(this View view, FigmaVectorEntity child)
        {
            Configure(view, (FigmaNode)child);

            //if (child.HasFills && child.fills[0].color != null)
            //{
            //    view.BackgroundColor = child.fills[0].color.ToColor();
            //}

            //var strokes = child.strokes.FirstOrDefault();
            //if (strokes != null)
            //{
            //    if (strokes.color != null)
            //    {
            //        //view.BorderColor = strokes.color.ToColor();
            //    }
            //    //view.BorderWidth = child.strokeWeight;
            //}
        }

        public static void Configure(this View view, FigmaRectangleVector child)
        {
            Configure(view, (FigmaVectorEntity)child);
            //view.Layer.CornerRadius = child.cornerRadius;
        }

        public static void Configure(this Label label, FigmaText text)
        {
            Configure(label, (FigmaNode)text);

            label.HorizontalTextAlignment = text.style.textAlignHorizontal == "CENTER" ? TextAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? TextAlignment.Start : TextAlignment.End;
            label.Opacity = text.opacity;
           label.VerticalTextAlignment = text.style.textAlignVertical == "CENTER" ? TextAlignment.Center : text.style.textAlignHorizontal == "TOP" ? TextAlignment.Start : TextAlignment.End;
            var fills = text.fills.FirstOrDefault();
            if (fills != null)
            {
                label.TextColor = FigmaExtensions.ToColor(fills.color);
            }

            //if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0)
            //{
            //    var attributedText = new NSMutableAttributedString(label.AttributedText);
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
            //        var localFont = FigmaExtensions.ToUIFont(element);
            //        var range = new NSRange(i, 1);
            //        attributedText.AddAttribute(UIStringAttributeKey.Font, localFont, range);
            //        attributedText.AddAttribute(UIStringAttributeKey.ForegroundColor, label.TextColor, range);
            //    }

            //    label.AttributedText = attributedText;
            //}
        }
    }
}
