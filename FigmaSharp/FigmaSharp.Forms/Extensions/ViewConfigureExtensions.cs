using System;
using System.Linq;
using FigmaSharp.Models;
using Xamarin.Forms;
using FigmaSharp.Views.Forms;

namespace FigmaSharp.Forms
{
    public static class ViewConfigureExtensions
    {
        public static void Configure(this Xamarin.Forms.View view, FigmaFrame child)
        {
            Configure(view, (FigmaNode)child);
            view.Opacity = child.opacity;
            view.BackgroundColor = child.backgroundColor.
                MixOpacity(child.opacity).ToFormsColor();
        }

        public static void Configure(this Xamarin.Forms.View view, FigmaNode child)
        {
            view.IsVisible = child.visible;
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
