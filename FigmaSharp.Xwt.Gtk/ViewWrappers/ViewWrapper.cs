/* 
 * ViewWrapper.cs
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
using Xwt;

namespace FigmaSharp
{
    public class ViewWrapper : IViewWrapper
    {
        public object NativeObject => nativeView;

        public IViewWrapper Parent
        {
            get
            {
                if (nativeView.Parent != null)
                {
                    return new ViewWrapper(nativeView.Parent);
                }
                return null;
            }
        }

        readonly List<IViewWrapper> children = new List<IViewWrapper>();
        public IReadOnlyList<IViewWrapper> Children => children;

        public float X {
            get => 0; //(float) nativeView.Frame.X;
            set {
               // nativeView.SetFrameOrigin(new CoreGraphics.CGPoint(value, nativeView.Frame.Y));
            } 
        }
        public float Y
        {
            get => 0;
            set
            {
               // nativeView// = value;
            }
        }

        public float Width
        {
            get => (float)nativeView.WidthRequest;
            set
            {
                nativeView.WidthRequest = value;
            }
        }
        public float Height
        {
            get => (float)nativeView.HeightRequest;
            set
            {
                nativeView.HeightRequest = value;
            }
        }

        public string Identifier 
        { 
            get => nativeView.Name;
            set => nativeView.Name = value;
         }

        public string NodeName
        {
            get => nativeView.Name;
            set => nativeView.Name = value;
        }

        public bool Hidden
        {
            get => !nativeView.Visible;
            set => nativeView.Visible = !value;
        }

        protected Widget nativeView;

        public ViewWrapper() : this (new Xwt.Label ())
        {

        }

        public ViewWrapper(Widget nativeView)
        {
            this.nativeView = nativeView;
        }

        public virtual void AddChild(IViewWrapper view)
        {
            throw new System.NotSupportedException();
        }

        public virtual void CreateConstraints(FigmaNode actual)
        {
            throw new System.NotSupportedException();
        }

        public virtual void ClearSubviews()
        {
            foreach (var item in nativeView.)
            {
                item.RemoveFromSuperview();
            }
        }

        public virtual void RemoveChild(IViewWrapper view)
        {
            if (view is Box box)
            {
                var c = box.Children;
                foreach (var child in c)
                {
                    box.Remove(child);
                }
            }
        }

        public void MakeFirstResponder()
        {
            if (nativeView.CanGetFocus)
            {
                nativeView.SetFocus();
            }
        }
    }
}
