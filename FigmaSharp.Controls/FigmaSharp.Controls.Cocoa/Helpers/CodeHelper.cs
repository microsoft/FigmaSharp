﻿// Authors:
//   Hylke Bons <hylbo@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
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
using FigmaSharp.Models;

namespace FigmaSharp.Controls.Cocoa.Helpers
{
    public static class CodeHelper
	{
		public static string GetNSFontSizeString(NativeControlVariant controlVariant)
		{
			if (controlVariant == NativeControlVariant.Small)
				return $"{ typeof(NSFont) }.{ nameof(NSFont.SmallSystemFontSize) }";

			return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontSize) }";
		}

		public static string GetNSFontString(NativeControlVariant controlVariant)
		{
			if (controlVariant == NativeControlVariant.Small)
				return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ GetNSFontSizeString(controlVariant) })";

			return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ GetNSFontSizeString(controlVariant) })";
		}

		public static string GetNSFontString(NativeControlVariant controlVariant, FigmaText text, bool withWeight = true)
		{
			var fontWeight = ViewHelper.GetNSFontWeight(text);

			if (controlVariant == NativeControlVariant.Regular)
			{
				// The system default Medium is slightly different, so let Cocoa handle that
				if (fontWeight == NSFontWeight.Medium || !withWeight)
					return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ GetNSFontSizeString(controlVariant) })";
			}

			if (controlVariant == NativeControlVariant.Small)
			{
				// The system default Medium is slightly different, so let Cocoa handle that
				if (fontWeight == NSFontWeight.Medium || !withWeight)
					return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ GetNSFontSizeString(controlVariant) })";
				else
					return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ GetNSFontSizeString(controlVariant) }, { GetNSFontWeightString(text) })";
			}

			if (withWeight)
				return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ GetNSFontSizeString(controlVariant) }, { GetNSFontWeightString(text) })";
			else
				return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ GetNSFontSizeString(controlVariant) })";
		}


		public static string GetNSFontWeightString(FigmaText text)
		{
			// The default macOS font is of medium weight
			string weight = nameof(NSFontWeight.Medium);
			string fontName = text?.style?.fontPostScriptName;

			if (fontName != null)
			{
				if (fontName.EndsWith("-Black"))
					weight = nameof(NSFontWeight.Black);

				if (fontName.EndsWith("-Heavy"))
					weight = nameof(NSFontWeight.Heavy);

				if (fontName.EndsWith("-Bold"))
					weight = nameof(NSFontWeight.Bold);

				if (fontName.EndsWith("-Semibold"))
					weight = nameof(NSFontWeight.Semibold);

				if (fontName.EndsWith("-Regular"))
					weight = nameof(NSFontWeight.Regular);

				if (fontName.EndsWith("-Light"))
					weight = nameof(NSFontWeight.Light);

				if (fontName.EndsWith("-Thin"))
					weight = nameof(NSFontWeight.Thin);

				if (fontName.EndsWith("-Ultralight"))
					weight = nameof(NSFontWeight.UltraLight);
			}

			return $"{ typeof(NSFontWeight) }.{ weight }";
		}


		public static string GetNSTextAlignmentString(FigmaText text)
		{
			return $"{nameof(NSTextAlignment)}.{ViewHelper.GetNSTextAlignment(text)}";
		}

	}
}
