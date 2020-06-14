using System;
using FigmaSharp.Views.Cocoa.Graphics;
using FigmaSharp.Views.Helpers;

namespace FigmaSharp.Views.Cocoa
{
    public class SvgView : View, ISvgView
    {
        public void Load(string image)
        {
            ((SvgFile)nativeView).Load(image);
            Console.WriteLine(image);
        }

        public SvgView() : base (new SvgFile ())
        {
           
        }
    }
}
