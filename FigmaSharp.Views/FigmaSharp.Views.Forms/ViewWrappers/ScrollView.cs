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

using System;
using System.Linq;
using Xamarin.Forms;
using System.Collections.Generic;

namespace FigmaSharp.Views.Forms
{
	public class ScrollView : View, IScrollView
	{
		public IView ContentView {
			get => scrollViewWrapper;
			set {
				if (value.NativeObject is AbsoluteLayout content) {
					this.scrollView.Content = content;
					scrollViewWrapper = value;
					scrollContent = content;
				}
			}
		}

		Xamarin.Forms.ScrollView scrollView;
		AbsoluteLayout scrollContent;
		IView scrollViewWrapper;

		public ScrollView (Xamarin.Forms.ScrollView scrollView) : base (scrollView)
		{
			this.scrollView = scrollView;

			if (scrollView.Content is AbsoluteLayout abs) {
				this.scrollContent = abs;
			} else {
				this.scrollContent = new AbsoluteLayout ();
				scrollView.Content = scrollContent;
			}
			scrollViewWrapper = new View (scrollContent);
			scrollView.Orientation = ScrollOrientation.Both;
		}

		public override Color BackgroundColor {
			get => scrollView.Content.BackgroundColor.ToLiteColor ();
			set => scrollView.Content.BackgroundColor = value.ToFormsColor ();
		}

		public override IReadOnlyList<IView> Children => scrollViewWrapper.Children;

		public override void OnAddChild (IView view)
		{
			scrollViewWrapper.AddChild (view);
		}

		public void AdjustToContent ()
		{
			if (scrollView.Content == null)
				return;

			var childs = Children;
			var contentRect = Rectangle.Zero;
			for (int i = 0; i < childs.Count; i++) {
				if (i == 0) {
					contentRect = childs.ElementAt (i).Allocation;
				} else {
					contentRect = contentRect.UnionWith (childs.ElementAt (i).Allocation);
				}
			}
			SetContentSize (contentRect.Width, contentRect.Height);
		}

		public override void OnRemoveChild (IView view)
		{
			scrollViewWrapper.RemoveChild (view);
		}

		public void SetContentSize (float width, float height)
		{
			scrollContent.WidthRequest = width;
			scrollContent.HeightRequest = height;
		}
	}
}
