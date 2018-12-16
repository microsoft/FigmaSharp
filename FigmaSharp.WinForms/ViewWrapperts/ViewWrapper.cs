using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace FigmaSharp
{
    public class ViewWrapper : IViewWrapper
    {
        protected Control nativeView;

        public ViewWrapper() : this(new Control())
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

        public void AddChild(IViewWrapper view)
        {
            if (children.Contains(view))
            {
                return;
            }
            children.Add(view);
            nativeView.Controls.Add((Control)view.NativeObject);
        }

        public void CreateConstraints(FigmaNode parent, IViewWrapper parentView)
        {

        }

        public void RemoveChild(IViewWrapper view)
        {
            if (children.Contains(view))
            {
                children.Remove(view);
                var controls = nativeView.Controls.Cast<Control>();
                nativeView.Controls.Remove((Control)view.NativeObject);
            }
        }
    }
}
