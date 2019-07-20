using System;
using System.Collections.Generic;
using FigmaSharp.Models;
using Xamarin.Forms;

namespace FigmaSharp.Forms
{
    public static class FigmaExtensions
    {
		public static View FindNativeViewByName(this Services.FigmaRendererService rendererService, string name)
		{
			foreach (var node in rendererService.NodesProcessed)
			{
				if (node.FigmaNode.name == name)
				{
					return (View)node.View.NativeObject;
				}
			}
			return null;
		}

		public static Xamarin.Forms.Color MixOpacity(this Xamarin.Forms.Color color, float opacity)
        {
            return new Xamarin.Forms.Color (
                color.R, color.G, color.B,
                 Math.Min(color.A, opacity)
            );
        }

        public static FigmaSharp.Color MixOpacity(this FigmaSharp.Color color, float opacity)
        {
            return new FigmaSharp.Color(
                color.R, color.G, color.B,
                 Math.Min(color.A, opacity)
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
                font = Font.SystemFontOfSize(style.fontSize, FontAttributes.Bold);
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
