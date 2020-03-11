﻿/* 
 * Author:
 *   Hylke Bons <hylbo@microsoft.com>
 *
 * Copyright (C) 2020 Microsoft, Corp
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

using AppKit;
using CoreGraphics;

namespace FigmaSharp.NativeControls.Cocoa
{
	class FakeSheetView : NSView
	{
		public FakeSheetView()
		{
			WantsLayer = true;
			Layer.BorderWidth = 0.5f;

			CreateHighlight();
		}


		bool DarkMode { get { return (EffectiveAppearance.Name == NSAppearance.NameDarkAqua); } }


		// Dark mode has a light border inside the window
		NSBox highlight = new NSBox();

		void CreateHighlight()
		{
			highlight.BoxType = NSBoxType.NSBoxCustom;
			highlight.BorderColor = NSColor.Highlight.ColorWithAlphaComponent(0.4f);
			highlight.BorderWidth = 0.5f;

			AddSubview(highlight);
		}


		public override void UpdateLayer()
		{
			base.UpdateLayer();

			if (DarkMode)
				Layer.BorderColor = NSColor.ControlDarkShadow.CGColor;
			else
				Layer.BorderColor = NSColor.WindowFrame.CGColor;

			Layer.BackgroundColor = NSColor.WindowBackground.ColorWithAlphaComponent(0.66f).CGColor;


			// Highlight
			highlight.Hidden = !DarkMode;
		}


		public override void SetFrameSize(CGSize newSize)
		{
			base.SetFrameSize(newSize);

			// Highlight
			highlight.Frame = new CGRect(1, 1, Frame.Width - 2, Frame.Height - 2);
		}
	}
}
