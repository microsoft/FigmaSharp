using System;
using UIKit;

namespace FigmaSharp
{
    public static class ViewConfigureExtensions
    {
        #region Configures

        public static void Configure(this UIView view, FigmaNode child)
        {
            view.Hidden = !child.visible;

            //if (child is IFigmaDocumentContainer container)
            //{
            //    view.Frame.Width = container.absoluteBoundingBox.width;
            //    view.Frame.Size = new CGSize(container.absoluteBoundingBox.width, container.absoluteBoundingBox.height));
            //}
        }

        #endregion
    }
}
