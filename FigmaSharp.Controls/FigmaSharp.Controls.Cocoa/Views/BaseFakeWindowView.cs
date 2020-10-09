/* 
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
using CoreAnimation;
using CoreGraphics;
using Foundation;

namespace FigmaSharp.Controls.Cocoa
{
	class BaseFakeWindowView : NSView
	{
		protected bool DarkMode { get { return (EffectiveAppearance.Name == NSAppearance.NameDarkAqua); } }


		public virtual string Title
		{
			get; set;
		}


		public BaseFakeWindowView (string title)
		{
			CreateLiveButton();
		}


		public virtual bool CloseButtonHidden
		{
			get; set;
		}

		public virtual bool MinButtonHidden
		{
			get; set;
		}

		public virtual bool MaxButtonHidden
		{
			get; set;
		}


		public NSButton LiveButton;

		public void CreateLiveButton()
		{
			LiveButton = new NSButton()
			{
				Bordered = false,
				Image = NSImage.ImageNamed("NSQuickLookTemplate"),
				ToolTip = "Preview in a window",
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			AddSubview(LiveButton);

			LiveButton.TopAnchor.ConstraintEqualToAnchor(TopAnchor, 6).Active = true;
			LiveButton.RightAnchor.ConstraintEqualToAnchor(RightAnchor, -7).Active = true;
		}


		NSTrackingArea trackingArea;

		public override void UpdateTrackingAreas()
		{
			base.UpdateTrackingAreas();

			if (trackingArea != null)
			{
				RemoveTrackingArea(trackingArea);
				trackingArea.Dispose();
			}

			var options = NSTrackingAreaOptions.MouseMoved |
			              NSTrackingAreaOptions.MouseEnteredAndExited |
			              NSTrackingAreaOptions.ActiveInActiveApp;

			trackingArea = new NSTrackingArea(Bounds, options, this, null);
			AddTrackingArea(trackingArea);
		}


		public override void MouseEntered(NSEvent theEvent)
		{
			base.MouseEntered(theEvent);

			LiveButton.Hidden = false;
		}

		public override void MouseExited(NSEvent theEvent)
		{
			base.MouseExited(theEvent);

			LiveButton.Hidden = true;
		}
	}
}
