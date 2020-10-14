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

namespace FigmaSharp.Views.Native.Cocoa
{
	public interface IFlippedView
	{
		bool MouseDownMovesWindow { get; set; }
	}

	public class FNSSearchField : NSSearchField, IFlippedView
	{
		public override bool IsFlipped => true;

		public bool MouseDownMovesWindow { get; set; }
		public override bool MouseDownCanMoveWindow => MouseDownMovesWindow;
	}

	public class FNSBox : NSBox, IFlippedView
	{
		public override bool IsFlipped => true;

		public bool MouseDownMovesWindow { get; set; }
		public override bool MouseDownCanMoveWindow => MouseDownMovesWindow;
	}

	public class FNSProgressIndicator : NSProgressIndicator, IFlippedView
	{
		public override bool IsFlipped => true;

		public bool MouseDownMovesWindow { get; set; }
		public override bool MouseDownCanMoveWindow => MouseDownMovesWindow;
	}

	public class FNWindow : NSWindow
	{
		public event EventHandler<Key> KeyDownPressed;

		bool IsMovableByWindowBackground;
		public override bool MovableByWindowBackground {
			get => IsMovableByWindowBackground;
			set => IsMovableByWindowBackground = value;
		}

		public FNWindow ()
		{
		}

		public FNWindow (NSCoder coder) : base (coder)
		{
		}

		public FNWindow (CGRect contentRect, NSWindowStyle aStyle, NSBackingStore bufferingType, bool deferCreation) : base (contentRect, aStyle, bufferingType, deferCreation)
		{
		}

		public FNWindow (CGRect contentRect, NSWindowStyle aStyle, NSBackingStore bufferingType, bool deferCreation, NSScreen screen) : base (contentRect, aStyle, bufferingType, deferCreation, screen)
		{
		}

		protected FNWindow (NSObjectFlag t) : base (t)
		{
		}

		protected internal FNWindow (IntPtr handle) : base (handle)
		{
		}

		public override void KeyDown (NSEvent theEvent)
		{
			KeyDownPressed?.Invoke (this, (Key)theEvent.KeyCode);
			base.KeyDown (theEvent);
		}
	}

	public class FNSView : NSView
	{
		public override bool IsFlipped => true;

		public bool MouseDownMovesWindow { get; set; }
		public override bool MouseDownCanMoveWindow => MouseDownMovesWindow;

	}
	public class FNSScrollview : NSScrollView, IFlippedView
	{
		public bool MouseDownMovesWindow { get; set; }
		public override bool MouseDownCanMoveWindow => MouseDownMovesWindow;

		public override bool IsFlipped => true;
	}
	public class FNSTextField : NSTextField, IFlippedView
	{
		public bool MouseDownMovesWindow { get; set; }
		public override bool MouseDownCanMoveWindow => MouseDownMovesWindow;

		public override bool IsFlipped => true;
	}

	public class FNSTextView : NSScrollView, IFlippedView
	{
		public bool MouseDownMovesWindow { get; set; }
		public override bool MouseDownCanMoveWindow => MouseDownMovesWindow;

		public override bool IsFlipped => true;
	}

	public class FNSTabView : NSTabView, IFlippedView
	{
		public bool MouseDownMovesWindow { get; set; }
		public override bool MouseDownCanMoveWindow => MouseDownMovesWindow;

		public override bool IsFlipped => true;
	}

	public class FNSButton : NSButton, IFlippedView
	{
		public bool MouseDownMovesWindow { get; set; }
		public override bool MouseDownCanMoveWindow => MouseDownMovesWindow;

		public override bool IsFlipped => true;
	}

	public class FNSStepper : NSStepper, IFlippedView
	{
		public bool MouseDownMovesWindow { get; set; }
		public override bool MouseDownCanMoveWindow => MouseDownMovesWindow;

		public override bool IsFlipped => true;
	}

	public class FNSPopUpButton : NSPopUpButton, IFlippedView
	{
		public bool MouseDownMovesWindow { get; set; }
		public override bool MouseDownCanMoveWindow => MouseDownMovesWindow;

		public override bool IsFlipped => true;
	}

	public class FNSImageView : NSImageView, IFlippedView
	{
		public bool MouseDownMovesWindow { get; set; }
		public override bool MouseDownCanMoveWindow => MouseDownMovesWindow;

		public override bool IsFlipped => true;
	}

	public class FNSStackView : NSStackView, IFlippedView
	{
		public bool MouseDownMovesWindow { get; set; }
		public override bool MouseDownCanMoveWindow => MouseDownMovesWindow;

		public override bool IsFlipped => true;
	}

	public class FNSSlider : NSSlider, IFlippedView
	{
		public bool MouseDownMovesWindow { get; set; }
		public override bool MouseDownCanMoveWindow => MouseDownMovesWindow;

		public override bool IsFlipped => true;
	}
}
