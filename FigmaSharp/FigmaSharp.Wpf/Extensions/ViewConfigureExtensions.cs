using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FigmaSharp.Models;

namespace FigmaSharp.Wpf
{
    public static class ViewConfigureExtensions
    {
        public static void Configure(this FrameworkElement view, FigmaFrame child)
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
                view.Width = (int) container.absoluteBoundingBox.Width;
                view.Height = (int) container.absoluteBoundingBox.Height;
            }
        }

        public static void Configure(this FrameworkElement view, FigmaElipse child)
        {
            Configure(view, (FigmaVectorEntity)child);

            if (view is Panel canvas)
            {
                canvas.Children.Add(new Ellipse
                {
                    Width = canvas.Width,
                    Height = canvas.Height,
                    Fill = canvas.Background
                });

                canvas.Background = Brushes.Transparent;
            }
        }

        public static void Configure(this FrameworkElement view, FigmaLine child)
        {
            Configure(view, (FigmaVectorEntity)child);

            if (view is Panel canvas)
            {
                if (child.HasStrokes && child.strokes[0].color != null)
                {
                    canvas.Background = child.strokes[0].color.ToColor();
                }
            }

            var absolute = child.absoluteBoundingBox;
            var lineWidth = absolute.Width == 0 ? child.strokeWeight : absolute.Width;

            view.Width = (int)lineWidth;

            var lineHeight = absolute.Height == 0 ? child.strokeWeight : absolute.Height;

            view.Height = (int)lineHeight;
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
         
        public static void Configure(this FrameworkElement view, RectangleVector child)
        {
            Configure(view, (FigmaVectorEntity)child);

            //view.Layer.CornerRadius = child.cornerRadius;
        }

        public static void Configure(this Label label, FigmaText text)
        {
            Configure(label, (FigmaNode)text);

            label.HorizontalContentAlignment = text.style.textAlignHorizontal == "CENTER" ? HorizontalAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? HorizontalAlignment.Left : HorizontalAlignment.Right;
            label.VerticalContentAlignment = text.style.textAlignVertical == "CENTER" ? VerticalAlignment.Center : text.style.textAlignVertical == "TOP" ? VerticalAlignment.Top : VerticalAlignment.Bottom;

            label.HorizontalAlignment = HorizontalAlignment.Center;
            string family = text.style.fontFamily;
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
                Console.WriteLine("FONT: {0} - {1}", family, text.style.fontPostScriptName);
            } 
            label.FontFamily = new FontFamily(family);
            label.FontSize = text.style.fontSize;// -3 ;
            label.FontWeight = FontWeight.FromOpenTypeWeight(text.style.fontWeight);
            if (text.style.letterSpacing > 0)
            {
                label.FontStretch = FontStretch.FromOpenTypeStretch(text.style.letterSpacing > 9 ? 9 : (int)text.style.letterSpacing);
            }

            label.Opacity = text.opacity;

            var fills = text.fills.FirstOrDefault();
            if (fills != null)
            {
                label.Foreground = fills.color.ToColor();
            } 
        }
    }
}
