
using AppKit;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Cocoa.Graphics;
using FigmaSharp.Views.Helpers;

namespace BasicGraphics.Cocoa
{
    class ExampleSvgShapeView : View
    {
        static SvgFile GetFile (string file)
        {
            var data = FileHelper.GetFileDataFromBundle(file);
            var svg = new SvgFile(data);
            return svg;
        }

        public ExampleSvgShapeView(string file) : base(GetFile (file))
        {
            
        }
    }
}
