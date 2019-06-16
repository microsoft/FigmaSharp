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

namespace FigmaSharp.GtkSharp
{
    public class ScrollViewWrapper : ViewWrapper, IScrollViewWrapper
    {
        readonly ScrolledWindow scrollView;

        readonly Fixed currentContainer;

        public FigmaColor BackgroundColor
        {
            get => viewPort?.Style.Background(StateType.Normal).ToFigmaColor ();
            set => viewPort?.ModifyBg (StateType.Normal, value.ToGdkColor ());
        }

        private Viewport viewPort => scrollView.Children.OfType<Viewport>().FirstOrDefault();

        public ScrollViewWrapper(ScrolledWindow scrollView, Fixed fixedView) : base(scrollView)
        {
            this.scrollView = scrollView;
            var viewportView = viewPort;
            if (viewportView != null && viewportView.Children[0] is Fixed viewPortFixedView)
            {
                currentContainer = viewPortFixedView;
            } else {
                throw new System.Exception();
            }
        }

        public void AdjustToContent()
        {
            //not necessary in gtk
        }

        public override void AddChild(IViewWrapper view)
        {
            children.Add(view);
            var viewWrapper = (ViewWrapper)view;
            currentContainer?.Put(viewWrapper.NativeObject as Widget, 0, 0);
        }

        public override void RemoveChild(IViewWrapper view)
        {
            var viewWrapper = (ViewWrapper)view;
            children.Remove(view);
            currentContainer?.Remove(viewWrapper.NativeObject as Widget);
        }

        public override void ClearSubviews()
        {
            var elements = children;

            foreach (var child in elements)
            {
                RemoveChild(child);
            };
        }

        public void SetContentSize(float width, float height)
        {
            //not necessary in gtk
        }
    }
}
