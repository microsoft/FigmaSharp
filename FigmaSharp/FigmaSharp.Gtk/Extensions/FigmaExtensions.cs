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
using System.Text;
using Gtk;
using FigmaSharp.Models;

namespace FigmaSharp.GtkSharp
{
    public static class FigmaExtensions
    {
        public static Label CreateLabel(string text)
        {
            var label = new Label() { Text = text ?? "" };
            return label;
        }

        #region View Extensions

        public static Gdk.Color ToGdkColor(this FigmaColor color)
        {
            return new Gdk.Color (GetByteColor(color.r), GetByteColor (color.g), GetByteColor (color.b));
        }

        public static FigmaColor ToFigmaColor(this Gdk.Color color)
        {
            return new FigmaColor() { 
                a = (float)1, 
                r = (float)color.Red / 255f, 
                g = (float)color.Green/255f, 
                b = (float)color.Blue/255f };
        }

        static byte GetByteColor (double color)
        {
           return (byte)(color * 255);
        }

        static byte GetFigmaColor(double color)
        {
            return (byte)(color * 255);
        }

        public static string ToDesignerString(this float value)
        {
            return string.Concat (value.ToString(),"f");
        }

        public static string ToDesignerString(this int value)
        {
            return string.Concat(value.ToString());
        }

        //public static NSTextAlignment ToNSTextAlignment(string value)
        //{
        //    return value == "CENTER" ? NSTextAlignment.Center : value == "LEFT" ? NSTextAlignment.Left : NSTextAlignment.Right;
        //}

        //public static string ToDesignerString(this NSTextAlignment alignment)
        //{
        //    return string.Format ("{0}.{1}", nameof(NSTextAlignment), alignment.ToString());
        //}

        public static string ToDesignerString(this FigmaColor color)
        {
            return $"new Gdk.Color({color.r.ToDesignerString ()}, {color.g.ToDesignerString ()}, {color.b.ToDesignerString ()}, {color.a.ToDesignerString ()})";
        }

        public static string ToDesignerString(this bool value)
        {
            return value ? "true" : "false";
        }

        public static Gdk.Rectangle ToRectangle(this FigmaRectangle rectangle)
        {
            return new Gdk.Rectangle(0, 0, (int) rectangle.width, (int)rectangle.height);
        }

        //public static string ToDesignerString(this NSFontTraitMask mask)
        //{
        //    if (mask.HasFlag (NSFontTraitMask.Bold))
        //    {
        //        return string.Format("{0}.{1}", nameof(NSFontTraitMask), nameof (NSFontTraitMask.Bold));
        //    }
        //    return "default(NSFontTraitMask)";
        //    //return string.Format("{0}.{1}", nameof(NSFontTraitMask), mask.ToString());
        //}

        //public static string ToNSFontDesignerString(this FigmaTypeStyle style)
        //{
        //    var font = style.ToNSFont();
        //    var family = font.FamilyName;
        //    var size = font.PointSize;
        //    var w = NSFontManager.SharedFontManager.WeightOfFont(font);
        //    var traits = NSFontManager.SharedFontManager.TraitsOfFont(font);
        //    return string.Format("NSFontManager.SharedFontManager.FontWithFamily(\"{0}\", {1}, {2}, {3})", family, traits.ToDesignerString (), w, style.fontSize);
        //}

        #endregion

    }
}
