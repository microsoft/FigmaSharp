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
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.Views.Cocoa
{
	public class TextBox : View, ITextBox
	{
		FNSTextField textField;
		public event EventHandler Changed;

		public TextBox () : this (new FNSTextField ())
		{

		}

		public TextBox (FNSTextField textField) : base (textField)
		{
			this.textField = textField;
            this.textField.TranslatesAutoresizingMaskIntoConstraints = false;
			textField.Changed += TextField_Changed;
		}

		private void TextField_Changed (object sender, EventArgs e)
		{
			Changed?.Invoke (this, EventArgs.Empty);
		}

		public string Text {
			get => textField.StringValue;
			set {
				textField.StringValue = value ?? "";
			}
		}

		public string PlaceHolderString {
			get => textField.PlaceholderString;
			set {
				textField.PlaceholderString = value ?? "";
			}
		}

		public Color ForegroundColor { get => textField.TextColor.ToColor (); set => textField.TextColor = value.ToNSColor (); }

		public override void Dispose ()
		{
			textField.Changed -= TextField_Changed;
			base.Dispose ();
		}
	}
}
