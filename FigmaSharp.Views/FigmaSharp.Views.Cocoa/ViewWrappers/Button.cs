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
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.Views.Cocoa
{
	
	public class ImageButton : Button, IImageButton
	{
		public ImageButton ()
		{
			Text = "";
		}

		public ImageButton (FNSButton button) : base (button)
		{

		}
	}

	public class Button : View, IButton
	{
		public event EventHandler Clicked;

		FNSButton button;

		public bool Border { get => button.Bordered; set => button.Bordered = value; }

		public Button () : this (new FNSButton ())
		{

		}

		public Button (FNSButton button) : base (button)
		{
			this.button = button;
			this.button.TranslatesAutoresizingMaskIntoConstraints = false;
			this.button.Activated += Button_Activated;
		}

		private void Button_Activated (object sender, EventArgs e)
		{
			Clicked?.Invoke (this, EventArgs.Empty);
		}

		IImage image;
		public IImage Image {
			get => image;
			set {
				this.image = value;
				button.Image = value.NativeObject as NSImage;
			}
		}

		public string Text {
			get => button.Title;
			set {
				button.Title = value ?? "";
			}
		}

		public bool Enabled {
			get => button.Enabled;
			set => button.Enabled = value;
		}

		public override void Dispose ()
		{
			this.button.Activated -= Button_Activated;
			base.Dispose ();
		}
	}
}
