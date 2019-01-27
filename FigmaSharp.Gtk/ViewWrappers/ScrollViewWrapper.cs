/* 
 * ScrollViewWrapper.cs
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
using Gtk;

namespace FigmaSharp
{
    public class ScrollViewWrapper : ViewWrapper, IScrollViewWrapper
    {
        readonly ScrolledWindow scrollView;
        //IViewWrapper ViewPort;

        readonly Fixed currentContainer;

        public ScrollViewWrapper(ScrolledWindow scrollView, Fixed fixedView) : base(scrollView, fixedView)
        {
            this.scrollView = scrollView;
            var viewportView = scrollView.Children.FirstOrDefault();
            if (viewportView is Viewport viewport && viewport.Children[0] is Fixed viewPortFixedView)
            {
                currentContainer = viewPortFixedView;
            } else {
                throw new System.Exception();
            }
        }

        public void AdjustToContent()
        {
            //CGRect contentRect = CGRect.Empty;
            //scrollView.bou
            //if (scrollView.DocumentView is NSView content)
            //{
            //    foreach (var view in content.Subviews)
            //    {
            //        contentRect = contentRect.UnionWith(view.Frame);
            //    }
            //    content.SetFrameSize(contentRect.Size);
            //}
        }

        public override void AddChild(IViewWrapper view)
        {
            children.Add(view);
            view.Parent = this;

            var viewWrapper = (ViewWrapper)view;
            currentContainer?.Put(viewWrapper.FixedContainer, 0, 0);
            //ViewPort.AddChild(view);
            //view.Parent = this;
        }

        public override void RemoveChild(IViewWrapper view)
        {
            var viewWrapper = (ViewWrapper)view;
            children.Remove(view);
            view.Parent = null;
            currentContainer?.Remove(viewWrapper.FixedContainer);
        }

        public override void ClearSubviews()
        {
            var elements = children;

            foreach (var child in elements)
            {
                RemoveChild(child);
            };
        }
    }
}
