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
	public class Slider : View, ISlider
	{
		FNSSlider slider;

		public float MaxValue {
			get => (float)slider.MaxValue;
			set {
				slider.MaxValue = value;
			}
		}

		public float MinValue {
			get => (float)slider.MinValue;
			set {
				slider.MinValue = value;
			}
		}

		public event EventHandler ValueChanged;

		public float Value {
			get => (float)slider.DoubleValue;
			set {
				slider.DoubleValue = value;
				ValueChanged?.Invoke (this, EventArgs.Empty);
			}
		}

		public Slider () : this (new FNSSlider ())
		{

		}

		public Slider (FNSSlider slider) : base (slider)
		{
			this.slider = slider;
            this.slider.TranslatesAutoresizingMaskIntoConstraints = false;
			this.slider.Activated += Slider_Activated;

		}

		private void Slider_Activated (object sender, EventArgs e)
		{
			ValueChanged?.Invoke (this, EventArgs.Empty);
		}

		public string Text {
			get => slider.StringValue;
			set {
				slider.StringValue = value ?? "";
			}
		}

		public override void Dispose ()
		{
			this.slider.Activated -= Slider_Activated;
			base.Dispose ();
		}
	}
}
