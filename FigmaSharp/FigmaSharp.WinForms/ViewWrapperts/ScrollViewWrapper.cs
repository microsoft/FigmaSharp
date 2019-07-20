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
using System.Windows.Forms;
using FigmaSharp.Models;

namespace FigmaSharp.WinForms
{
    public class ScrollViewWrapper : ViewWrapper, IScrollViewWrapper
    {
        readonly ScrollableControl scrollView;
        TransparentControl canvasContainer;
        IViewWrapper canvasContainerWrapper;

        public override IReadOnlyList<IViewWrapper> Children => canvasContainerWrapper.Children;

        public FigmaColor BackgroundColor
        {
            get => scrollView.BackColor.ToFigmaColor();
            set => scrollView.BackColor = value.ToColor();
        }

        public IViewWrapper ContentView {
            get => canvasContainerWrapper;
            set {

                if (value != null && value.NativeObject is TransparentControl canvas)
                {
                    if (canvasContainer != null)
                        scrollView.Controls.Remove(canvasContainer);

                    canvasContainer = canvas;
                    canvasContainerWrapper = value;
                    scrollView.Controls.Add (canvasContainer);
                }
            }
        }
        public ScrollViewWrapper(ScrollableControl scrollView) : base(scrollView)
        {
            this.scrollView = scrollView;
            ContentView = new ViewWrapper(new TransparentControl());
            scrollView.AutoScroll = true;
        }

        public override void AddChild(IViewWrapper view) => canvasContainerWrapper.AddChild(view);

        public override void RemoveChild(IViewWrapper view) => canvasContainerWrapper.RemoveChild(view);

        public void AdjustToContent()
        {
            var children = Children;
            FigmaRectangle contentRect = FigmaRectangle.Zero;
            for (int i = 0; i < children.Count; i++)
            {
                if (i == 0)
                {
                    contentRect = children[i].Allocation;
                }
                else
                {
                    contentRect = contentRect.UnionWith(children[i].Allocation);
                }
            }
            SetContentSize(contentRect.width, contentRect.height);
        }

        public void SetContentSize(float width, float height)
        {
            canvasContainer.Width = (int) width;
            canvasContainer.Height = (int)height;
        }
    }
}
