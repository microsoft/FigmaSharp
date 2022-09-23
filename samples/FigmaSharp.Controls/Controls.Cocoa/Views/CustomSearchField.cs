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

using System;

using AppKit;
using CoreGraphics;
using Foundation;

using FigmaSharp.Views.Native.Cocoa;

namespace LocalFile.Cocoa
{
	class CustomSearchField : FNSSearchField
	{
		const int FocusRingHorizontalSeparation = 1;
		public override void DrawFocusRingMask() => NSBezierPath.FillRect(FocusRingMaskBounds);
		public override CGRect FocusRingMaskBounds => new CGRect(Bounds.X + FocusRingHorizontalSeparation, Bounds.Y, Bounds.Width - (FocusRingHorizontalSeparation * 2), Bounds.Height);

		const int LeftContentPadding = 8;
		const int TopContentPadding = 5;

		const int DefaultWidth = 188;
		const int DefaultHeight = 24;

		public class SearchFieldCell : NSSearchFieldCell
		{
			CustomSearchField customSearchField;

			public SearchFieldCell(CustomSearchField customSearchField)
			{
				this.customSearchField = customSearchField;
				SearchButtonCell.Transparent = true;
				Bezeled = false;
				BezelStyle = NSTextFieldBezelStyle.Rounded;
				Editable = true;
			}

			public override void DrawInteriorWithFrame(CGRect cellFrame, NSView inView)
			{
				var bounds = DrawingRectForBounds(cellFrame);
				bounds.Width += ContentWidthDelta * 2;
				bounds.Y -= (ContentHeightDelta / 2);
				base.DrawInteriorWithFrame(bounds, inView);
			}

			public override void SelectWithFrame(CGRect aRect, NSView inView, NSText editor, NSObject delegateObject, nint selStart, nint selLength)
			{
				var bounds = DrawingRectForBounds(aRect);
				base.SelectWithFrame(bounds, inView, editor, delegateObject, selStart, selLength);
			}

			public override CGRect DrawingRectForBounds(CGRect theRect)
			{
				CGRect baseRect = base.DrawingRectForBounds(theRect);
				baseRect.X = LeftContentPadding;
				baseRect.Y = TopContentPadding;
				baseRect.Width = DefaultWidth - (LeftContentPadding * 2) - ContentWidthDelta;
				baseRect.Height = DefaultHeight - TopContentPadding - ContentHeightDelta;
				return baseRect;
			}

			const int ContentHeightDelta = 2;
			const int ContentWidthDelta = 10;
		}

		readonly SearchFieldCell cell;

		public CustomSearchField()
		{
			TranslatesAutoresizingMaskIntoConstraints = false;

			Cell = cell = new SearchFieldCell(this);
			WantsLayer = true;

			var internalLayer = new CoreAnimation.CALayer();
			internalLayer.BorderWidth = 1;
			internalLayer.BorderColor = NSColor.FromRgba(red: 0.75f, green: 0.75f, blue: 0.75f, 0.45f).CGColor;
			internalLayer.Frame = new CGRect(1, 0, DefaultWidth + 2, DefaultHeight);
			internalLayer.CornerRadius = 2;
			Layer.AddSublayer(internalLayer);

			Font = NSFont.SystemFontOfSize(SearchFontSize, NSFontWeight.Thin);

			//default size needs to be changed before change cell 
			WidthAnchor.ConstraintGreaterThanOrEqualTo(DefaultWidth).Active = true;
			HeightAnchor.ConstraintGreaterThanOrEqualTo(DefaultHeight).Active = true;
		}

		public static nfloat SearchFontSize { get; internal set; } = 11;
	}
}

