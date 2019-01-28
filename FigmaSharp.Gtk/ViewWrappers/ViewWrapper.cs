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
using Gtk;

namespace FigmaSharp
{
    public class ViewWrapper : IViewWrapper
    {
        public object NativeObject => nativeView;
        public IViewWrapper Parent => new ViewWrapper(nativeView.Parent);

        protected readonly List<IViewWrapper> children = new List<IViewWrapper>();
        public virtual IReadOnlyList<IViewWrapper> Children => children;

        public float X {
            get => nativeView.Allocation.X;
            set {
                FixedParentContainer?.Move(nativeView, (int)value, (int)(Y == -1 ? 0 : Y) );
            } 
        }

        //static Fixed GetFixed (Widget widget)
        //{
        //    return widget as Fixed ?? widget.Parent as Fixed;
        //}

        //static Fixed GetFixedParent(Fixed fixedWidger)
        //{
        //    if (fixedWidger.Parent is Fixed)
        //    {
        //        return (Fixed)fixedWidger.Parent;
        //    }
        //    return fixedWidger.Parent?.Parent as Fixed;
        //}

        //static Fixed GetFixedParent (Widget widget)
        //{
        //    var fixedContainter = GetFixed(widget);
        //    if (fixedContainter == null)
        //    {
        //        return null;
        //    }
        //    return GetFixedParent(fixedContainter);
        //}

        public Fixed FixedParentContainer => nativeView.Parent as Fixed;

        //public Fixed FixedContainer => GetFixed(nativeView);
        ///public Fixed FixedContainer { get; }

        public float Y
        {
            get => nativeView.Allocation.Y;
            set
            {
                FixedParentContainer?.Move(nativeView,(int) (X == -1 ? 0 : X), (int)value);
            }
        }

        public float Width
        {
            get => nativeView.WidthRequest;
            set
            {
                nativeView.WidthRequest = (int)value;
            }
        }
        public float Height
        {
            get => nativeView.HeightRequest;
            set
            {
                nativeView.HeightRequest = (int) value;
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

        public ViewWrapper(Widget nativeView)
        {
            //if (nativeView is Viewport viewport && viewport.Children[0] is Fixed viewPortFixedView)
            //{
            //    FixedContainer = viewPortFixedView;
            //    this.nativeView = viewPortFixedView;
            //    return;
            //} 
            //if (nativeView is Fixed fixedView)
            //{
            //    FixedContainer = fixedView;
            //}
            //else
            //{
            //    FixedContainer = new Fixed();
            //    FixedContainer.Put(nativeView, 0, 0);
            //}
            //this.FixedContainer = fixedContainer;
            this.nativeView = nativeView;
        }

        public virtual void AddChild(IViewWrapper view)
        {
            if (nativeView is Fixed nativeFixed)
            {
                children.Add(view);
                var viewWrapper = (ViewWrapper)view;
                nativeFixed.Add(viewWrapper.nativeView);
            }
        }

        public virtual void CreateConstraints(FigmaNode actual)
        {
           throw new System.NotSupportedException();
        }

        public virtual void ClearSubviews()
        {
            var elements = children;

            foreach (var child in elements)
            {
                RemoveChild(child);
            }
        }

        public virtual void RemoveChild(IViewWrapper view)
        {
            if (nativeView is Fixed nativeFixed)
            {
                var viewWrapper = (ViewWrapper)view;
                children.Remove(view);
                nativeFixed.Remove(viewWrapper.nativeView);
            }
        }

        public void MakeFirstResponder()
        {
            if (nativeView.CanFocus)
            {
                nativeView.HasFocus = true;
            }
        }

        public void SetPosition(float x, float y)
        {
            FixedParentContainer?.Move(nativeView, (int)x, (int)y);
        }
    }
}
