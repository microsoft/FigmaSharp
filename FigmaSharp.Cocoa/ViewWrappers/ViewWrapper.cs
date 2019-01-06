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
using AppKit;

namespace FigmaSharp
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

        public float X {
            get => (float) nativeView.Frame.X;
            set {
                nativeView.SetFrameOrigin(new CoreGraphics.CGPoint(value, nativeView.Frame.Y));
            } 
        }
        public float Y
        {
            get =>  (float)(nativeView.Frame.Y);
            set
            {
                nativeView.SetFrameOrigin(new CoreGraphics.CGPoint( nativeView.Frame.X, value));
            }
        }

        public float Width
        {
            get => (float)nativeView.Frame.Width;
            set
            {
                nativeView.SetFrameSize(new CoreGraphics.CGSize(value, nativeView.Frame.Height));
            }
        }
        public float Height
        {
            get => (float)nativeView.Frame.Height;
            set
            {
                nativeView.SetFrameSize(new CoreGraphics.CGSize(nativeView.Frame.Width, value));
            }
        }

        public string Identifier 
        { 
            get => nativeView.Identifier;
            set => nativeView.Identifier = value;
         }

        public string NodeName
        {
            get => nativeView.Identifier;
            set => nativeView.Identifier = value;
        }

        public bool Hidden
        {
            get => nativeView.Hidden;
            set => nativeView.Hidden = value;
        }

        protected NSView nativeView;

        public ViewWrapper() : this (new NSView ())
        {

        }

        public ViewWrapper(NSView nativeView)
        {
            this.nativeView = nativeView;
        }

        public virtual void AddChild(IViewWrapper view)
        {
            children.Add(view);
            nativeView.AddSubview(view.NativeObject as NSView);
        }

        public virtual void CreateConstraints(FigmaNode actual)
        {
            if (actual == null)
            {
                return;
            }

        }

        public virtual void ClearSubviews()
        {
            foreach (var item in nativeView.Subviews)
            {
                item.RemoveFromSuperview();
            }
        }

        public virtual void RemoveChild(IViewWrapper view)
        {
            if (children.Contains (view))
            {
                children.Remove(view);
                ((NSView)view.NativeObject).RemoveFromSuperview();
            }
        }

        public void MakeFirstResponder()
        {
            nativeView.Window.MakeFirstResponder(nativeView);
        }
    }
}
