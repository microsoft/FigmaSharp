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

using Microsoft.Maui;

namespace FigmaSharp.Maui
{
	public static class Extensions
	{
		public static Color ToFigmaColor(this Microsoft.Maui.Graphics.Color color)
		{
			return new Color((float) color.Red, (float)color.Green, (float)color.Blue, (float)color.Alpha);
		}

		public static Microsoft.Maui.Graphics.Color ToMauiColor(this Color color)
		{
			return new Microsoft.Maui.Graphics.Color((float)color.R, (float)color.G, (float)color.B, (float)color.A);
		}

		public static Microsoft.Maui.Graphics.Rect ToMauiRectangle(this Rectangle rectangle)
		{
			return new Microsoft.Maui.Graphics.Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public static Rectangle ToFigmaRectangle(this Microsoft.Maui.Graphics.Rect rectangle)
		{
			return new Rectangle((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
		}
	}
}
