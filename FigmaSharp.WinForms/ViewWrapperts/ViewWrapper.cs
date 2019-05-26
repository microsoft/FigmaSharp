using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace FigmaSharp.WinForms
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

        public string Identifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string NodeName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Hidden { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

        public void MakeFirstResponder()
        {

        }

        public void SetPosition(float x, float y)
        {
            nativeView.Left = (int)x;
            nativeView.Top = (int)y;
        }
    }
}
