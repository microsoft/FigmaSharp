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

        readonly List<IViewWrapper> children = new List<IViewWrapper>();
        public IReadOnlyList<IViewWrapper> Children => children;

        public float X
        {
            get => (float)nativeView.X;
            set
            {
                if (value > 0)
                    AbsoluteLayout.SetLayoutBounds(nativeView, new Rectangle(value, nativeView.Y, nativeView.Width, nativeView.Height));

                // TODO:
                nativeView.TranslationX = value;
            }
        }
        public float Y
        {
            get => (float)nativeView.Y;
            set
            {
                if (value > 0)
                    AbsoluteLayout.SetLayoutBounds(nativeView, new Rectangle(nativeView.X, value, nativeView.Width, nativeView.Height));
            }
        }
        public float Width
        {
            get => (float)nativeView.Width;
            set
            {
                if (value > 0)
                    AbsoluteLayout.SetLayoutBounds(nativeView, new Rectangle(nativeView.X, nativeView.Y, value, nativeView.Height));

                nativeView.WidthRequest = value;
            }
        }
        public float Height
        {
            get => (float)nativeView.Height;
            set
            {
                if (value > 0)
                    AbsoluteLayout.SetLayoutBounds(nativeView, new Rectangle(nativeView.X, nativeView.Y, nativeView.Width, value));

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

        public virtual void CreateConstraints(FigmaNode current)
        {

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

        public void SetPosition(float x, float y)
        {
            AbsoluteLayout.SetLayoutBounds(nativeView, new Rectangle(x, y, nativeView.Width, nativeView.Height));

            // TODO:
            nativeView.TranslationX = x;
            nativeView.TranslationY = y;
        }
    }

    public class EmptyView : View
    {

    }
}
