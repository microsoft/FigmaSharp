using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace FigmaSharp
{
    public class ViewWrapper : IViewWrapper
    {
        protected Control nativeView;

        public ViewWrapper() : this(new TransparentControl())
        {

        }

        public ViewWrapper(Control nativeView)
        {
            this.nativeView = nativeView;
        }

        public IViewWrapper Parent => new ViewWrapper(nativeView.Parent);

        readonly List<IViewWrapper> children = new List<IViewWrapper>();
        public IReadOnlyList<IViewWrapper> Children => children;

        public object NativeObject => nativeView;


        public float X
        {
            get => (float)nativeView.Location.X;
            set
            {
                //layer.Bounds = new CoreGraphics.CGRect(0, 0, 150, 150);
                //Position = new CGPoint(50, 50);
                nativeView.Left = (int) value;
            }
        }
        public float Y
        {
            get => (float)nativeView.Location.Y;
            set
            {
                nativeView.Top = (int)value;
            }
        }
        public float Width
        {
            get => (float)nativeView.Width;
            set
            {
                nativeView.Width = (int) value;
            }
        }
        public float Height
        {
            get => (float)nativeView.Height;
            set
            {
                nativeView.Height = (int)value;
            }
        }
        public virtual void AddChild(IViewWrapper view)
        {
            if (children.Contains(view))
            {
                return;
            }
            children.Add(view);
            nativeView.Controls.Add((Control)view.NativeObject);
        }

        public virtual void CreateConstraints(FigmaNode current)
        {
          
        }

        public virtual void RemoveChild(IViewWrapper view)
        {
            if (children.Contains(view))
            {
                children.Remove(view);
                var controls = nativeView.Controls.Cast<Control>();
                nativeView.Controls.Remove((Control)view.NativeObject);
            }
        }

        public virtual void ClearSubviews()
        {
            var controls = nativeView.Controls;
            foreach (var item in controls)
            {
                if (item is Control control)
                nativeView.Controls.Remove(control);
            }
        }
    }
}
