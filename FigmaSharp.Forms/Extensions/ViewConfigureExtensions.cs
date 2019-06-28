using System;
using System.Linq;
using FigmaSharp.Models;
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
        }

        public static void Configure(this View view, FigmaVectorEntity child)
        {
            Configure(view, (FigmaNode)child);
        }

        public static void Configure(this View view, FigmaRectangleVector child)
        {
            Configure(view, (FigmaVectorEntity)child);

            if (child.HasFills && child.fills[0].color != null)
            {
                view.BackgroundColor = child.fills[0].color.ToColor();
            }
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
        }
    }
}
