﻿/* 
 * View.cs
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
using FigmaSharp.Maui;
using Microsoft.Maui;

namespace FigmaSharp.Views.Maui
{
	public class View : IView
	{
		public object NativeObject => nativeView;

        IView IView.Parent
        {
            get
            {
                if (nativeView.Parent is Microsoft.Maui.Controls.View parent)
                {
                    return new View(parent);
                }
                return null;
            }
            set { }
        }

		protected readonly List<IView> children = new List<IView> ();
		public virtual IReadOnlyList<IView> Children => children;

		public float X {
			get {
				var bounds = Microsoft.Maui.Controls.AbsoluteLayout.GetLayoutBounds (nativeView);
				return (float)bounds.X;
			}
			set {
				if (value < 0) {
					return;
				}
				var bounds = Microsoft.Maui.Controls.AbsoluteLayout.GetLayoutBounds (nativeView);
                Microsoft.Maui.Controls.AbsoluteLayout.SetLayoutBounds (nativeView, new Microsoft.Maui.Graphics.Rect (value, bounds.Y, bounds.Width, bounds.Height));
			}
		}

		public float Y {
			get {
				var bounds = Microsoft.Maui.Controls.AbsoluteLayout.GetLayoutBounds (nativeView);
				return (float)bounds.Y;
			}
			set {
				if (value < 0) {
					return;
				}
				var bounds = Microsoft.Maui.Controls.AbsoluteLayout.GetLayoutBounds (nativeView);
                Microsoft.Maui.Controls.AbsoluteLayout.SetLayoutBounds (nativeView, new Microsoft.Maui.Graphics.Rect (bounds.X, value, bounds.Width, bounds.Height));
			}
		}

		public float Width {
			get {
				var bounds = Microsoft.Maui.Controls.AbsoluteLayout.GetLayoutBounds (nativeView);
				return (float)bounds.Width;
			}
			set {
				nativeView.WidthRequest = value;
			}
		}

		public float Height {
			get {
				var bounds = Microsoft.Maui.Controls.AbsoluteLayout.GetLayoutBounds (nativeView);
				return (float)bounds.Height;
			}
			set {
				nativeView.HeightRequest = value;
			}
		}

		public string Identifier {
			get => nativeView.Id.ToString ();
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

		public Rectangle Allocation {
			get {
				var bounds = Microsoft.Maui.Controls.AbsoluteLayout.GetLayoutBounds (nativeView);
				return new Rectangle ((float)bounds.X, (float)bounds.Y, (float)bounds.Width, (float)bounds.Height);
			}
		}

		public bool IsDark {
			get => false;
			set { }
		}
		public float CornerRadius {
			get => 0;
			set { }
		}

		public bool MovableByWindowBackground {
			get => false;
			set { }
		}

		public Size Size {
			get => new Size ((float)nativeView.Width, (float)nativeView.Height);
			set {
				nativeView.WidthRequest = value.Width;
				nativeView.HeightRequest = value.Height;
			}
		}

		public Size IntrinsicContentSize => Allocation.Size;

		public virtual Color BackgroundColor {
			get => nativeView.BackgroundColor.ToFigmaColor ();
			set {
				nativeView.BackgroundColor = value.ToMauiColor ();
			}
		}

		public AnchorStyles Anchor {
			get => AnchorStyles.None;
			set { }
		}

        protected Microsoft.Maui.Controls.View nativeView;

		public virtual void ClearSubviews ()
		{
			if (nativeView is Microsoft.Maui.Controls.AbsoluteLayout layout)
				layout.Children.Clear ();
			children.Clear ();
		}

		public View () : this (new Microsoft.Maui.Controls.AbsoluteLayout())
		{

		}

		public View(Microsoft.Maui.Controls.View nativeView)
		{
			this.nativeView = nativeView;
		}

		public void AddChild (IView view)
		{
			if (!children.Contains (view)) {
				children.Add (view);
				OnAddChild (view);
			}
		}

		public virtual void OnAddChild (IView view)
		{
			if (nativeView is Microsoft.Maui.Controls.AbsoluteLayout layout) {
				layout.Children.Add (view.NativeObject as Microsoft.Maui.Controls.View);
			}
		}

		public virtual void RemoveChild (IView view)
		{
			if (children.Contains (view)) {
				children.Remove (view);
				OnRemoveChild (view);
			}
		}

		public virtual void OnRemoveChild (IView view)
		{
			if (nativeView is Microsoft.Maui.Controls.AbsoluteLayout layout) {
				layout.Children.Remove (view.NativeObject as Microsoft.Maui.Controls.View);
			}
		}

		public void SetPosition (float x, float y)
		{
			var bounds = Microsoft.Maui.Controls.AbsoluteLayout.GetLayoutBounds (nativeView);
            Microsoft.Maui.Controls.AbsoluteLayout.SetLayoutBounds (nativeView, new Microsoft.Maui.Graphics.Rect (x, y, bounds.Width, bounds.Height));
		}

		public void SetAllocation (float x, float y, float width, float height)
		{
            Microsoft.Maui.Controls.AbsoluteLayout.SetLayoutBounds (nativeView, new Microsoft.Maui.Graphics.Rect (x, y, (float)width, (float)height));
		}

		public void Focus ()
		{
			nativeView.Focus ();
		}

		public void SetPosition (Point origin) => SetPosition (origin.X, origin.Y);

		public void SetAllocation (Point origin, Size size)
		{
			SetPosition (origin);
		}

		public void OnChangeFrameSize (Size newSize)
		{
			foreach (var item in children) {
				item.OnChangeFrameSize (newSize);
			}
		}

		public virtual void Dispose ()
		{

		}

        public void SetAlignmentRect(float x, float y, float width, float height)
        {
            throw new System.NotImplementedException();
        }
    }

	public class EmptyView : Microsoft.Maui.Controls.View
	{

	}
}
