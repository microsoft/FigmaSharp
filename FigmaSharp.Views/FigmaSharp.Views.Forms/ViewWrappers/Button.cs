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
using Xamarin.Forms;

namespace FigmaSharp.Views.Forms
{
	public class ImageButton : Button, IImageButton
	{
		public ImageButton ()
		{
			Text = "";
		}

		public ImageButton (Xamarin.Forms.Button button) : base (button)
		{

		}
	}

	public class ExtendedButton : Xamarin.Forms.Button
	{
		public static BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create<ExtendedButton, Xamarin.Forms.TextAlignment> (x => x.HorizontalTextAlignment, Xamarin.Forms.TextAlignment.Center);
		public Xamarin.Forms.TextAlignment HorizontalTextAlignment {
			get {
				return (Xamarin.Forms.TextAlignment)GetValue (HorizontalTextAlignmentProperty);
			}
			set {
				SetValue (HorizontalTextAlignmentProperty, value);
			}
		}
	}

	public class Button : View, IButton
	{
		public event EventHandler Clicked;

		Xamarin.Forms.Button button;

		public float BorderWidth {
			get => (float)button.BorderWidth;
			set {
				button.BorderWidth = value;
			}
		}

		public bool Border {
			get => button.BorderWidth > 0;
			set => button.BorderWidth = value ? BorderWidth : 0;
		}

		public Button () : this (new ExtendedButton ())
		{

		}

		public TextAlignment TextHorizontalAlignment {
			get {
				if (button is ExtendedButton extendedButton) {
					return (TextAlignment)(int)extendedButton.HorizontalTextAlignment;
				}
				return TextAlignment.Center;
			}
			set {
				if (button is ExtendedButton extendedButton) {
					extendedButton.HorizontalTextAlignment = (Xamarin.Forms.TextAlignment)value;
				}
			}
		}

		public Button (Xamarin.Forms.Button button) : base (button)
		{
			this.button = button;
			this.button.Clicked += Button_Activated;
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
				button.ImageSource = value.NativeObject as Xamarin.Forms.ImageSource;
			}
		}

		public string Text {
			get => button.Text;
			set {
				button.Text = value ?? "";
			}
		}

		public bool Enabled {
			get => button.IsEnabled;
			set => button.IsEnabled = value;
		}

		public Color TextColor {
			get => button.TextColor.ToLiteColor ();
			set => button.TextColor = value.ToFormsColor ();
		}

		public override void Dispose ()
		{
			this.button.Clicked -= Button_Activated;
			base.Dispose ();
		}
	}
}
