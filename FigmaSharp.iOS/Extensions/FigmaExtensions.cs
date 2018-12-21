using System;
using CoreGraphics;
using CoreText;
using UIKit;

namespace FigmaSharp
{
    public static class FigmaExtensions
    {
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

        public static UIColor ToUIColor(this FigmaColor color)
        {
            return UIColor.FromRGBA (color.r, color.g, color.b, color.a);
        }

        public static CGRect ToCGRect(this FigmaRectangle rectangle)
        {
            return new CGRect(rectangle.x, rectangle.y, rectangle.width, rectangle.height);
        }

        public static UIFont ToUIFont(this FigmaTypeStyle style)
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

            var font = UIFont.FromName (family, style.fontSize);
            //var w = ToAppKitFontWeight(style.fontWeight);
            //NSFontTraitMask traits = default(NSFontTraitMask);
            //if (style.fontPostScriptName != null && style.fontPostScriptName.EndsWith("-Bold"))
            //{
            //    traits = NSFontTraitMask.Bold;
            //}
            //else
            //{

            //}
            //if (font != null)
            //{
            //    var w = NSFontManager.SharedFontManager.WeightOfFont(font);
            //    var traits = NSFontManager.SharedFontManager.TraitsOfFont(font);

            //}

            //font = CTFontManager. SharedFontManager.FontWithFamily(family, traits, w, style.fontSize);
            //var font = NSFont.FromFontName(".SF NS Text", 12);

            if (font == null)
            {
                Console.WriteLine($"[ERROR] Font not found :{family}");
                font = UIFont.SystemFontOfSize (style.fontSize);
            }
            return font;
        }
    }
}
