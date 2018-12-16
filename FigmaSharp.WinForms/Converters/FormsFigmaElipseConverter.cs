using System;
using System.Windows.Forms;
using FigmaSharp.Converters;

namespace FigmaSharp.WinForms.Converters
{
    public class FormsFigmaElipseConverter : FigmaElipseConverterBase
    {
        public override IViewWrapper ConvertTo(FigmaNode currentNode, FigmaNode parentNode, IViewWrapper parentView)
        {
            var elipseView = new Control();
            return new ViewWrapper(elipseView);
        }
    }
}
