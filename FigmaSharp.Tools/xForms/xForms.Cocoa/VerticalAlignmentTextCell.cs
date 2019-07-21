using System;
using AppKit;
using CoreGraphics;
using Foundation;

namespace LiteForms.Cocoa
{
	public enum VerticalTextAlignment
	{
		Top,
		Center,
		Bottom
	}

	public class VerticalAlignmentTextCell : NSTextFieldCell
	{
		public VerticalTextAlignment VerticalAligment { get; set; } = VerticalTextAlignment.Top;

		public VerticalAlignmentTextCell()
		{
		}

		public VerticalAlignmentTextCell(NSCoder coder) : base(coder)
		{
		}

		public VerticalAlignmentTextCell(string aString) : base(aString)
		{
		}

		public VerticalAlignmentTextCell(NSImage image) : base(image)
		{
		}

		protected VerticalAlignmentTextCell(NSObjectFlag t) : base(t)
		{
		}

		protected internal VerticalAlignmentTextCell(IntPtr handle) : base(handle)
		{
		}

		public override CGRect DrawingRectForBounds(CGRect theRect)
		{
			var newRect = base.DrawingRectForBounds(theRect);
			if (VerticalAligment == VerticalTextAlignment.Top)
				return newRect;

			var textSize = CellSizeForBounds(theRect);
			//Center in the proposed rect
			float heightDelta = (float)(newRect.Size.Height - textSize.Height);
			float height = (float)newRect.Height;
			float y = (float)newRect.Y;
			if (heightDelta > 0)
			{
				height -= heightDelta;
				if (VerticalAligment == VerticalTextAlignment.Bottom)
					y += heightDelta;
				else
					y += heightDelta / 2f;
			}
			return new CGRect(newRect.X, y, newRect.Width, height);
		}
	}
}
