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

using AppKit;
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.Views.Cocoa
{
	public class Stepper : View, IStepper
	{
		FNSStepper stepper;

		public Stepper () : this (new FNSStepper ())
		{

		}

		public Stepper (FNSStepper stepper) : base (stepper)
		{
			this.stepper = stepper;
            this.stepper.TranslatesAutoresizingMaskIntoConstraints = false;
		}
	}

	public class DisclosureTriangle : Button, IDisclosureTriangle
	{
		public DisclosureTriangle () : this (new FNSButton ())
		{

		}

		public DisclosureTriangle (FNSButton disclosureButton) : base (disclosureButton)
		{
			disclosureButton.TranslatesAutoresizingMaskIntoConstraints = false;
			disclosureButton.SetButtonType (NSButtonType.PushOnPushOff);
			disclosureButton.BezelStyle = NSBezelStyle.Disclosure;
			disclosureButton.Title = string.Empty;
			disclosureButton.Highlight (false);
		}
	}

	public class Spinner : View, ISpinner
	{
		FNSProgressIndicator progressIndicator;

		public Spinner () : this (new FNSProgressIndicator ())
		{

		}

		public Spinner (FNSProgressIndicator progressIndicator) : base (progressIndicator)
		{
			this.progressIndicator = progressIndicator;
			this.progressIndicator.TranslatesAutoresizingMaskIntoConstraints = false;
			this.progressIndicator.Style = NSProgressIndicatorStyle.Spinning;
		}

		public void Start ()
		{
			progressIndicator.StartAnimation (null);
		}

		public void Stop ()
		{
			progressIndicator.StopAnimation (null);
		}

		public override void Dispose ()
		{
			Stop ();
			base.Dispose ();
		}
	}


	public class ProgressBar : View, IProgressBar
	{
		FNSProgressIndicator progressIndicator;

		public ProgressBar() : this(new FNSProgressIndicator())
		{

		}

		public ProgressBar(FNSProgressIndicator progressIndicator) : base(progressIndicator)
		{
			this.progressIndicator = progressIndicator;
			this.progressIndicator.TranslatesAutoresizingMaskIntoConstraints = false;
			progressIndicator.Style = NSProgressIndicatorStyle.Bar;
		}
	}
}
