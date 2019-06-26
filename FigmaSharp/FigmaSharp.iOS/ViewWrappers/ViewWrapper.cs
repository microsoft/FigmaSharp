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
using UIKit;

namespace FigmaSharp.iOS
{
    public class ViewWrapper : IViewWrapper
    {
        public object NativeObject => nativeView;

        public IViewWrapper Parent
        {
            get
            {
                if (nativeView.Superview != null)
                {
                    return new ViewWrapper(nativeView.Superview);
                }
                return null;
            }
        }

        readonly List<IViewWrapper> children = new List<IViewWrapper>();
        public IReadOnlyList<IViewWrapper> Children => children;

        public float Width
        {
            get => (float)nativeView.Frame.Width;
            set
            {
                nativeView.Bounds = new CoreGraphics.CGRect(0,0, value, nativeView.Frame.Height); 
                nativeView.Frame = new CoreGraphics.CGRect(nativeView.Frame.X, nativeView.Frame.Y, value, nativeView.Frame.Height);
            }
        }
        public float Height
        {
            get => (float)nativeView.Frame.Height;
            set
            {
                nativeView.Bounds = new CoreGraphics.CGRect(0, 0, nativeView.Frame.Width, value);
                nativeView.Frame = new CoreGraphics.CGRect(nativeView.Frame.X, nativeView.Frame.Y, nativeView.Frame.Width, value);
            }
        }

        public string Identifier { get => string.Empty; set { } }
        public string NodeName { get => string.Empty; set { } }

        public bool Hidden
        {
            get => nativeView.Hidden;
            set => nativeView.Hidden = value;
        }

        public FigmaRectangle Allocation => new FigmaRectangle((float)nativeView.Frame.X, (float)nativeView.Frame.Y, (float)nativeView.Frame.Width, (float)nativeView.Frame.Height);

        protected UIView nativeView;

        public virtual void ClearSubviews()
        {
            //clean views from current container
            //var views = nativeView.Subviews;
            //foreach (var item in views)
            //{
            //    item.RemoveFromSuperview();
            //}
            //nativeView.RemoveConstraints(nativeView.Constraints);

            //Figma doesn't calculate the bounds of our first level
            //frameEntityResponse.FigmaMainNode.CalculateBounds();

        }

        public ViewWrapper(UIView nativeView)
        {
            this.nativeView = nativeView;
        }

        public virtual void AddChild(IViewWrapper view)
        {
            children.Add(view);
            nativeView.AddSubview(view.NativeObject as UIView);
        }

        public virtual void CreateConstraints(FigmaNode current)
        {

        }

        public virtual void RemoveChild(IViewWrapper view)
        {
            if (children.Contains (view))
            {
                children.Remove(view);
                ((UIView)view.NativeObject).RemoveFromSuperview();
            }
        }

        public void MakeFirstResponder()
        {
            if (nativeView.CanBecomeFirstResponder)
                nativeView.BecomeFirstResponder();
        }

        public void SetPosition(float x, float y)
        {
            nativeView.Frame = new CoreGraphics.CGRect(x, y, nativeView.Frame.Width, nativeView.Frame.Height);
        }

        public void SetAllocation(float x, float y, float width, float height)
        {
            nativeView.Frame = new CoreGraphics.CGRect(x, y, width, height);
        }
    }
}
