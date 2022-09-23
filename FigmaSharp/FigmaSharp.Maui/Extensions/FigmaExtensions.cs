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
using Microsoft.Maui.Controls;
using FigmaSharp.Models;
using Microsoft.Maui;

namespace FigmaSharp.Maui
{
    public static class FigmaExtensions
    {
		public static View FindNativeViewByName(this Services.ViewRenderService rendererService, string name)
		{
			foreach (var node in rendererService.NodesProcessed)
			{
				if (node.Node.name == name)
				{
					return (View)node.View.NativeObject;
				}
			}
			return null;
		}

		public static Color MixOpacity(this Microsoft.Maui.Graphics.Color color, float opacity)
        {
            return new Color (
                color.Red, color.Green, color.Blue,
                 Math.Min(color.Alpha, opacity)
            );
        }

        public static Color MixOpacity(this FigmaSharp.Color color, float opacity)
        {
            return new Color(
                (float)color.R, (float)color.G, (float)color.B,
                 (float)Math.Min(color.A, opacity)
            );
        }

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

        //public static Color ToColor(this FigmaSharp.Views.Color color)
        //{
        //    return new Color(color.R, color.G, color.B, color.A);
        //}

        //public static FigmaSharp.Views.Color ToFigmaColor(this Color color)
        //{
        //    return new FigmaSharp.Views.Color() { A = (float)color.A, R = (float)color.R, G = (float)color.G, B = (float)color.B };
        //}

        //public static Rectangle ToCGRect(this FigmaSharp.Views.Rectangle rectangle)
        //{
        //    return new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        //}

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

            Font font;
            //var font =  FromName (family, style.fontSize);
            //var w = ToAppKitFontWeight(style.fontWeight);
            //NSFontTraitMask traits = default(NSFontTraitMask);
            if (style.fontPostScriptName?.EndsWith("-Bold") ?? false)
            {
                font = Font.SystemFontOfSize(style.fontSize, FontWeight.Bold);
            }
            else
            {
                font = Font.SystemFontOfSize(style.fontSize);
            }
            //if (font != null)
            //{
            //    var w = NSFontManager.SharedFontManager.WeightOfFont(font);
            //    var traits = NSFontManager.SharedFontManager.TraitsOfFont(font);

            //}

            //font = CTFontManager. SharedFontManager.FontWithFamily(family, traits, w, style.fontSize);
            //var font = NSFont.FromFontName(".SF NS Text", 12);

            Console.WriteLine($"[ERROR] Font not found :{family}");
            //font = UIFont.SystemFontOfSize (style.fontSize);
            return font;
        }
    }
}
