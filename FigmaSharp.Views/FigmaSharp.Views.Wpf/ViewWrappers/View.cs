using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FigmaSharp.Models;
using FigmaSharp.Views;

namespace FigmaSharp.Views.Wpf
{
    public class View : IView
    {
        protected FrameworkElement nativeView;

        public View() :this(new FrameworkElement())
        { 
        }

        public View(FrameworkElement nativeView)
        {
            this.nativeView = nativeView;
        }

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

        protected readonly List<IView> children = new List<IView>();
        public virtual IReadOnlyList<IView> Children => children;

        public object NativeObject => nativeView;

        public float X
        {
            get => (float)Canvas.GetLeft(nativeView);
            set => Canvas.SetLeft(nativeView, value);
        }

        public float Y
        {
            get => (float)Canvas.GetTop(nativeView);
            set => Canvas.SetTop(nativeView, value);
        } 

        public float Width
        {
            get => (float)nativeView.Width;
            set
            {
                nativeView.Width =  value;
            }
        }
        public float Height
        {
            get => (float)nativeView.Height;
            set
            {
                nativeView.Height = value;
            }
        }
         
        public string Identifier { get => nativeView.Name; set { } }
        public string NodeName { get => nativeView.Name; set { } }
        public bool Hidden { get => true; set { }  }

        public Rectangle Allocation {
            get {
                return new Rectangle((float)Canvas.GetLeft(nativeView), (float)Canvas.GetTop(nativeView), (float)nativeView.Width, (float) nativeView.Height);
            }
        }

        public virtual void AddChild(IView view)
        {
            if (children.Contains(view))
            {
                return;
            }
          
            if (nativeView is Panel panel)
            {
                children.Add(view);
                panel.Children.Add((FrameworkElement)view.NativeObject);
            }
        }

        public virtual void CreateConstraints(FigmaNode current)
        {
          
        }

        public virtual void RemoveChild(IView view)
        {
            if (!children.Contains(view))
            {
                return;
            }

            if (nativeView is Panel panel)
            {
                children.Remove(view);
                panel.Children.Remove((FrameworkElement)view.NativeObject);
            }
        }

        public virtual void ClearSubviews()
        {
            if (nativeView is Panel panel)
            {
                var controls = panel.Children;
                foreach (var item in controls)
                {
                    controls.Remove((FrameworkElement)item);
                }
            }
            children.Clear();
        }

        public void MakeFirstResponder()
        {

        }

        public void SetPosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void SetPosition(Point point) => SetPosition(point.X, point.Y);

        public void SetAllocation(float x, float y, float width, float height)
        {
            SetPosition(x, y);
            Width = width;
            Height = height;
        }

        public void SetAllocation(Point point, Size size) => SetAllocation(point.X, point.Y, size.Width, size.Height);

        public Size Size
        {
            get => new Size(Width, Height);
            set => SetSize(value);
        }

        public void SetSize(float width, float height)
        {
            bool hasChanged = width != Width || height != Height; 
            Width = width;
            Height = height;
            if (hasChanged)
                OnChangeFrameSize(new Size(width, height));
        }

        public void SetSize(Size size) => SetSize(size.Width, size.Height);

        public virtual void OnChangeFrameSize(Size newSize)
        {
            foreach (var item in children)
            {
                item.OnChangeFrameSize(newSize);
            }
        }

        AnchorStyles anchor;
        public AnchorStyles Anchor
        {
            get => anchor;
            set
            {
                anchor = value;
            }
        }

        public void Focus()
        {
            nativeView.Focus();
        }

        public virtual void Dispose()
        {
        }

        public float CornerRadius { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool MovableByWindowBackground { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Size IntrinsicContentSize => throw new NotImplementedException();

        public Color BackgroundColor {
            get { 
                return Color.White;
            } 
            set
            { 
            }
        }

        public void SetAlignmentRect(float x, float y, float width, float height)
        {
            throw new NotImplementedException();
        }
    }
}
