/* 
 * FigmaImageView.cs - NSImageView which stores it's associed Figma Id
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Collections.Generic;
using System.Linq;

using AppKit;
using CoreGraphics;
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.Views.Cocoa
{
	public class ScrollView : View, IScrollView
	{
		readonly FNSScrollview scrollView;
		IView contentScrollView;
		NSView contentScrollview;

		public override Color BackgroundColor {
			get => scrollView.BackgroundColor.ToColor ();
			set => scrollView.BackgroundColor = value.ToNSColor ();
		}

		public IView ContentView {
			get => contentScrollView;
			set {
				if (value == null) {
					return;
				}
				contentScrollview = (NSView)value.NativeObject;
				contentScrollView = value;
				contentScrollView.Parent = this;
				this.scrollView.DocumentView = contentScrollview;
			}
		}

		public ScrollView () : this (new FNSScrollview ())
		{
		}

		public ScrollView (FNSScrollview scrollView) : base (scrollView)
		{
			this.scrollView = scrollView;
			var content = scrollView.DocumentView as NSView;
            if (content == null)
            {
				content = new FNSView();
			}

			ContentView = new View(content);

			this.scrollView.HasVerticalScroller = true;
			this.scrollView.HasHorizontalScroller = true;

			this.scrollView.AutomaticallyAdjustsContentInsets = false;
			this.scrollView.AutohidesScrollers = false;
			this.scrollView.ScrollerStyle = NSScrollerStyle.Overlay;
			this.scrollView.AllowsMagnification = true;
			this.scrollView.UsesPredominantAxisScrolling = false;

			this.scrollView.ScrollerInsets = new NSEdgeInsets() { Top = 2, Bottom = 2, Left = 2, Right = 2 };
		}

		public override IReadOnlyList<IView> Children => contentScrollView.Children;

		protected override void OnAddChild(IView view)
		{
			if (contentScrollView.Children.Contains(view))
				return;

			view.Parent = this;
			contentScrollView.AddChild(view);
		}

		public override void ClearSubviews () => contentScrollView.ClearSubviews ();

		protected override void OnRemoveChild(IView view)
		{
			contentScrollView.RemoveChild(view);
		}

		public void AdjustToContent ()
		{
			var items = Children;

			Rectangle contentRect = Rectangle.Zero;
			for (int i = 0; i < items.Count; i++) {
				if (i == 0) {
					contentRect = items[i].Allocation;
				} else {
					contentRect = contentRect.UnionWith (items[i].Allocation);
				}
			}

			//SetContentSize (contentRect.Width, contentRect.Height);
			ContentView.Size = contentRect.Size;
		}

		public void SetContentSize (float width, float height)
		{
            if (ContentView != null)
            {
				ContentView.Size = new Size(width, height);
			}
		}
	}
}
