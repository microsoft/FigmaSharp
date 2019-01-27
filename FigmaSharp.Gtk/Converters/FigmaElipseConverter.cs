using FigmaSharp.Converters;
using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigmaSharp.Converters
{
    public class FigmaElipseConverter : FigmaElipseConverterBase
    {
        public override IViewWrapper ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
            var model = (FigmaElipse)currentNode;
            var view = new Label ();
            view.Configure(model);

            var fixedView = new Fixed();
            fixedView.Put(view, 0, 0);
            return new ViewWrapper (view, fixedView);
        }

        public override string ConvertToCode(FigmaNode currentNode, ProcessedNode parent)
        {
            return string.Empty;
        }
    }
}
