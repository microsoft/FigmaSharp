using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FigmaSharp.Wpf
{
    public static class ViewConfigureExtensions
    {
        public static void Configure(this FrameworkElement view, FigmaFrameEntity child)
        {
            Configure(view, (FigmaNode)child);
            view.Opacity = child.opacity;
            if (view is Panel canvas)
            {
                canvas.Background = child.backgroundColor.ToColor();
            }
        }

        public static void Configure(this FrameworkElement view, FigmaNode child)
        {
            view.Visibility = child.visible ? Visibility.Visible : Visibility.Collapsed;

            if (child is IAbsoluteBoundingBox container)
            {
                view.Width = (int) container.absoluteBoundingBox.width;
                view.Height = (int) container.absoluteBoundingBox.height;
            }
        }

        public static void Configure(this UserControl view, FigmaElipse elipse)
        {
            Configure(view, (FigmaVectorEntity)elipse);
        }

        public static void Configure(this UserControl figmaLineView, FigmaLine figmaLine)
        {
            Configure(figmaLineView, (FigmaVectorEntity)figmaLine);

            var fills = figmaLine.fills.OfType<FigmaPaint> ().FirstOrDefault ();
            if (fills != null) {
                figmaLineView.Background = fills.color.ToColor ();
            }

            var absolute = figmaLine.absoluteBoundingBox;
            var lineWidth = absolute.width == 0 ? figmaLine.strokeWeight : absolute.width;

            figmaLineView.Width = (int)lineWidth;

            var lineHeight = absolute.height == 0 ? figmaLine.strokeWeight : absolute.height;

            figmaLineView.Height = (int)lineHeight;
        }

        public static void Configure(this FrameworkElement view, FigmaVectorEntity child)
        {
            Configure(view, (FigmaNode)child);

            if (view is Panel canvas)
            {
                if (child.HasFills && child.fills[0].color != null)
                {
                    canvas.Background = child.fills[0].color.ToColor();
                }
            }
        }

        public static void Configure(this FrameworkElement view, FigmaRectangleVector child)
        {
            Configure(view, (FigmaVectorEntity)child);

            //view.Layer.CornerRadius = child.cornerRadius;
        }

        public static void Configure(this Label label, FigmaText text)
        {
            Configure(label, (FigmaNode)text);

            label.ConfigureStyle(text.style);

            label.HorizontalAlignment = text.style.textAlignHorizontal == "CENTER" ? HorizontalAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? HorizontalAlignment.Left : HorizontalAlignment.Right;
            label.VerticalAlignment = text.style.textAlignVertical == "CENTER" ? VerticalAlignment.Center : text.style.textAlignVertical == "TOP" ? VerticalAlignment.Top : VerticalAlignment.Bottom;

            label.Opacity = text.opacity;

            var fills = text.fills.FirstOrDefault();
            if (fills != null)
            {
                label.Foreground = fills.color.ToColor();
            }
        }
    }
}
