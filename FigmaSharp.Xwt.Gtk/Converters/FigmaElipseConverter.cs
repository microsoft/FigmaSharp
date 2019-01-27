using FigmaSharp.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;

namespace FigmaSharp.Converters
{
    public class FigmaElipseConverter : FigmaElipseConverterBase
    {
        public override IViewWrapper ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
            var elipseView = new Box (Xwt.Backends.Orientation.Horizontal);
            return new ViewWrapper (elipseView);
        }

        public override string ConvertToCode(FigmaNode currentNode, ProcessedNode parent)
        {
            return string.Empty;
        }
    }
}
