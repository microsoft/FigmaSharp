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

using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FigmaSharp.Wpf
{
    public class ScrollViewWrapper : ViewWrapper, IScrollViewWrapper
    {
        readonly ScrollViewer scrollView;

        Canvas canvasContainer;

        public FigmaColor BackgroundColor
        {
            get => scrollView.Background.ToFigmaColor();
            set => scrollView.Background = value.ToColor();
        }

        public ScrollViewWrapper(ScrollViewer scrollView) : base(scrollView)
        {
            this.scrollView = scrollView;

            canvasContainer = new Canvas();
            scrollView.Content = canvasContainer;
            scrollView.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            scrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
        }

        public override void AddChild(IViewWrapper view)
        {
            if (children.Contains(view))
            {
                return;
            }

            children.Add(view);
            canvasContainer.Children.Add((FrameworkElement)view.NativeObject);
        }

        public override void RemoveChild(IViewWrapper view)
        {
            if (!children.Contains(view))
            {
                return;
            }

            children.Remove(view);
            canvasContainer.Children.Remove((FrameworkElement)view.NativeObject);
        }

        public void AdjustToContent()
        {
            FigmaRectangle contentRect = FigmaRectangle.Zero;
            foreach (var view in Children)
            {
                contentRect = contentRect.UnionWith(view.Allocation);
            }
            SetContentSize(contentRect.width, contentRect.height);
        }

        public void SetContentSize(float width, float height)
        {
            canvasContainer.Width = width;
            canvasContainer.Height = height;
        }
    }
}
