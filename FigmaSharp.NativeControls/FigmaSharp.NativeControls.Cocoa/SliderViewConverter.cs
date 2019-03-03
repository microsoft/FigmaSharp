using System;
using System.Text;
using AppKit;

namespace FigmaSharp.NativeControls
{
    public class SliderViewConverter : CustomViewConverter
    {
        public override bool CanConvert(FigmaNode currentNode)
        {
            return currentNode.name == "slider" && currentNode is IFigmaDocumentContainer;
        }

        public override IViewWrapper ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
            var textField = new NSSlider();
            textField.SliderType = NSSliderType.Linear;
            ((NSSliderCell)textField.Cell).TickMarkPosition = NSTickMarkPosition.Right;
            textField.Configure(currentNode);
            return new ViewWrapper(textField);
        }

        public override string ConvertToCode(FigmaNode currentNode, ProcessedNode parent)
        {
            var builder = new StringBuilder();
            var name = "sliderView";
            builder.AppendLine($"var {name} = new {nameof(NSSlider)}();");
            builder.Configure(name, currentNode);
            return builder.ToString();
        }
    }
}
