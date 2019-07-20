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
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.Views.Cocoa
{
	public class View : IView
    {
        public object NativeObject => nativeView;

        IView parent;
        public IView Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            } 
        }

        readonly List<IView> children = new List<IView>();
        public virtual IReadOnlyList<IView> Children => children;

        public float Width
        {
            get => (float)nativeView.Frame.Width;
            set
            {
                SetSize(value, (float) nativeView.Frame.Height);
            }
        }
        public float Height
        {
            get => (float)nativeView.Frame.Height;
            set
            {
                SetSize((float) nativeView.Frame.Width, value);
            }
        }

		public Size Size {
			get => new Size (Width, Height);
			set => SetSize(value);
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

		public float CornerRadius
		{
			get => (float) nativeView.Layer.CornerRadius;
			set => nativeView.Layer.CornerRadius = value;
		}

		public bool Hidden
        {
            get => nativeView.Hidden;
            set => nativeView.Hidden = value;
        }

        protected NSView nativeView;

        public View() : this (new FNSView())
        {

        }

		public bool MovableByWindowBackground
		{
			get => nativeView.MouseDownCanMoveWindow;
			set
			{
				if (nativeView is IFlippedView fNSView)
					fNSView.MouseDownMovesWindow = value;
			}
		}


		public View(NSView nativeView)
		{
			this.nativeView = nativeView;
			this.nativeView.WantsLayer = true;
		}

		Transform transform;
		public void ApplyTransform (Transform transform)
		{
			this.transform = transform;
			ApplyTransform();
		}

		public Point AnchorPoint { get; set; } = Point.Zero;

		public void ApplyTransform()
		{
			if (transform == null) {
				return;
			}

			var tranform = CoreGraphics.CGAffineTransform.MakeTranslation(transform.Translation.X, transform.Translation.Y);
			tranform.TransformPoint(new CoreGraphics.CGPoint(nativeView.Frame.Width / 2f, nativeView.Frame.Height / 2f));
			tranform.Rotate(transform.RotateAngle);
			tranform.Scale(transform.Scale, transform.Scale);
			
			nativeView.Layer.AffineTransform = tranform;

			OnApplyTransform();
		}

		public virtual void OnApplyTransform()
		{
			//to implemente
		}


        public void AddChild(IView view)
        {
            view.Parent = this;
            children.Add(view);
			OnAddChild(view);
		}

		protected virtual void OnAddChild (IView view)
		{
			nativeView.AddSubview(view.NativeObject as NSView);
		}

		protected virtual void OnRemoveChild(IView view)
		{
			((NSView)view.NativeObject).RemoveFromSuperview();
		}

		public virtual void ClearSubviews()
        {
            foreach (var item in nativeView.Subviews)
            {
                item.RemoveFromSuperview();
            }
        }

        public virtual void OnChangeFrameSize (Size newSize)
        {
			foreach (var item in children)
			{
				item.OnChangeFrameSize(newSize);
			}
        }

		public void Rotate ()
		{

		}

        public virtual void RemoveChild(IView view)
        {
            if (children.Contains (view))
            {
                children.Remove(view);
				OnRemoveChild(view);
			}
        }

        public void Focus()
        {
            nativeView.Window?.MakeFirstResponder(nativeView);
        }

		#region Allocation

		public Size IntrinsicContentSize
		{
			get => new Size((float)nativeView.IntrinsicContentSize.Width, (float) nativeView.IntrinsicContentSize.Height);
		}

		public Rectangle Allocation
        {
            get => new Rectangle((float)nativeView.Frame.X, (float)nativeView.Frame.Y, (float)nativeView.Frame.Width, (float)nativeView.Frame.Height);
            set => SetAllocation(value.X, value.Y, value.Width, value.Height);
        }

        public void SetPosition(float x, float y) => nativeView.SetFrameOrigin(new CoreGraphics.CGPoint(x, y));
        public void SetPosition(Point point) => SetPosition(point.X, point.Y);

        public void SetSize(float width, float height)
        {
			bool hasChanged = width != Width || height != Height;
			nativeView.SetFrameSize(new CoreGraphics.CGSize(width, height));
			if (hasChanged)
				OnChangeFrameSize(new Size(width, height));
		}

        public void SetSize(Size size) => SetSize(size.Width, size.Height);

        public void SetAllocation(float x, float y, float width, float height)
        {
			bool hasChanged = width != Width || height != Height;
			nativeView.Frame = new CoreGraphics.CGRect(x, y, width, height);
			if (hasChanged)
				OnChangeFrameSize(new Size(width, height));
		}
          
        public void SetAllocation(Point point, Size size) => SetAllocation(point.X, point.Y, size.Width, size.Height);

        #endregion

        public virtual Color BackgroundColor {
            get => nativeView.Layer.BackgroundColor.ToColor();
            set => nativeView.Layer.BackgroundColor = value.ToCGColor();
        }

        AnchorStyles anchor;
        public AnchorStyles Anchor {
            get => anchor;
            set
            {
                anchor = value;
            }
        }

		public bool IsDark {
			get => nativeView.Appearance == NSAppearance.GetAppearance(NSAppearance.NameDarkAqua);
			set => nativeView.Appearance = NSAppearance.GetAppearance(NSAppearance.NameDarkAqua);
		}

		public virtual void Dispose()
        {
            
        }
    }
}
