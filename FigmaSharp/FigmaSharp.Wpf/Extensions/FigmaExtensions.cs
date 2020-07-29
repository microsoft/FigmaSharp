﻿// Authors:
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
using System.Windows.Media; 
using System.Windows.Documents;

using FigmaSharp.Models;
using FigmaSharp.Views.Wpf;

namespace FigmaSharp.Wpf
{
    public static class FigmaExtensions
    {
        #region View Extensions

        public static void ConfigureStyle(this TextElement textElement, FigmaTypeStyle style)
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
            textElement.FontFamily = new FontFamily(family);

            if(style.fontSize > 0)
                textElement.FontSize = style.fontSize;// -3 ;

            textElement.FontWeight = FontWeight.FromOpenTypeWeight(style.fontWeight);
            if (style.letterSpacing != default)
            {
                textElement.FontStretch = FontStretch.FromOpenTypeStretch(style.letterSpacing > 9 ? 9 : (int)style.letterSpacing);
            }

            var fill = style.fills.FirstOrDefault();
            if (fill != null)
            {
                textElement.Foreground = fill.color.ToColor();
            }
        }

        #endregion
    }
}
