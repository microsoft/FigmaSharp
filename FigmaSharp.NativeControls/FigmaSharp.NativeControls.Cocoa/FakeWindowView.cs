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
			CreateLiveButton();
			CreateHighlight();
		}

		bool DarkMode { get { return (EffectiveAppearance.Name == NSAppearance.NameDarkAqua); } }

		NSBox titleBar = new NSBox();
		NSBox separator = new NSBox();
		NSTextField titleField = new NSTextField();

		void CreateTitleBar(string title)
		{
			titleBar.BorderWidth = 0;
			titleBar.BoxType = NSBoxType.NSBoxCustom;

			titleField.StringValue = title;
			titleField.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
			titleField.Alignment = NSTextAlignment.Center;
			titleField.DrawsBackground = false;
			titleField.Editable = false;
			titleField.Bezeled = false;
			titleField.Selectable = false;

			separator.BoxType = NSBoxType.NSBoxCustom;
			separator.BorderWidth = 1;

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
			closeButton.BorderColor = NSColor.Black.ColorWithAlphaComponent(0.1f);
			closeButton.CornerRadius = 6;

			minButton.BoxType = NSBoxType.NSBoxCustom;
			minButton.BorderColor = NSColor.Black.ColorWithAlphaComponent(0.1f);
			minButton.CornerRadius = 6;

			maxButton.BoxType = NSBoxType.NSBoxCustom;
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
			base.UpdateLayer();

			// Window
			Layer.BackgroundColor = NSColor.WindowBackground.CGColor;

			if (DarkMode)
				Layer.BorderColor = NSColor.ControlDarkShadow.CGColor;
			else
				Layer.BorderColor = NSColor.WindowFrame.CGColor;


			// Window buttons
			if (DarkMode) {
				closeButton.FillColor = NSColor.FromRgb(255, 90,  82); // #FF5A52
				minButton.FillColor   = NSColor.FromRgb(230, 192, 41); // #E6C029
				maxButton.FillColor   = NSColor.FromRgb(83,  194, 43); // #53C22B
			} else {
				closeButton.FillColor = NSColor.FromRgb(255, 95,  87); // #FF5F57
				minButton.FillColor   = NSColor.FromRgb(255, 189, 46); // #FFBD2E
				maxButton.FillColor   = NSColor.FromRgb(40,  201, 64); // #28C940
			}


			// Title bar
			if (DarkMode)
				titleBar.FillColor = NSColor.White.ColorWithAlphaComponent(0.1f);
			else
				titleBar.FillColor = NSColor.Black.ColorWithAlphaComponent(0.08f);

			titleField.TextColor = NSColor.Text.ColorWithAlphaComponent(0.8f);


			// Separator
			if (DarkMode)
				separator.BorderColor = NSColor.Black.ColorWithAlphaComponent(0.5f);
			else
				separator.BorderColor = NSColor.Black.ColorWithAlphaComponent(0.12f);


			// Highlight
			highlight.Hidden = !DarkMode;
		}


		public override void SetFrameSize(CGSize newSize)
		{
			base.SetFrameSize(newSize);


			// Title bar
			titleBar.Frame = new CGRect(0, Frame.Height - 22, Frame.Width, 22);
			titleField.Frame = new CGRect(0, Frame.Height - 25, Frame.Width, 22);


			// Traffic lights
			int posX = 9, posY = 5;
			int width = 12, height = width;
			int spacing = 8;

			closeButton.Frame = new CGRect(posX, Frame.Height - posY - height, width, height);
			minButton.Frame = new CGRect(posX + width + spacing, Frame.Height - posY - height, width, height);
			maxButton.Frame = new CGRect(posX + width + spacing + width + spacing, Frame.Height - posY - height, width, height);


			// Live button
			LiveButton.Hidden = true;


			// Separator
			separator.Frame = new CGRect(0, Frame.Height - 22, Frame.Width, 1);


			// Highlight
			highlight.Frame = new CGRect(1, 1, Frame.Width - 2, Frame.Height - 2);
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
			              NSTrackingAreaOptions.ActiveInKeyWindow |
			              NSTrackingAreaOptions.MouseEnteredAndExited;

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
