using FigmaSharp.Converters;
using FigmaSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigmaSharp.WinForms.Converters
{
    public class FigmaRegularPolygonConverter : FigmaRegularPolygonConverterBase
    {
        public override IViewWrapper ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
            var currengroupView = new ImageTransparentControl();
            currengroupView.Configure((FigmaRegularPolygon)currentNode);
            return new ImageViewWrapper(currengroupView);
        }

        public override string ConvertToCode(FigmaNode currentNode)
        {
            return string.Empty;
        }
    }
}
