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
            var elipseView = new UserControl();
            return new ViewWrapper (elipseView);
        }

        public override string ConvertToCode(FigmaNode currentNode)
        {
            return string.Empty;
        }
    }
}
