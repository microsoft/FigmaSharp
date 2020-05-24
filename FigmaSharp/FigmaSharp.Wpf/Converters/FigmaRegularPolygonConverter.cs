using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Wpf;
using System;

namespace FigmaSharp.Wpf.Converters
{
    public class FigmaRegularPolygonConverter : FigmaRegularPolygonConverterBase
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(CanvasImage);

        public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var currengroupView = new CanvasImage();
            currengroupView.Configure((FigmaRegularPolygon)currentNode);
            return new ImageView();// currengroupView);
        }
         
        public override string ConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            return string.Empty;
        }
    }
}
