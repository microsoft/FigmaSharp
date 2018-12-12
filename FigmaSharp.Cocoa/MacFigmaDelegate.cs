using System;
using System.Reflection;
using AppKit;

namespace FigmaSharp.Cocoa
{
    public static class MacFigmaDelegate
    {
        //public MacFigmaDelegate()
        //{
        //}

        public static MacImageWrapper GetImage (string url)
        {
            var image = new NSImage(new Foundation.NSUrl(url));
            return new MacImageWrapper(image);
        }

        public static IImageWrapper GetImageFromManifest (Assembly assembly, string imageRef)
        {
            var assemblyImage = FigmaViewsHelper.GetManifestImageResource(assembly, string.Format("{0}.png", imageRef));
            return new MacImageWrapper (assemblyImage);
        }

        public static IImageWrapper GetImageFromFilePath(string filePath)
        {
           var image = new NSImage(filePath);
           return new MacImageWrapper(image);
        }

        public static IImageViewWrapper GetImageView(FigmaPaint figmaPaint)
        {
            return new MacImageViewWrapper(new NSImageView())
            {
                 Data = figmaPaint
            };
       }
    }
}
