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
using FigmaSharp.Models;

namespace FigmaSharp.WinForms
{
    public static class FigmaExtensions
    {
        #region View Extensions

        public static Color ToColor(this FigmaColor color)
        {
            return Color.FromArgb ((int)(color.a * 255), (int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255));
        }

        public static FigmaColor ToFigmaColor(this Color color)
        {
            return new FigmaColor() { a = (float)color.A, r = (float)color.R, g = (float)color.G, b = (float)color.B };
        }

        public static Font ToFont(this FigmaTypeStyle style)
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
            var isBold = style.fontPostScriptName != null && style.fontPostScriptName.EndsWith ("-Bold");
            var font = new Font (family, size, isBold ? FontStyle.Bold : FontStyle.Regular);

            if (font == null)
            {
                Console.WriteLine($"[ERROR] Font not found :{family}");
                font = new Font ("Times New Roman", style.fontSize);
            }
            return font;
        }

        //public static CGPoint GetRelativePosition(this IAbsoluteBoundingBox parent, IAbsoluteBoundingBox node)
        //{
        //    return new CGPoint(
        //        node.absoluteBoundingBox.x - parent.absoluteBoundingBox.x,
        //        node.absoluteBoundingBox.y - parent.absoluteBoundingBox.y
        //    );
        //}

        //public static void CreateConstraints(this NSView view, NSView parent, FigmaLayoutConstraint constraints, FigmaRectangle absoluteBoundingBox, FigmaRectangle absoluteBoundBoxParent)
        //{
        //    System.Console.WriteLine("Create constraint  horizontal:{0} vertical:{1}", constraints.horizontal, constraints.vertical);

        //    if (constraints.horizontal.Contains("RIGHT"))
        //    {
        //        var endPosition1 = absoluteBoundingBox.x + absoluteBoundingBox.width;
        //        var endPosition2 = absoluteBoundBoxParent.x + absoluteBoundBoxParent.width;
        //        var value = Math.Max(endPosition1, endPosition2) - Math.Min(endPosition1, endPosition2);
        //        view.RightAnchor.ConstraintEqualToAnchor(parent.RightAnchor, -value).Active = true;

        //        var value2 = absoluteBoundingBox.x - absoluteBoundBoxParent.x;
        //        view.LeftAnchor.ConstraintEqualToAnchor(parent.LeftAnchor, value2).Active = true;
        //    }

        //    if (constraints.horizontal != "RIGHT")
        //    {
        //        var value2 = absoluteBoundingBox.x - absoluteBoundBoxParent.x;
        //        view.LeftAnchor.ConstraintEqualToAnchor(parent.LeftAnchor, value2).Active = true;
        //    }

        //    if (constraints.horizontal.Contains("BOTTOM"))
        //    {
        //        var value = absoluteBoundingBox.y - absoluteBoundBoxParent.y;
        //        view.TopAnchor.ConstraintEqualToAnchor(parent.TopAnchor, value).Active = true;

        //        var endPosition1 = absoluteBoundingBox.y + absoluteBoundingBox.height;
        //        var endPosition2 = absoluteBoundBoxParent.y + absoluteBoundBoxParent.height;
        //        var value2 = Math.Max(endPosition1, endPosition2) - Math.Min(endPosition1, endPosition2);

        //        view.BottomAnchor.ConstraintEqualToAnchor(parent.BottomAnchor, -value2).Active = true;
        //    }

        //    if (constraints.horizontal != "BOTTOM")
        //    {
        //        var value = absoluteBoundingBox.y - absoluteBoundBoxParent.y;
        //        view.TopAnchor.ConstraintEqualToAnchor(parent.TopAnchor, value).Active = true;
        //    }
        //}

        static int[] app_kit_font_weights = {
            2,   // FontWeight100
      3,   // FontWeight200
      4,   // FontWeight300
      5,   // FontWeight400
      6,   // FontWeight500
      8,   // FontWeight600
      9,   // FontWeight700
      10,  // FontWeight800
      12,  // FontWeight900
            };

        public static int ToAppKitFontWeight(float font_weight)
        {
            float weight = font_weight;
            if (weight <= 50 || weight >= 950)
                return 5;

            var select_weight = (int)Math.Round(weight / 100) - 1;
            return app_kit_font_weights[select_weight];
        }

        #endregion

    }
}
