/* 
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
using AppKit;
using CoreGraphics;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Native.Cocoa;

namespace LocalFile.Cocoa
{
	public class FixedFlatButton : FNSButton
	{
		const int FixedButtonWidth = 263;
		const int FixedButtonHeight = 48;

		NSView logo;
		NSTextField label;

		void RecalculateSizes()
		{
			if (logo != null)
			{
				logo.Frame = new CGRect(14, (FixedButtonHeight / 2) - (logo.Frame.Height / 2), logo.Frame.Width, logo.Frame.Height);
			}

			if (label != null)
			{
				nfloat fontY = label.IntrinsicContentSize.Height / 2f;
				var leftPadding = logo != null ? 15 : 6;
				label.SetFrameOrigin(new CGPoint(leftPadding + (FixedButtonWidth / 2) - (label.IntrinsicContentSize.Width / 2), (FixedButtonHeight / 2) - fontY));
			}
		}

		public FixedFlatButton(string title, NSView logo = null)
		{
			Alignment = NSTextAlignment.Center;
			WantsLayer = true;
			BezelStyle = NSBezelStyle.ShadowlessSquare;
			ShowsBorderOnlyWhileMouseInside = true;
			Layer.CornerRadius = 3;
			BackgroundColor = NSColor.Clear;
			BorderColor = NSColor.White;
			BorderWidth = 1.0f;
			Title = "";

			if (logo != null)
			{
				this.logo = logo;
				AddSubview(logo);
			}

			label = ViewsHelper.CreateLabel(title);
			label.StringValue = title;
			label.Alignment = NSTextAlignment.Center;
			label.TextColor = NSColor.White;
			label.Font = NSFont.SystemFontOfSize(15);
			label.SetFrameSize(label.IntrinsicContentSize);
			AddSubview(label);

			//WidthAnchor.ConstraintEqualToConstant(FixedButtonWidth).Active = true;
			//HeightAnchor.ConstraintEqualToConstant(FixedButtonHeight).Active = true;
			RecalculateSizes();
		}

		public override CGSize IntrinsicContentSize => new CGSize(FixedButtonWidth, FixedButtonHeight);

		NSColor backgroundColor = NSColor.ControlBackground;
		public NSColor BackgroundColor
		{
			get => backgroundColor;
			set
			{
				backgroundColor = value;
				Layer.BackgroundColor = value.CGColor;
			}
		}

		public NSColor BorderColor
		{
			get => NSColor.FromCGColor(Layer.BorderColor);
			set => Layer.BorderColor = value.CGColor;
		}

		public nfloat BorderWidth
		{
			get => Layer.BorderWidth;
			set => Layer.BorderWidth = value;
		}
	}
}

