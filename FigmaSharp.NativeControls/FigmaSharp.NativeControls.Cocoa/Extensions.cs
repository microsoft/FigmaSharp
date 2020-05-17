//
// Author:
//   jmedrano <josmed@microsoft.com>
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
//
using System;
using FigmaSharp.Models;
using FigmaSharp.Services;

namespace FigmaSharp.NativeControls.Cocoa
{
	public static class Extensions
	{
		public static bool TryGetStringColorByStyleKey(this FigmaText figmaText, IFigmaFileProvider fileProvider, IColorConverter colorConverter, string styleKey, out string stringColor)
		{
			foreach (var style in figmaText.styles)
			{
				try
				{
					if (fileProvider.TryGetStyle(style.Value, out FigmaStyle fillStyle))
					{
						if (style.Key == styleKey)
						{
							stringColor = colorConverter.FromStyleToStringColor(fillStyle.name);
							return stringColor == null;
						}
						continue;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}

			stringColor = null;
			return stringColor == null;
		}

		public static bool TryGetNSColorByStyleKey(this FigmaText figmaText, IFigmaFileProvider fileProvider, IColorConverter colorConverter, string styleKey, out AppKit.NSColor color)
		{
			foreach (var style in figmaText.styles)
			{
				try
				{
					if (fileProvider.TryGetStyle(style.Value, out FigmaStyle fillStyle))
					{
						if (style.Key == styleKey)
						{
							color = colorConverter.FromStyleToColor(fillStyle.name) as AppKit.NSColor;
							return color != null;
						}
						continue;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
			color = null;
			return color == null;
		}
	}
}
