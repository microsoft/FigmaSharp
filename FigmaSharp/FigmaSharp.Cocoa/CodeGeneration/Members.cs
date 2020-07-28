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
