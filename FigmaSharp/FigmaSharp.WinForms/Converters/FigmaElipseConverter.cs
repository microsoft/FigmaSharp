using FigmaSharp.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FigmaSharp.Models;

namespace FigmaSharp.WinForms.Converters
{
    public class FigmaElipseConverter : FigmaElipseConverterBase
    {
        public override IViewWrapper ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
            var elipseView = new ImageTransparentControl();
            var elipse = (FigmaElipse)currentNode;
            elipseView.Configure(elipse);
            return new ViewWrapper(elipseView);
        }

        public override string ConvertToCode(FigmaNode currentNode)
        {
            return string.Empty;
        }
    }
}
