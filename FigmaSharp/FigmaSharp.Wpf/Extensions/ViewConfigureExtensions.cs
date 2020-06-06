using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using FigmaSharp.Extensions;
using FigmaSharp.Models;
using FigmaSharp.Views.Wpf;

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

        public static void Configure(this TextBlock label, FigmaText text)
        {
            Configure(label, (FigmaNode)text);
             
            label.TextAlignment = text.style.textAlignHorizontal == "CENTER" ? TextAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? TextAlignment.Left : TextAlignment.Right;
            
            // textblock doesn't support vertical text alignment, unfortunately
            label.VerticalAlignment = text.style.textAlignVertical == "CENTER" ? VerticalAlignment.Center : text.style.textAlignVertical == "TOP" ? VerticalAlignment.Top : VerticalAlignment.Bottom;

            label.Opacity = text.opacity;

            text.style.FillEmptyStylePropertiesWithDefaults(text);

            if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0)
            {
                foreach(var overrideStyle in text.styleOverrideTable.Values)
                {
                    overrideStyle.FillEmptyStylePropertiesWithDefaults(text);
                }

                var chars = text.characters.ToCharArray();

                // since we can expect in most cases a style will continue for multiple chars, 
                // there's an optimization to be made here

                int i = 0;
                for (; i < text.characterStyleOverrides.Length; i++)
                { 
                    var run = new Run(chars[i].ToString());

                    var key = text.characterStyleOverrides[i].ToString();
                    if (text.styleOverrideTable.ContainsKey(key))
                    {
                        //if there is a style to override
                        var styleOverride = text.styleOverrideTable[key];
                        run.ConfigureStyle(styleOverride); 
                    }
                    else
                    {
                        //we want the default values
                        run.ConfigureStyle(text.style); 
                    } 

                    label.Inlines.Add(run);
                }

                var remainingChars = chars.Skip(i);
                var lastRun = new Run(string.Concat(remainingChars));
                lastRun.ConfigureStyle(text.style);
                label.Inlines.Add(lastRun);
            }
            else
            {
                var run = new Run(text.characters);
                run.ConfigureStyle(text.style);
                label.Inlines.Add(run);
            }
        }
    }
}
