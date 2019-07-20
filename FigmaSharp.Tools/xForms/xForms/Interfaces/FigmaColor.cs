/* 
 * FigmaImageView.cs - NSImageView which stores it's associed Figma Id
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

namespace FigmaSharp.Models
{
	public class FigmaColor
	{
		public float r { get; set; }
		public float g { get; set; }
		public float b { get; set; }
		public float a { get; set; }
	}

	public class FigmaRectangle : IEquatable<FigmaRectangle>
	{
		public float x { get; set; }
		public float y { get; set; }
		public float width { get; set; }
		public float height { get; set; }

		public static FigmaRectangle Zero { get; set; } = new FigmaRectangle { x = 0, y = 0, width = 0, height = 0 };

		public FigmaRectangle()
		{

		}

		public FigmaRectangle(float x, float y, float width, float height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public FigmaRectangle UnionWith(FigmaRectangle allocation)
		{
			//TODO: improve
			float xMin = Math.Min(x, allocation.x);
			float yMin = Math.Min(y, allocation.y);
			float xMax = Math.Max(x + width, allocation.x + allocation.width);
			float yMax = Math.Max(y + height, allocation.y + allocation.height);
			return new FigmaRectangle(xMin, yMin, xMax - xMin, yMax - yMin);
		}

		public bool Equals(FigmaRectangle other)
		{
			if (x != other.x || y != other.y || width != other.width || height != other.height)
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (!(obj is FigmaRectangle)) return false;
			return Equals((FigmaRectangle)obj);
		}
	}

}
