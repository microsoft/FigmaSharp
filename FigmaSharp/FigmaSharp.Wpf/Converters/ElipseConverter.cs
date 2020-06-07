using FigmaSharp.Converters;
using System; 
using FigmaSharp.Models;
using FigmaSharp.Views;
using FigmaSharp.Services;
using FigmaSharp.Views.Wpf;

namespace FigmaSharp.Wpf.Converters
{
    public class ElipseConverter : ElipseConverterBase
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(CanvasImage); 

        public override IView ConvertToView(FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var figmaEntity = (FigmaElipse)currentNode;

            var image = new CanvasImage();
            var elipseView = new View(image);
            image.Configure(figmaEntity);

            return elipseView; 
        }

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        } 
    }
}
