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

namespace LiteForms
{
	public class Rectangle : IEquatable<Rectangle>
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Width { get; set; }
		public float Height { get; set; }

		public static Rectangle Zero { get; set; } = new Rectangle { X = 0, Y = 0, Width = 0, Height = 0 };

		public Rectangle()
		{

		}

		public Rectangle(float x, float y, float width, float height)
		{
			this.X = x;
			this.Y = y;
			this.Width = width;
			this.Height = height;
		}

		public Rectangle UnionWith(Rectangle allocation)
		{
			//TODO: improve
			float xMin = Math.Min(X, allocation.X);
			float yMin = Math.Min(Y, allocation.Y);
			float xMax = Math.Max(X + Width, allocation.X + allocation.Width);
			float yMax = Math.Max(Y + Height, allocation.Y + allocation.Height);
			return new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
		}

		public bool Equals(Rectangle other)
		{
			if (X != other.X || Y != other.Y || Width != other.Width || Height != other.Height)
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (!(obj is Rectangle)) return false;
			return Equals((Rectangle)obj);
		}

		public bool IntersectsWith(Rectangle allocation)
		{
			return (allocation.X >= X && X <= allocation.X) &&
				(allocation.Y >= Y && Y <= allocation.Y);
		}
	}

}
