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
using FigmaSharp;

namespace FigmaSharp.Forms
{
    public class ScrollViewWrapper : ViewWrapper, IScrollViewWrapper
    {
        public IViewWrapper ContentView
        {
            get => new ViewWrapper(scrollView.Content);
            set
            {
                if (value.NativeObject is View content)
                {
                    this.scrollView.Content = content;
                }
            }
        }

        readonly ScrollView scrollView;
        readonly AbsoluteLayout scrollContent;

        public ScrollViewWrapper(ScrollView scrollView) : base(scrollView)
        {
            this.scrollView = scrollView;
            this.scrollContent = new AbsoluteLayout();
            scrollView.Content = scrollContent;
            scrollView.Orientation = ScrollOrientation.Both;
        }

        public FigmaColor BackgroundColor {
            get => scrollView.Content.BackgroundColor.ToFigmaColor ();
            set => scrollView.Content.BackgroundColor = value.ToColor();
        }

        public override void AddChild(IViewWrapper view)
        {
            children.Add(view);
            scrollContent.Children.Add(view.NativeObject as View);
        }

        public void AdjustToContent()
        {
            if (scrollView.Content == null)
                return;

            FigmaRectangle contentRect = FigmaRectangle.Zero;
            foreach (var view in Children)
            {
                contentRect = contentRect.UnionWith(view.Allocation);
            }
            SetContentSize(contentRect.width, contentRect.height);
        }

        public override void RemoveChild(IViewWrapper view)
        {
            if (scrollView.Content == view.NativeObject)
            {
                scrollView.Content = null;
            }
        }

        public void SetContentSize(float width, float height)
        {
            scrollView.Content.WidthRequest = width;
            scrollView.Content.HeightRequest = height;
        }
    }
}
