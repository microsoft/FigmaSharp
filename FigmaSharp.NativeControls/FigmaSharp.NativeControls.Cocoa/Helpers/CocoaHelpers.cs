// Authors:
//   hbons <hylbo@microsoft.com>
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

using System;
using System.Linq;

using AppKit;

using FigmaSharp.Models;

namespace FigmaSharp.NativeControls.Cocoa
{
	public static class CocoaHelpers
	{
		public static NSControlSize GetNSControlSize(NativeControlVariant controlVariant)
		{
			if (controlVariant == NativeControlVariant.Small)
				return NSControlSize.Small;

			return NSControlSize.Regular;
		}


		public static nfloat GetNSFontSize(NativeControlVariant controlVariant)
		{
			if (controlVariant == NativeControlVariant.Small)
				return NSFont.SmallSystemFontSize;

			return NSFont.SystemFontSize;
		}


		public static NSFont GetNSFont(NativeControlVariant controlVariant)
		{
			if (controlVariant == NativeControlVariant.Small)
				return NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize);

			return NSFont.SystemFontOfSize(NSFont.SystemFontSize);
		}

		public static NSFont GetNSFont(NativeControlVariant controlVariant, FigmaText text)
		{
			var fontWeight = GetNSFontWeight(text);

			if (controlVariant == NativeControlVariant.Regular)
			{
				// The system default Medium is slightly different, so let Cocoa handle that
				if (fontWeight == NSFontWeight.Medium)
					return NSFont.SystemFontOfSize(NSFont.SystemFontSize);
			}

			if (controlVariant == NativeControlVariant.Small)
			{
				// The system default Medium is slightly different, so let Cocoa handle that
				if (fontWeight == NSFontWeight.Medium)
					return NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize);
				else
					return NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize, fontWeight);
			}

			return NSFont.SystemFontOfSize(GetNSFontSize(controlVariant), fontWeight);
		}


		public static nfloat GetNSFontWeight(FigmaText text)
		{
			string fontName = text?.style?.fontPostScriptName;

			if (fontName != null)
			{
				if (fontName.EndsWith("-Black"))
					return NSFontWeight.Black;

				if (fontName.EndsWith("-Heavy"))
					return NSFontWeight.Heavy;

				if (fontName.EndsWith("-Bold"))
					return NSFontWeight.Bold;

				if (fontName.EndsWith("-Semibold"))
					return NSFontWeight.Semibold;

				if (fontName.EndsWith("-Regular"))
					return NSFontWeight.Regular;

				if (fontName.EndsWith("-Light"))
					return NSFontWeight.Light;

				if (fontName.EndsWith("-Thin"))
					return NSFontWeight.Thin;

				if (fontName.EndsWith("-Ultralight"))
					return NSFontWeight.UltraLight;
			}

			// The default macOS font is of medium weight
			return NSFontWeight.Medium;
		}


		public static NSTextAlignment GetNSTextAlignment(FigmaText text)
		{
			FigmaTypeStyle style = text.style;

			if (style.textAlignHorizontal == "RIGHT")
				return NSTextAlignment.Right;

			if (style.textAlignHorizontal == "CENTER")
				return NSTextAlignment.Center;

			return NSTextAlignment.Left;
		}


		public static NSColor GetNSColor(string colorStyleName)
		{
			return ColorService.ThemeColors.FirstOrDefault(c => c.StyleName == colorStyleName).Color;
		}
	}
}
