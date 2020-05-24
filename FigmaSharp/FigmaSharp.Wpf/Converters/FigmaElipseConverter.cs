using FigmaSharp.Converters;
using System; 
using FigmaSharp.Models;
using FigmaSharp.Views;
using FigmaSharp.Services;
using FigmaSharp.Views.Wpf;

namespace FigmaSharp.Wpf.Converters
{
    public class FigmaElipseConverter : FigmaElipseConverterBase
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(CanvasImage); 

        public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var figmaEntity = (FigmaElipse)currentNode;

            var image = new CanvasImage();
            var elipseView = new View(image);
            image.Configure(figmaEntity);

            return elipseView; 
        }

        public override string ConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            return string.Empty;
        } 
    }
}
