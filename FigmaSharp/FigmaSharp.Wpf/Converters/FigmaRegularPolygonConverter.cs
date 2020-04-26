using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigmaSharp.Wpf.Converters
{
    public class FigmaRegularPolygonConverter : FigmaRegularPolygonConverterBase
    { 
        public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var currengroupView = new CanvasImage();
            currengroupView.Configure((FigmaRegularPolygon)currentNode);
            return new ImageView(currengroupView);
        }
         
        public override string ConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            return string.Empty;
        }
    }
}
