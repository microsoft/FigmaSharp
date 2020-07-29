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
	public class StackView : View, IStackView
	{
		public Padding Padding {
			get {
				return new Padding ((float)stackView.EdgeInsets.Top, (float)stackView.EdgeInsets.Left, (float)stackView.EdgeInsets.Bottom, (float)stackView.EdgeInsets.Right);
			}
			set {
				stackView.EdgeInsets = new NSEdgeInsets (value.Top, value.Left, value.Bottom, value.Right);
			}
		}

		public float ItemSpacing {
			get => (float)stackView.Spacing;
			set => stackView.Spacing = value;
		}

		FNSStackView stackView;

		public LayoutOrientation Orientation {
			get => stackView.Orientation.ToOrientation ();
			set => stackView.Orientation = value.ToOrientation ();
		}

		public StackView () : this (new FNSStackView ())
		{

		}

		public StackView (FNSStackView stackView) : base (stackView)
		{
            this.stackView = stackView;
			this.stackView.WantsLayer = true;
            this.stackView.TranslatesAutoresizingMaskIntoConstraints = false;
			this.stackView.Distribution = NSStackViewDistribution.GravityAreas;
			this.stackView.Alignment = NSLayoutAttribute.Top;
		}

		protected override void OnAddChild (IView view)
		{
			stackView.AddArrangedSubview (view.NativeObject as NSView);
		}

		public void AddFlexibleSpace ()
		{
			var spaceView = new View ();
			var space = NativeObject as NSView;
			space.SetContentCompressionResistancePriority ((float)NSLayoutPriority.DefaultLow, NSLayoutConstraintOrientation.Horizontal);
			space.SetContentHuggingPriorityForOrientation ((float)NSLayoutPriority.DefaultLow, NSLayoutConstraintOrientation.Horizontal);
			space.SetContentCompressionResistancePriority ((float)NSLayoutPriority.DefaultLow, NSLayoutConstraintOrientation.Vertical);
			space.SetContentHuggingPriorityForOrientation ((float)NSLayoutPriority.DefaultLow, NSLayoutConstraintOrientation.Vertical);
			AddChild (spaceView);
		}
	}
}
