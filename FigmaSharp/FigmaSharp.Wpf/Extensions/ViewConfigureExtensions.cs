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
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using FigmaSharp.Controls;
using FigmaSharp.Extensions;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views.Wpf;

using PseudoLocalizer;

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
                view.MaxWidth = (int) container.absoluteBoundingBox.Width;
                view.MaxHeight = (int) container.absoluteBoundingBox.Height;
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
         
        public static void Configure(this FrameworkElement view, RectangleVector child, ViewNode parent)
        {
            Configure(view, (FigmaVector)child);
        }

        public static void ConfigureAcceleratorKey(this Button button, string text, string key)
        {
            button.Content = getAcceleratorText(text, key);
        }

        public static void ConfigureAcceleratorKey(this CheckBox checkbox, string text, string key)
        {
            checkbox.Content = getAcceleratorText(text, key);
        }

        public static void ConfigureAcceleratorKey(this Label label, string text, string key)
        {
            label.Content = getAcceleratorText(text, key);
        }

        private static string getAcceleratorText(string text, string key)
        {
            var keyIndex = text.IndexOf(key);
            if (keyIndex > -1)
            {
                text = text.Insert(keyIndex, "_");
            }
            return text;
        }

        public static void ConfigureAutomationProperties(this DependencyObject control, FigmaFrame figmaFrame)
        {
            if (figmaFrame.TrySearchA11Label(out var label))
            {
                if (label != null)
                {
                    AutomationProperties.SetName(control, label);
                }
            }

            if (figmaFrame.TrySearchA11Help(out var help))
            {
                if (help != null)
                {
                    AutomationProperties.SetHelpText(control, help);
                }
            }
        }

        public static void ConfigureTooltip(this FrameworkElement frameworkElement, FigmaFrame figmaFrame)
        {
            if (figmaFrame.TrySearchTooltip(out var tooltip))
            {
                if (tooltip != null)
                {
                    frameworkElement.ToolTip = tooltip.PseudoLocalize();
                }
            }
        }

        public static void ConfigureTabIndex(this Control control, FigmaFrame figmaFrame)
        {
            if (figmaFrame.TrySearchTabIndex(out var index))
            {
                if(index != null)
                {
                    control.TabIndex = Int16.Parse(index);
                    
                }
            }
        }

        public static void configureAlignment(this FrameworkElement frameworkElement, ViewNode parent)
        {
            if (parent.View.NativeObject.GetType() == typeof(StackPanel))
            {
                var frame = (FigmaFrame)parent.Node;
                if (frame.LayoutMode != FigmaLayoutMode.None)
                {
                    Console.WriteLine("Layout Mode: {0}", frame.layoutMode);
                    Console.WriteLine("Primary alignment: {0}", frame.PrimaryAxisAlignment);
                    Console.WriteLine("Counter alignment: {0}", frame.CounterAxisAlignment);
                    frameworkElement.HorizontalAlignment = GetHorizontalAlignment(frame.LayoutMode, frame.PrimaryAxisAlignment, frame.CounterAxisAlignment);
                    frameworkElement.VerticalAlignment = GetVerticalAlignment(frame.LayoutMode, frame.PrimaryAxisAlignment, frame.CounterAxisAlignment);
                    frameworkElement.Margin = GetMargins(frame);
                }
            }
        }

        public static Thickness GetMargins(FigmaFrame frame)
        {
            var margins = new Thickness();
            if (frame.LayoutMode == FigmaLayoutMode.Horizontal)
            {
                margins.Top = frame.paddingTop;
                margins.Bottom = frame.paddingBottom;
                margins.Left = frame.itemSpacing / 2;
                margins.Right = frame.itemSpacing / 2;
                
            }
            if (frame.LayoutMode == FigmaLayoutMode.Vertical)
            {
                margins.Left = frame.paddingLeft;
                margins.Right = frame.paddingRight;
                margins.Top = frame.itemSpacing / 2;
                margins.Bottom = frame.itemSpacing / 2;
            }
            return margins;
        }

        public static HorizontalAlignment GetHorizontalAlignment(FigmaLayoutMode layoutMode, FigmaAxisAlignment primaryAxisAlignment, FigmaAxisAlignment counterAxisAlignment)
        {
            if(layoutMode != FigmaLayoutMode.None)
            {
                if(layoutMode == FigmaLayoutMode.Horizontal)
                {
                    if(primaryAxisAlignment == FigmaAxisAlignment.MIN)
                    {
                        return HorizontalAlignment.Left;
                    }
                    else if(primaryAxisAlignment == FigmaAxisAlignment.CENTER)
                    {
                        return HorizontalAlignment.Center;
                    }
                    else if(primaryAxisAlignment == FigmaAxisAlignment.MAX)
                    {
                        return HorizontalAlignment.Right;
                    }
                }
                else if(layoutMode == FigmaLayoutMode.Vertical)
                {
                    if (counterAxisAlignment == FigmaAxisAlignment.MIN)
                    {
                        return HorizontalAlignment.Left;
                    }
                    else if (counterAxisAlignment == FigmaAxisAlignment.CENTER)
                    {
                        return HorizontalAlignment.Center;
                    }
                    else if (counterAxisAlignment == FigmaAxisAlignment.MAX)
                    {
                        return HorizontalAlignment.Right;
                    }
                } 
            }
            return HorizontalAlignment.Left;
        }

        public static VerticalAlignment GetVerticalAlignment(FigmaLayoutMode layoutMode, FigmaAxisAlignment primaryAxisAlignment, FigmaAxisAlignment counterAxisAlignment)
        {
            if (layoutMode != FigmaLayoutMode.None)
            {
                if (layoutMode == FigmaLayoutMode.Vertical)
                {
                    if (primaryAxisAlignment == FigmaAxisAlignment.MIN)
                    {
                        return VerticalAlignment.Top;
                    }
                    else if (primaryAxisAlignment == FigmaAxisAlignment.CENTER)
                    {
                        return VerticalAlignment.Center;
                    }
                    else if (primaryAxisAlignment == FigmaAxisAlignment.MAX)
                    {
                        return VerticalAlignment.Bottom;
                    }
                }
                else if (layoutMode == FigmaLayoutMode.Horizontal)
                {
                    if (counterAxisAlignment == FigmaAxisAlignment.MIN)
                    {
                        return VerticalAlignment.Top;
                    }
                    else if (counterAxisAlignment == FigmaAxisAlignment.CENTER)
                    {
                        return VerticalAlignment.Center;
                    }
                    else if (counterAxisAlignment == FigmaAxisAlignment.MAX)
                    {
                        return VerticalAlignment.Bottom;
                    }
                }
                
            }
            return VerticalAlignment.Bottom;
        }

        public static void ConfigureGridPosition(this Control control, FigmaFrame figmaFrame, ViewNode parent)
        {
            if (parent.View.NativeObject.GetType() == typeof(Grid))
            {
                if (figmaFrame.TryGetGridRowPosition(out int rowPosition))
                {
                    if (rowPosition > -1)
                    {
                        control.SetValue(Grid.RowProperty, rowPosition);
                    }
                }
                if (figmaFrame.TryGetGridColumnPosition(out int columnPosition))
                {
                    if (columnPosition > -1)
                    {
                        control.SetValue(Grid.ColumnProperty, columnPosition);
                    }
                }
            }
        }

        public static void Configure(this Slider slider, FigmaFrame frame)
        {
            Configure((FrameworkElement)slider, frame);

            slider.Maximum = 1;
            slider.Minimum = 0;
            slider.Value = 0.5;

            var orientation = frame.name.Split('/');
            if(orientation.Length > 1)
            {
                slider.Orientation = GetOrientation(orientation[1]);
            }
        }

        public static void Configure(this StackPanel stackPanel, FigmaFrame frame)
        {
            Configure((FrameworkElement)stackPanel, frame);
            var orientation = frame.name.Split('/');
            if (orientation.Length > 1)
            {
                stackPanel.Orientation = GetOrientation(orientation[1]);
            }
        }

        public static Orientation GetOrientation (string orientation)
        {
            if (orientation == "Vertical")
            {
                return Orientation.Vertical;
            }
            else
            {
                return Orientation.Horizontal;
            }
        }

        public static void Configure(this TextBlock label, FigmaText text)
        {
            //label.MaxWidth = (int)text.absoluteBoundingBox.Width;
            //label.MaxHeight = (int)text.absoluteBoundingBox.Height;
            label.MaxWidth = text.absoluteBoundingBox.Width;
            label.MaxHeight = text.absoluteBoundingBox.Height + text.style.lineHeightPx;

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

        public static void Configure(this Label label, FigmaText text)
        {
            label.MaxWidth = (int)text.absoluteBoundingBox.Width;
            label.MaxHeight = (int)text.absoluteBoundingBox.Height;
            //Configure(label, (FigmaNode)text);
            label.HorizontalContentAlignment = text.style.textAlignHorizontal == "CENTER" ? HorizontalAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? HorizontalAlignment.Left : HorizontalAlignment.Right;

            // textblock doesn't support vertical text alignment, unfortunately
            label.VerticalAlignment = text.style.textAlignVertical == "CENTER" ? VerticalAlignment.Center : text.style.textAlignVertical == "TOP" ? VerticalAlignment.Top : VerticalAlignment.Bottom;

            label.Opacity = text.opacity;

            label.FontSize = text.style.fontSize;
            label.FontWeight = FontWeight.FromOpenTypeWeight(text.style.fontWeight);
            label.Content = text.characters;
            label.Padding = new Thickness(0);
        }
    }
}
