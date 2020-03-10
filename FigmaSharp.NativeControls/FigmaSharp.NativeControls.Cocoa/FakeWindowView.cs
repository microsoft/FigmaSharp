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
using CoreGraphics;

namespace FigmaSharp.NativeControls.Cocoa
{
	class FakeWindowView : NSView
	{
		public FakeWindowView (string title)
		{
			WantsLayer = true;

			Layer.BorderWidth = 0.5f;
			Layer.CornerRadius = 7;

			CreateTitleBar(title);
			CreateTrafficLights();
			CreateTrafficLights();
			CreateLiveButton();
			CreateHighlight();

			UpdateLayer();
		}


		bool DarkMode { get { return (EffectiveAppearance.Name == NSAppearance.NameDarkAqua); } }


		NSBox titleBar = new NSBox();
		NSBox separator = new NSBox();
		NSTextField titleField = new NSTextField();

		void CreateTitleBar(string title)
		{
			titleBar.BorderWidth = 0;
			titleBar.BoxType = NSBoxType.NSBoxCustom;
			titleBar.CornerRadius = 6;

			titleField.StringValue = title;
			titleField.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
			titleField.Alignment = NSTextAlignment.Center;
			titleField.DrawsBackground = false;
			titleField.Editable = false;
			titleField.Bezeled = false;
			titleField.Selectable = false;

			separator.BoxType = NSBoxType.NSBoxCustom;
			separator.BorderWidth = 1;
			separator.BorderColor = NSColor.ControlShadow;

			AddSubview(titleBar);
			AddSubview(titleField);
			AddSubview(separator);
		}


		NSBox closeButton = new NSBox();
		NSBox minButton = new NSBox();
		NSBox maxButton = new NSBox();

		void CreateTrafficLights()
		{
			closeButton.BoxType = NSBoxType.NSBoxCustom;
			closeButton.FillColor = NSColor.SystemRedColor;
			closeButton.BorderColor = NSColor.Black.ColorWithAlphaComponent(0.1f);
			closeButton.CornerRadius = 6;

			minButton.BoxType = NSBoxType.NSBoxCustom;
			minButton.FillColor = NSColor.SystemYellowColor;
			minButton.BorderColor = NSColor.Black.ColorWithAlphaComponent(0.15f);
			minButton.CornerRadius = 6;

			maxButton.BoxType = NSBoxType.NSBoxCustom;
			maxButton.FillColor = NSColor.SystemGreenColor;
			maxButton.BorderColor = NSColor.Black.ColorWithAlphaComponent(0.1f);
			maxButton.CornerRadius = 6;

			AddSubview(closeButton);
			AddSubview(minButton);
			AddSubview(maxButton);
		}


		public NSButton LiveButton;

		void CreateLiveButton()
		{
			LiveButton = new NSButton()
			{
				Bordered = false,
				Hidden = true,
				Image = NSImage.ImageNamed("NSQuickLookTemplate"),
				ToolTip = "Preview in a window",
				TranslatesAutoresizingMaskIntoConstraints = false,
			};

			AddSubview(LiveButton);

			LiveButton.TopAnchor.ConstraintEqualToAnchor(TopAnchor, 5).Active = true;
			LiveButton.RightAnchor.ConstraintEqualToAnchor(RightAnchor, -7).Active = true;
		}


		// Dark mode has a light border inside the window
		NSBox highlight = new NSBox();

		void CreateHighlight()
		{
			highlight.BoxType = NSBoxType.NSBoxCustom;
			highlight.BorderColor = NSColor.Highlight.ColorWithAlphaComponent(0.4f);
			highlight.BorderWidth = 0.5f;
			highlight.CornerRadius = 4.5f;

			AddSubview(highlight);
		}


		public override void UpdateLayer()
		{
			Layer.BackgroundColor = NSColor.WindowBackground.CGColor;
			Layer.BorderColor = NSColor.ControlDarkShadow.CGColor;


			titleBar.Frame = new CGRect(0, Frame.Height - 22, Frame.Width, 22);

			if (DarkMode)
				titleBar.FillColor = NSColor.White.ColorWithAlphaComponent(0.1f);
			else
				titleBar.FillColor = NSColor.Black.ColorWithAlphaComponent(0.06f);

			titleField.Frame = new CGRect(0, Frame.Height - 25, Frame.Width, 22);


			// Traffic light positions
			int posX = 9;
			int posY = 5;
			int spacing = 8;
			int width = 12;
			int height = width;

			closeButton.Frame = new CGRect(posX, Frame.Height - posY - height, width, height);
			minButton.Frame   = new CGRect(posX + width + spacing, Frame.Height - posY - height, width, height);
			maxButton.Frame   = new CGRect(posX + width + spacing + width + spacing, Frame.Height - posY - height, width, height);

			separator.Frame = new CGRect(0, Frame.Height - 22, Frame.Width, 1);

			highlight.Frame = new CGRect(1, 1, Frame.Width - 2, Frame.Height - 2);
			highlight.Hidden = !DarkMode;


			base.UpdateLayer();
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

			var options = NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.ActiveInKeyWindow | NSTrackingAreaOptions.MouseEnteredAndExited;

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
