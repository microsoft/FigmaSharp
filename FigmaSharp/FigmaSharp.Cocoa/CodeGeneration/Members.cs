using AppKit;
using FigmaSharp.Cocoa.Helpers;

namespace FigmaSharp.Cocoa.CodeGeneration
{
    public static class Members
    {
        public static class Colors
        {
            public static string Clear = typeof(NSColor).WithProperty(nameof(NSColor.Clear));
            public static string Red = typeof(NSColor).WithProperty(nameof(NSColor.Red));
            public static string Blue = typeof(NSColor).WithProperty(nameof(NSColor.Blue));
            public static string White = typeof(NSColor).WithProperty(nameof(NSColor.White));
            public static string Black = typeof(NSColor).WithProperty(nameof(NSColor.Black));
        }

        #region Static Resources

        public const string This = "this";

        public static class Math
        {
            public static string Max(float min, float max)
            {
                return $"{typeof(Math).FullName}.{nameof(Math.Max)}({min.ToDesignerString()}, {max.ToDesignerString()})";
            }
        }

        public static class Font
        {
            public static string SystemFontOfSize(string font)
            {
                return CodeGenerationHelpers.GetMethod(typeof(AppKit.NSFont).FullName, nameof(AppKit.NSFont.SystemFontOfSize), font);
            }

            public static string BoldSystemFontOfSize(string font)
            {
                return CodeGenerationHelpers.GetMethod(typeof(AppKit.NSFont).FullName, nameof(AppKit.NSFont.BoldSystemFontOfSize), font);
            }

            public static string SystemFontSize { get; } = $"{typeof(AppKit.NSFont).FullName}.{nameof(AppKit.NSFont.SystemFontSize)}";
            public static string SmallSystemFontSize { get; } = $"{typeof(AppKit.NSFont).FullName}.{nameof(AppKit.NSFont.SmallSystemFontSize)}";
        }

        public static class Draw
        {
            internal static string ToCGPath = "ToCGPath";
            public static class BezierPath
            {
                public static string FromOvalInRect(CoreGraphics.CGRect rect)
                {
                    return typeof(NSBezierPath).GetMethod(nameof (NSBezierPath.FromOvalInRect), rect.ToDesignerString (), false, false);
                }
            }
        }

        public static string StringEmpty { get; } = $"{typeof(string).FullName}.{nameof(string.Empty)}";

        #endregion
    }
}
