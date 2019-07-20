using FigmaSharp.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using FigmaSharp.Models;
namespace FigmaSharp.Wpf.Converters
{
    public class FigmaElipseConverter : FigmaElipseConverterBase
    {
        public override IViewWrapper ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
            var figmaEntity = (FigmaElipse)currentNode;

            var image = new CanvasImage();
            var figmaImageView = new ImageViewWrapper();
            image.Configure(figmaEntity);

            return figmaImageView;
        }

        public override string ConvertToCode(FigmaNode currentNode)
        {
            return string.Empty;
        }
    }
}
