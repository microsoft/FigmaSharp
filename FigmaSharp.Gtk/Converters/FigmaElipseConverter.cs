using FigmaSharp.Converters;
using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FigmaSharp.Models;

namespace FigmaSharp.GtkSharp.Converters
{
    public class FigmaElipseConverter : FigmaElipseConverterBase
    {
        public override IViewWrapper ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
            var model = (FigmaElipse)currentNode;
            var view = new Fixed ();
            view.Configure(model);

            return new ViewWrapper (view);
        }

        public override string ConvertToCode(FigmaNode currentNode)
        {
            return string.Empty;
        }
    }
}
