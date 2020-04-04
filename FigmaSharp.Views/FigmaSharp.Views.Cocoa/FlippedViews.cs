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

	public class FNSTextView : NSTextView, IFlippedView
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
