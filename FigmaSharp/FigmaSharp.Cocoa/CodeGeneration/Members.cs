using FigmaSharp.Cocoa.Helpers;

namespace FigmaSharp.Cocoa.CodeGeneration
{
    public static class Members
    {
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

        public static string StringEmpty { get; } = $"{typeof(string).FullName}.{nameof(string.Empty)}";

        #endregion
    }
}
