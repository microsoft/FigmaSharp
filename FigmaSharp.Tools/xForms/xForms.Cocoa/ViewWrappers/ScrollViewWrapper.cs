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

using FigmaSharp.Models;

namespace FigmaSharp.Cocoa
{
    public class ScrollViewWrapper : ViewWrapper, IScrollViewWrapper
    {
        readonly NSScrollView scrollView;
        IViewWrapper contentScrollviewWrapper;
        NSView contentScrollview;

        public FigmaColor BackgroundColor
        {
            get => scrollView.BackgroundColor.ToFigmaColor();
            set => scrollView.BackgroundColor = value.ToNSColor();
        }

        public IViewWrapper ContentView {
            get => contentScrollviewWrapper;
            set
            {
                if (value == null)
                {
                    return;
                }
                contentScrollview = (NSView)value.NativeObject;
                contentScrollviewWrapper = value;
                this.scrollView.DocumentView = contentScrollview;
            }
        }

        public ScrollViewWrapper(NSScrollView scrollView) : base(scrollView)
        {
            this.scrollView = scrollView;

            contentScrollview = scrollView.DocumentView as NSView;
            if (contentScrollview == null)
            {
                contentScrollview = new NSView();
                this.scrollView.DocumentView = contentScrollview;
            }
            contentScrollviewWrapper = new ViewWrapper (contentScrollview);

            this.scrollView.HasVerticalScroller = true;
            this.scrollView.HasHorizontalScroller = true;
            this.scrollView.AutomaticallyAdjustsContentInsets = false;
            this.scrollView.AutohidesScrollers = false;
            this.scrollView.ScrollerStyle = NSScrollerStyle.Legacy;
        }

        public override IReadOnlyList<IViewWrapper> Children => contentScrollviewWrapper.Children;

        public override void AddChild(IViewWrapper view) => contentScrollviewWrapper.AddChild(view);

        public override void ClearSubviews() => contentScrollviewWrapper.ClearSubviews();

        public override void RemoveChild (IViewWrapper view)=> contentScrollviewWrapper.ClearSubviews();

        public void AdjustToContent()
        {
            var items = Children;

            FigmaRectangle contentRect = FigmaRectangle.Zero;
            for (int i = 0; i < items.Count; i++)
            {
                if (i == 0)
                {
                    contentRect = items[i].Allocation;
                } else
                {
                    contentRect = contentRect.UnionWith(items[i].Allocation);
                }
            }
            SetContentSize(contentRect.width, contentRect.height);
        }

        public void SetContentSize(float width, float height)
        {
            if (scrollView.DocumentView is NSView content)
            {
                content.SetFrameSize(new CGSize(width, height));
            }
        }
    }
}
