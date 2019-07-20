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
using FigmaSharp.Models;
using Xamarin.Forms;

namespace FigmaSharp.Forms
{
    public class ViewWrapper : IViewWrapper
    {
        public object NativeObject => nativeView;

        public IViewWrapper Parent
        {
            get
            {
                if (nativeView.Parent is View parentView)
                {
                    return new ViewWrapper(parentView);
                }
                return null;
            }
        }

        protected readonly List<IViewWrapper> children = new List<IViewWrapper>();
        public virtual IReadOnlyList<IViewWrapper> Children => children;

        public float X
        {
            get
            {
                var bounds = AbsoluteLayout.GetLayoutBounds(nativeView);
                return (float)bounds.X;
            }
            set
            {
                if (value < 0)
                {
                    return;
                }
                var bounds = AbsoluteLayout.GetLayoutBounds(nativeView);
                AbsoluteLayout.SetLayoutBounds(nativeView, new Rectangle(value, bounds.Y, bounds.Width, bounds.Height));
            }
        }
        public float Y
        {
            get
            {
                var bounds = AbsoluteLayout.GetLayoutBounds(nativeView);
                return (float)bounds.Y;
            }
            set
            {
                if (value < 0)
                {
                    return;
                }
                var bounds = AbsoluteLayout.GetLayoutBounds(nativeView);
                AbsoluteLayout.SetLayoutBounds(nativeView, new Rectangle(bounds.X, value, bounds.Width, bounds.Height));
            }
        }
        public float Width
        {
            get
            {
                var bounds = AbsoluteLayout.GetLayoutBounds(nativeView);
                return (float) bounds.Width;
            }
            set
            {
                nativeView.WidthRequest = value;
            }
        }
        public float Height
        {
            get
            {
                var bounds = AbsoluteLayout.GetLayoutBounds(nativeView);
                return (float)bounds.Height;
            }
            set
            {
                nativeView.HeightRequest = value;
            }
        }

        public string Identifier {
            get => nativeView.Id.ToString();
            set { }
        }
        public string NodeName {
            get => nativeView.GetType ().ToString ();
            set { }
        }
        public bool Hidden {
            get => !nativeView.IsVisible;
            set => nativeView.IsVisible = !value;
        }

        public FigmaRectangle Allocation
        {
            get
            {
                var bounds = AbsoluteLayout.GetLayoutBounds(nativeView);
                return new FigmaRectangle((float) bounds.X, (float)bounds.Y, (float)bounds.Width, (float)bounds.Height);
            }
        }

        protected View nativeView;

        public virtual void ClearSubviews()
        {
            if (nativeView is Layout<View> layout)
            {
                layout.Children.Clear();
            }
            children.Clear();
        }

        public ViewWrapper(View nativeView)
        {
            this.nativeView = nativeView;
        }

        public virtual void AddChild(IViewWrapper view)
        {
            if (nativeView is Layout<View> layout)
            {
                children.Add(view);
                layout.Children.Add(view.NativeObject as View);
            }
        }

        public virtual void RemoveChild(IViewWrapper view)
        {
            if (children.Contains (view))
            {
                if (nativeView is Layout<View> layout)
                {
                    children.Remove(view);
                    layout.Children.Remove(view.NativeObject as View);
                }
            }
        }

        public void MakeFirstResponder()
        {
            
        }

        public void SetPosition (float x, float y)
        {
            var bounds = AbsoluteLayout.GetLayoutBounds(nativeView);
            AbsoluteLayout.SetLayoutBounds(nativeView, new Rectangle(x, y, bounds.Width, bounds.Height));
        }

        public void SetAllocation(float x, float y, float width, float height)
        {
            AbsoluteLayout.SetLayoutBounds(nativeView, new Rectangle(x, y, width, height));
        }
    }

    public class EmptyView : View
    {

    }
}
