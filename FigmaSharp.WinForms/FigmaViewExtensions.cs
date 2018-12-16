using System;
using System.Windows.Forms;

namespace FigmaSharp.WinForms
{
    public static class FigmaViewExtensions
    {
        public static void Configure(this Control view, FigmaNode child)
        {
            view.Visible = child.visible;

            if (child is IFigmaDocumentContainer container)
            {
               //view.SetFrameSize(new CGSize(container.absoluteBoundingBox.width, container.absoluteBoundingBox.height));
            }
        }
    }
}
