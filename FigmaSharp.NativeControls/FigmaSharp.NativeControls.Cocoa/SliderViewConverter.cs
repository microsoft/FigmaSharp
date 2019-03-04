using System;
using System.Text;
using AppKit;

namespace FigmaSharp.NativeControls
{
    public class SliderViewConverter : CustomViewConverter
    {
        public override bool CanConvert(FigmaNode currentNode)
        {
            return ContainsType (currentNode, "slider");
        }

        public override IViewWrapper ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
			var sliderView = new NSSlider ();

			var keyValues = GetKeyValues (currentNode);
			foreach (var key in keyValues) {
				if (key.Key == "type") {
					continue;
				} 
				if (key.Key == "enabled") {
					sliderView.Enabled = key.Value == "true";
				} else if (key.Key == "size") {
					sliderView.ControlSize = ToEnum<NSControlSize> (key.Value);
				} else if (key.Key == "max") {
					sliderView.MaxValue = Convert.ToDouble (key.Value);
				} else if (key.Key == "min") {
					sliderView.MinValue = Convert.ToDouble (key.Value);
				} else if (key.Key == "value") {
					sliderView.DoubleValue = Convert.ToDouble (key.Value);
				}
			}

			sliderView.SliderType = NSSliderType.Linear;
            //((NSSliderCell)sliderView.Cell).TickMarkPosition = NSTickMarkPosition.Right;
            sliderView.Configure(currentNode);
            return new ViewWrapper(sliderView);
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
