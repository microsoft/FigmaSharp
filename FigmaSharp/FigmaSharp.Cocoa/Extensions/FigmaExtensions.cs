/* 
 * FigmaViewExtensions.cs - Extension methods for NSViews
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;

using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;

using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Views;

namespace FigmaSharp.Cocoa
{
    public static class FigmaExtensions
    {
        public static NSView FindNativeViewByName(this Services.FigmaRendererService rendererService, string name)
        {
            foreach (var node in rendererService.NodesProcessed)
            {
                if ( node.FigmaNode.name == name)
                {
                    return (NSView)node.View.NativeObject;
                }
            }
            return null;
        }

        public static NSTextField CreateLabel(string text, NSFont font = null, NSTextAlignment alignment = NSTextAlignment.Left)
        {
            var label = new NSTextField()
            {
                StringValue = text ?? "",
                Font = font ?? Views.Cocoa.ViewsHelper.GetSystemFont(false),
                Editable = false,
                Bordered = false,
                Bezeled = false,
                DrawsBackground = false,
                Selectable = false,
                Alignment = alignment
            };
            label.TranslatesAutoresizingMaskIntoConstraints = false;
            return label;
        }

        #region View Extensions

        public static string ToDesignerString(this float value)
        {
            return string.Concat (value.ToString(),"f");
        }

        public static string ToDesignerString(this double value)
        {
            return string.Concat(value.ToString(), "f");
        }

        public static NSTextAlignment ToNSTextAlignment(string value)
        {
            return value == "CENTER" ? NSTextAlignment.Center : value == "LEFT" ? NSTextAlignment.Left : NSTextAlignment.Right;
        }

        public static string ToDesignerString(this NSTextAlignment alignment)
        {
            return string.Format ("{0}.{1}", nameof(NSTextAlignment), alignment.ToString());
        }

        public static string ToDesignerString(this Color color, bool cgColor = false)
        {
            var cg = cgColor ? ".CGColor" : "";
            return $"NSColor.FromRgba({color.R.ToDesignerString ()}, {color.G.ToDesignerString ()}, {color.B.ToDesignerString ()}, {color.A.ToDesignerString ()}){cg}";
        }

        public static string ToDesignerString(this bool value)
        {
            return value ? "true" : "false";
        }

        public static string ToDesignerString(this NSFontTraitMask mask)
        {
            if (mask.HasFlag (NSFontTraitMask.Bold))
            {
                return string.Format("{0}.{1}", nameof(NSFontTraitMask), nameof (NSFontTraitMask.Bold));
            }
            return string.Format ("default({0})", nameof (NSFontTraitMask));
            //return string.Format("{0}.{1}", nameof(NSFontTraitMask), mask.ToString());
        }

		public static string CreateLabelToDesignerString(string text, NSTextAlignment alignment = NSTextAlignment.Left, Func<string, (string data,bool translated)> tranlationHandler = null)
		{
			StringBuilder builder = new StringBuilder();

            text = text ?? string.Empty;

            var isMultiline = text.Contains('\n');
            if (isMultiline) {
                text = string.Format("@\"{0}\"", text.Replace("\"", "\"\""));
            }

            bool translated = false;
            if (tranlationHandler != default) {
                var result = tranlationHandler.Invoke(text);
                translated = result.translated;
                text = result.data;
            }

            if (!translated && !isMultiline)
                text = $"\"{text}\"";

            builder.Append(string.Format("new {0}() {{", typeof(NSTextField).FullName));
			builder.AppendLine(string.Format("StringValue = {0},", text));
			builder.AppendLine("Editable = false,");
			builder.AppendLine("Bordered = false,");
			builder.AppendLine("Bezeled = false,");
			builder.AppendLine("DrawsBackground = false,");
			builder.AppendLine(string.Format("Alignment = {0},", alignment.ToDesignerString()));
			builder.Append("}");
			return builder.ToString();
		}

		public static string ToNSFontDesignerString(this FigmaTypeStyle style)
        {
            //var font = style.ToNSFont();
            //var family = font.FamilyName;
            var size = style.fontSize;

            //NSFont.SystemFontOfSize (style.fontSize);

            //var viewName = $"{typeof (NSFont).FullName}.{nameof (NSFontManager.SharedFontManager)}";
            //return CodeGenerationHelpers.GetMethod (typeof (NSFont).FullName, nameof (NSFont.SystemFontOfSize), $"\"{family}\", {size}", includesSemicolon: false);
            return CodeGenerationHelpers.GetMethod (typeof (NSFont).FullName, nameof (NSFont.SystemFontOfSize), size.ToString (), includesSemicolon: false);
        }

        static nfloat GetFontWeight (FigmaTypeStyle style)
        {
            if (style.fontPostScriptName != null)
            {
                if (style.fontPostScriptName.EndsWith("-Bold"))
                {
                    return NSFontWeight.Regular;
                }
                if (style.fontPostScriptName.EndsWith("-Light"))
                {
                    return NSFontWeight.Light;
                }
                if (style.fontPostScriptName.EndsWith("-Thin"))
                {
                    return NSFontWeight.Thin;
                }
                if (style.fontPostScriptName.EndsWith("-SemiBold"))
                {
                    return NSFontWeight.Semibold;
                }
            }

            return NSFontWeight.Regular;
        }

        static Dictionary<string, string> FontConversion = new Dictionary<string, string>()
        {
            { "SF UI Text", ".SF NS Text" },
            { "SF Mono", ".SF NS Display" }
        };

        public static NSFont ToNSFont(this FigmaTypeStyle style)
        {
            string family = style.fontFamily;
            NSFont font;

            try {
                font = NSFont.FromFontName (style.fontFamily, style.fontSize);
				if (font == null)
                    font = NSFont.SystemFontOfSize (style.fontSize, GetFontWeight (style));

            } catch (Exception ex) {
                Console.WriteLine ($"Font not found in system: {family} .. using system default font.");
                Console.WriteLine (ex);
                font = NSFont.SystemFontOfSize (style.fontSize, GetFontWeight (style));
            }

            return font;

            //if (FontConversion.TryGetValue (family, out string newFamilyName))
            //{
            //    Console.WriteLine("{0} font was in the conversion dicctionary and was replaced by {1}.", family, newFamilyName);
            //    family = newFamilyName;
            //}

            //var fontDefault = NSFont.SystemFontOfSize(style.fontSize, GetFontWeight(style));
            //var traits = NSFontManager.SharedFontManager.TraitsOfFont(fontDefault);
            //var weight = Math.Max (Views.Cocoa.ViewsHelper.ToAppKitFontWeight(style.fontWeight) - 2,1);

            //NSFont font = null;
            //try
            //{
            //    font = NSFontManager.SharedFontManager.FontWithFamily(family, traits, weight, style.fontSize);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}

            //if (font == null)
            //{
            //    try
            //    {
            //        font = NSFontManager.SharedFontManager.FontWithFamily(fontDefault.FamilyName, traits, weight, style.fontSize);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex);
            //    }
            //}
           // return font;
        }

        public static CGPoint GetRelativePosition(this IAbsoluteBoundingBox parent, IAbsoluteBoundingBox node)
        {
            return new CGPoint(
                node.absoluteBoundingBox.X - parent.absoluteBoundingBox.X,
                node.absoluteBoundingBox.Y - parent.absoluteBoundingBox.Y
            );
        }

        #endregion

    }
}
