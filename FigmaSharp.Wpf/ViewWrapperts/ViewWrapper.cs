using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FigmaSharp.Wpf
{
    public class ViewWrapper : IViewWrapper
    {
        protected FrameworkElement nativeView;

        public ViewWrapper() : this(new UserControl())
        {

        }

        public ViewWrapper(FrameworkElement nativeView)
        {
            this.nativeView = nativeView;
        }

        public IViewWrapper Parent => new ViewWrapper(nativeView.Parent as FrameworkElement);

        protected readonly List<IViewWrapper> children = new List<IViewWrapper>();
        public IReadOnlyList<IViewWrapper> Children => children;

        public object NativeObject => nativeView;

        public float Width
        {
            get => (float)nativeView.ActualWidth;
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

        public string Identifier { get => string.Empty; set { } }
        public string NodeName { get => string.Empty; set { } }
        public bool Hidden { get => true; set { }  }

        public FigmaRectangle Allocation {
            get {
                return new FigmaRectangle((float)Canvas.GetLeft(nativeView), (float)Canvas.GetTop(nativeView), (float)nativeView.Width, (float) nativeView.Height);
            }
        }

        public virtual void AddChild(IViewWrapper view)
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

        public virtual void RemoveChild(IViewWrapper view)
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
            Canvas.SetLeft(nativeView, x);
            Canvas.SetTop(nativeView, y);
        }

        public void SetAllocation(float x, float y, float width, float height)
        {
            SetPosition(x, y);
            Width = width;
            Height = height;
        }
    }
}
