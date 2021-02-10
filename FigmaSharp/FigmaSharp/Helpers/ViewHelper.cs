using System;
using System.Collections.Generic;
using System.Text;
using FigmaSharp.Models;


namespace FigmaSharp.Helpers
{
    class ViewHelper
    {
        public static int GetTextVerticalAlignment(FigmaText text)
        {
            FigmaTypeStyle style = text.style;

            if (style.textAlignVertical == "TOP")
            {
                return 0; // System.Windows.VerticalAlignment
            }
            if (style.textAlignVertical == "CENTER")
            {
                return 1; // System.Windows.VerticalAlignment
            }
            if (style.textAlignVertical == "BOTTOM")
            {
                return 2; // System.Windows.VerticalAlignment
            }

            return 0;
        }

        public static string GetTextFontWeight(FigmaText text)
        {
            FigmaTypeStyle style = text.style;
            return "Regular";
        }
    }

}
