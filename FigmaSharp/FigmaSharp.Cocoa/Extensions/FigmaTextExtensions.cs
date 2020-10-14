// Authors:
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
using System;
using FigmaSharp.Models;

namespace FigmaSharp.Services
{
    public static class StyleExtensions
    {
		public static bool TryGetStringColorByStyleKey(this IFigmaStyle figmaText, INodeProvider nodeProvider, IColorService colorService, string styleKey, out string stringColor)
		{
			if (figmaText.styles != null)
            {
				foreach (var style in figmaText.styles)
				{
					try
					{
						if (nodeProvider.TryGetStyle(style.Value, out FigmaStyle fillStyle))
						{
							if (style.Key == styleKey)
							{
								stringColor = colorService.GetStringColorFromStyle(fillStyle.name);
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
			}
			
			stringColor = null;
			return false;
		}

		public static bool TryGetStringColorByStyleKey(this IFigmaStyle figmaText, CodeRenderService codeRenderService, string styleKey, out string stringColor)
			=> figmaText.TryGetStringColorByStyleKey(codeRenderService.NodeProvider, codeRenderService.ColorService, styleKey, out stringColor);

		public static bool TryGetNSColorByStyleKey(this IFigmaStyle figmaText, ViewRenderService renderService, string styleKey, out AppKit.NSColor color)
			=> figmaText.TryGetNSColorByStyleKey(renderService.NodeProvider, renderService.ColorService, styleKey, out color);

		public static bool TryGetNSColorByStyleKey(this IFigmaStyle figmaText, INodeProvider fileProvider, IColorService colorConverter, string styleKey, out AppKit.NSColor color)
		{
			if (figmaText.styles != null)
            {
				foreach (var style in figmaText.styles)
				{
					try
					{
						if (fileProvider.TryGetStyle(style.Value, out FigmaStyle fillStyle))
						{
							if (style.Key == styleKey)
							{
								color = colorConverter.GetColorFromStyle(fillStyle.name) as AppKit.NSColor;
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
			}
			
			color = null;
			return false;
		}
	}
}
