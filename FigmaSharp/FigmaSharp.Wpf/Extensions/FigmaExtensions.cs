/* 
 * FigmaViewExtensions.cs - Extension methods for NSViews
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FigmaSharp.Converters;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using FigmaSharp.Models;

namespace FigmaSharp.Wpf
{
    public static class FigmaExtensions
    {
        #region View Extensions

        public static Brush ToColor(this Color color)
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(color.A * 255), (byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255)));
        }

        public static Color ToFigmaColor(this Brush color)
        {
            if (color is SolidColorBrush solidColor)
            {
                return new Color() {
                    A = (float)solidColor.Color.A,
                    R = (float)solidColor.Color.R,
                    G = (float)solidColor.Color.G,
                    B = (float)solidColor.Color.B
                };
            }
            return new Color();
        }

        public static Color ToFigmaColor(this Color color)
        {
            return new Color() { A = (float)color.A, R = (float)color.R, G = (float)color.G, B = (float)color.B };
        }

        public static void ConfigureStyle (this Label label, FigmaTypeStyle style)
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

            var size = style.fontSize - 3;
            var isBold = style.fontPostScriptName != null && style.fontPostScriptName.EndsWith("-Bold");

            label.FontSize = size;
            label.FontFamily = new FontFamily(family);
            label.FontWeight = isBold ?  FontWeights.Bold : FontWeights.Regular;
        }

        #endregion
    }
}
