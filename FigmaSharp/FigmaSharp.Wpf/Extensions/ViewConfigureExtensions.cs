// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

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
                canvas.Background = child.fills[0].color.ToColor();
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
            Configure(view, (FigmaVector)child);

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
            Configure(view, (FigmaVector)child);

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

        public static void Configure(this FrameworkElement view, FigmaVector child)
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
            Configure(view, (FigmaVector)child);

            //view.Layer.CornerRadius = child.cornerRadius;
        }

        public static void ConfigureAcceleratorKey(this Button button, string text, string key)
        {
            var keyIndex = text.IndexOf(key);
            if (keyIndex > -1)
            {
                text = text.Insert(keyIndex, "_");
                button.Content = text;
            }
        }
        public static void Configure(this Button button, FigmaFrame frame)
        {
            button.MaxWidth = frame.absoluteBoundingBox.Width;
            button.MaxHeight = frame.absoluteBoundingBox.Height;
        }

        public static void Configure(this TextBox textBox, FigmaFrame frame)
        {
            textBox.MaxWidth = frame.absoluteBoundingBox.Width;
            textBox.MaxHeight = frame.absoluteBoundingBox.Height;
        }

        public static void Configure(this CheckBox checkbox, FigmaFrame frame)
        {
            checkbox.MaxWidth = frame.absoluteBoundingBox.Width;
            checkbox.MaxHeight = frame.absoluteBoundingBox.Height;
        }

        public static void Configure(this ComboBox comboBox, FigmaFrame frame)
        {
            comboBox.MaxWidth = frame.absoluteBoundingBox.Width;
            comboBox.MaxHeight = frame.absoluteBoundingBox.Height;
        }

        public static void Configure(this RadioButton radioButton, FigmaFrame frame)
        {
            radioButton.MaxWidth = frame.absoluteBoundingBox.Width;
            radioButton.MaxHeight = frame.absoluteBoundingBox.Height;
        }

        public static void Configure(this Slider slider, FigmaFrame frame)
        {
            slider.MaxWidth = frame.absoluteBoundingBox.Width;
            slider.MaxHeight = frame.absoluteBoundingBox.Height;

            slider.Maximum = 1;
            slider.Minimum = 0;
            slider.Value = 0.5;

            var orientation = frame.name.Split('/');
            if(orientation.Length > 0)
            {
                if (orientation[1] == "Vertical") 
                {
                    slider.Orientation = Orientation.Vertical;
                }
                else
                {
                    slider.Orientation = Orientation.Horizontal;
                }
            }
        }

        public static void Configure(this ProgressBar progressBar, FigmaVector frame)
        {
            progressBar.MaxWidth = frame.absoluteBoundingBox.Width;
            progressBar.MaxHeight = frame.absoluteBoundingBox.Height;
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
