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
using FigmaSharp.Maui;
using Microsoft.Maui.Controls;

namespace FigmaSharp.Views.Maui
{
	public class ImageButton : Button, IImageButton
	{
		public ImageButton ()
		{
			Text = "";
		}

		public ImageButton (Microsoft.Maui.Controls.Button button) : base (button)
		{

		}
	}

    public class TextBox : View, ITextBox
    {
        Microsoft.Maui.Controls.Entry entry => base.nativeView as Microsoft.Maui.Controls.Entry;

        public TextBox() : this(new Microsoft.Maui.Controls.Entry())
        {

        }
        public TextBox(Microsoft.Maui.Controls.Entry label) : base(label)
        {

        }

        public string Text
        {
            get => entry.Text;
            set => entry.Text = value;
        }

        public Color ForegroundColor
        {
            get => entry.TextColor.ToFigmaColor();
            set => entry.TextColor = value.ToMauiColor();
        }
		
        public string PlaceHolderString
		{
			get => entry.Placeholder;
			set => entry.Placeholder = value;
		}
    }

    public class Label : View, ILabel
    {
		Microsoft.Maui.Controls.Label label => base.nativeView as Microsoft.Maui.Controls.Label;

        public Label() : this(new Microsoft.Maui.Controls.Label())
        {

        }

        public Label(Microsoft.Maui.Controls.Label label) : base(label)
        {

        }

        public string Text
		{
			get => label.Text;
			set => label.Text = value;
		}

        public Color ForegroundColor {
			get => label.TextColor.ToFigmaColor();
			set => label.TextColor = value.ToMauiColor();
		}
    }

    public class ExtendedButton : Microsoft.Maui.Controls.Button
    {
        public static readonly BindableProperty HorizontalTextAlignmentProperty =
  BindableProperty.Create(nameof(HorizontalTextAlignment), typeof(Microsoft.Maui.TextAlignment), typeof(ExtendedButton), Microsoft.Maui.TextAlignment.Center);

		public Microsoft.Maui.TextAlignment HorizontalTextAlignment {
			get {
				return (Microsoft.Maui.TextAlignment)GetValue (HorizontalTextAlignmentProperty);
			}
			set {
				SetValue (HorizontalTextAlignmentProperty, value);
			}
		}
	}

	public class Button : View, IButton
	{
		public event EventHandler Clicked;

        Microsoft.Maui.Controls.Button button;

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
					extendedButton.HorizontalTextAlignment = (Microsoft.Maui.TextAlignment)(int)value;
				}
			}
		}

		public Button (Microsoft.Maui.Controls.Button button) : base (button)
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
				button.ImageSource = value.NativeObject as ImageSource;
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
			get => button.TextColor.ToFigmaColor ();
			set => button.TextColor = value.ToMauiColor ();
		}

		public override void Dispose ()
		{
			this.button.Clicked -= Button_Activated;
			base.Dispose ();
		}
	}
}
