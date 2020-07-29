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
using Foundation;

using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.Views.Cocoa
{
	public class Window : IWindow, IDisposable
	{
		public event EventHandler<Key> KeyDown;
		public event EventHandler Closing;
		public event EventHandler Resize;

		NSObject resizeNotification;
		FNWindow window;

		public string Title {
			get => window.Title;
			set => window.Title = value;
		}

		public bool Resizable {
			get => window.StyleMask.HasFlag (NSWindowStyle.Resizable);
			set {
				if (value) {
					if (!Resizable)
						window.StyleMask &= NSWindowStyle.Resizable;
				} else {
					if (Resizable)
						window.StyleMask &= ~NSWindowStyle.Resizable;
				}
			}
		}

		public bool IsClosable {
			get => window.StyleMask.HasFlag (NSWindowStyle.Closable);
			set {
				if (value) {
					if (!IsClosable)
						window.StyleMask &= NSWindowStyle.Closable;
				} else {
					if (IsClosable)
						window.StyleMask &= ~NSWindowStyle.Closable;
				}
			}
		}

		public bool IsFullSizeContentView {
			get => window.StyleMask.HasFlag (NSWindowStyle.FullSizeContentView);
			set {
				if (value) {
					if (!IsFullSizeContentView)
						window.StyleMask &= NSWindowStyle.FullSizeContentView;
				} else {
					if (IsFullSizeContentView)
						window.StyleMask &= ~NSWindowStyle.FullSizeContentView;
				}
			}
		}

		public void Close()
		{
			window.Close();
		}

		public bool Borderless {
			get => window.StyleMask.HasFlag (NSWindowStyle.Borderless);
			set {
				if (value) {
					if (!Borderless)
						window.StyleMask &= NSWindowStyle.Borderless;
				} else {
					if (Borderless) {
						window.StyleMask &= ~NSWindowStyle.Borderless;
					}
				}
			}
		}

		public bool MovableByWindowBackground {
			get => window.MovableByWindowBackground;
			set {
				window.MovableByWindowBackground = value;
			}
		}

		public bool IsOpaque {
			get => window.IsOpaque;
			set {
				window.IsOpaque = value;
			}
		}

		public bool ShowCloseButton {
			get => window.StandardWindowButton (NSWindowButton.CloseButton).Enabled;
			set => window.StandardWindowButton (NSWindowButton.CloseButton).Enabled = value;
		}

		public bool ShowZoomButton {
			get => window.StandardWindowButton (NSWindowButton.ZoomButton).Enabled;
			set => window.StandardWindowButton (NSWindowButton.ZoomButton).Enabled = value;
		}

		public bool ShowMiniaturizeButton {
			get => window.StandardWindowButton (NSWindowButton.MiniaturizeButton).Enabled;
			set => window.StandardWindowButton (NSWindowButton.MiniaturizeButton).Enabled = value;
		}

		public bool ShowTitle {
			get => !window.TitlebarAppearsTransparent;
			set {
				if (value) {
					if (!ShowTitle)
						window.StyleMask &= NSWindowStyle.Titled;
				} else {
					if (ShowTitle)
						window.StyleMask &= ~NSWindowStyle.Titled;
				}

				window.TitleVisibility = value ? NSWindowTitleVisibility.Visible : NSWindowTitleVisibility.Hidden;
				window.TitlebarAppearsTransparent = !value;
			}
		}

		public Window (Rectangle rectangle) : this (new FNWindow (rectangle.ToCGRect (), NSWindowStyle.Titled | NSWindowStyle.Resizable | NSWindowStyle.Miniaturizable | NSWindowStyle.Closable, NSBackingStore.Buffered, false))
		{

		}

		public Window (FNWindow window)
		{
			this.window = window;
			this.window.AutorecalculatesKeyViewLoop = true;
			Content = new View(window.ContentView);
			window.KeyDownPressed += OnKeyDownPressed;
		}

		protected virtual void OnKeyDownPressed (object sender, Key args)
		{
			KeyDown?.Invoke (this, args);
		}

		public Size Size {
			get => window.Frame.Size.ToLiteSize ();
			set {
				window.SetFrame (new CoreGraphics.CGRect (window.Frame.Location, value.ToCGSize ()), true);
			}
		}

		private void Window_WillClose (object sender, EventArgs e)
		{
			Closing?.Invoke (this, EventArgs.Empty);
		}

		IView content;
		public IView Content {
			get => content;
			set {
				content = value;

				if (content.NativeObject is NSView view)
				{
					view.TranslatesAutoresizingMaskIntoConstraints = true;

					if (this.window.ContentView != view)
					this.window.ContentView = view;
				}
			}
		}

		public object NativeObject => window;

		public Color BackgroundColor {
			get => window.BackgroundColor.ToColor ();
			set {
				window.BackgroundColor = value.ToNSColor ();
			}
		}

		public void AddChild (IWindow window)
		{
			//To implement
		}

		public void RemoveChild (IWindow window)
		{
			//To implement
		}

		public void ShowDialog ()
		{
			//To implement
		}

		public void Show ()
		{
			window.MakeKeyAndOrderFront (null);
		}

		public void Dispose ()
		{
			this.window.WillClose -= Window_WillClose;
			NSNotificationCenter.DefaultCenter.RemoveObserver (resizeNotification);
		}

		public void Center ()
		{
			window.Center ();
		}
	}
}

