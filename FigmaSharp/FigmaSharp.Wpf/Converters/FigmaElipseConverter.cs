using FigmaSharp.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using FigmaSharp.Models;
using FigmaSharp.Views;
using FigmaSharp.Services;

namespace FigmaSharp.Wpf.Converters
{
    public class FigmaElipseConverter : FigmaElipseConverterBase
    {
        public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var figmaEntity = (FigmaElipse)currentNode;

            var image = new CanvasImage();
            var figmaImageView = new ImageView();
            image.Configure(figmaEntity);

            return figmaImageView;
        }

        public override string ConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            return string.Empty;
        } 
    }
}
