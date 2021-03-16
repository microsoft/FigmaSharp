// You may change the methods in this file to return your own resources.
// This file will NOT be regenerated by FigmaSharp.

using AppKit;
using Foundation;

namespace NAMESPACE
{
    public static class Resources
    {
        // Return your translations here
        public static string GetString(this string s)
        {
            return s;
        }


        // Return your !icon resources here
        public static NSImage GetIcon(this string iconName)
        {
            return NSImage.ImageNamed(iconName);
        }


        // Return your !image resources here
        public static NSImage GetImage(this string imageName)
        {
            return NSImage.ImageNamed(imageName);
        }


        // Return your color resources here
        public static NSColor GetColor(this string colorName)
        {
            return NSColor.FromName(colorName, NSBundle.MainBundle);
        }
    }
}

