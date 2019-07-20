using System;
using System.Linq;
using FigmaSharp.Models;
using Xamarin.Forms;
using FigmaSharp.Views.Forms;

namespace FigmaSharp.Forms
{
    public static class ViewConfigureExtensions
    {
        public static void Configure(this Xamarin.Forms.View view, FigmaFrameEntity child)
        {
            Configure(view, (FigmaNode)child);
            view.Opacity = child.opacity;
            view.BackgroundColor = child.backgroundColor.
                MixOpacity(child.opacity).ToFormsColor();
        }

        public static void Configure(this Xamarin.Forms.View view, FigmaNode child)
        {
            view.IsVisible = child.visible;
            if (child is IAbsoluteBoundingBox container)
            {
                AbsoluteLayout.SetLayoutBounds(view, new Xamarin.Forms.Rectangle(0, 0, container.absoluteBoundingBox.Width, container.absoluteBoundingBox.Height));
            }
        }

        public static void Configure(this Xamarin.Forms.View view, FigmaElipse elipse)
        {
            Configure(view, (FigmaVectorEntity)elipse);
        }

        public static void Configure(this Xamarin.Forms.View view, FigmaLine figmaLine)
        {
            Configure(view, (FigmaVectorEntity)figmaLine);

            //var fills = figmaLine.fills.OfType<FigmaPaint>().FirstOrDefault();
            //if (fills != null)
            //{
            //    view.BackgroundColor = fills.color.ToFormsColor();
            //}
        }

        public static void Configure(this Xamarin.Forms.View view, FigmaVectorEntity child)
        {
            Configure(view, (FigmaNode)child);
        }

        public static void Configure(this Xamarin.Forms.View view, RectangleVector child)
        {
            Configure(view, (FigmaVectorEntity)child);

            //if (child.HasFills && child.fills[0].color != null)
            //{
            //    view.BackgroundColor = child.fills[0].color.ToFormsColor();
            //}
        }

        public static void Configure(this Label label, FigmaText text)
        {
            Configure(label, (FigmaNode)text);

            label.HorizontalTextAlignment = text.style.textAlignHorizontal == "CENTER" ? TextAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? TextAlignment.Start : TextAlignment.End;
            label.Opacity = text.opacity;
            label.VerticalTextAlignment = text.style.textAlignVertical == "CENTER" ? TextAlignment.Center : text.style.textAlignHorizontal == "TOP" ? TextAlignment.Start : TextAlignment.End;

            if (text.HasFills)
                label.TextColor = text.fills[0].color.ToFormsColor();
        }
    }
}
