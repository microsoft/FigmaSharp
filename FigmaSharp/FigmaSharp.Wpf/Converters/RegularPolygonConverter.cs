using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Wpf;
using System;

namespace FigmaSharp.Wpf.Converters
{
    public class RegularPolygonConverter : RegularPolygonConverterBase
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(CanvasImage);

        public override IView ConvertToView (FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var currengroupView = new CanvasImage();
            currengroupView.Configure((FigmaRegularPolygon)currentNode);
            return new ImageView();// currengroupView);
        }
         
        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        }
    }
}
