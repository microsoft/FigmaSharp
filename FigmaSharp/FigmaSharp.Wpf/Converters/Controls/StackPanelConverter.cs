using System;
using System.Linq;

using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Extensions;
using FigmaSharp.Views;
using FigmaSharp.Services;
using FigmaSharp.Controls;
using FigmaSharp.Views.Wpf;
using System.Windows.Controls;
using System.Windows.Automation;
using System.Windows;

namespace FigmaSharp.Wpf.Converters
{
    class StackPanelConverter: FrameConverterBase
    {
       
        public override Type GetControlType(FigmaNode currentNode) => typeof(StackPanel);
        public override bool ScanChildren(FigmaNode currentNode) => true;

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);
            return controlType == FigmaControlType.StackPanel;
        }
        public override IView ConvertToView(FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var stackPanel = new StackPanel();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            stackPanel.Configure(frame);

            if (frame.fills.Length > 0)
            {
                if(frame.fills[0].type == "SOLID")
                {
                    stackPanel.Background = frame.fills[0].color.ToColor();
                }
            }


            //FigmaVector rect = frame.children
            //    .OfType<FigmaVector>()
            //    .FirstOrDefault(s => s.name == ComponentString.BACKGROUND);

            //if(rect != null)
            //{
            //    if (rect.strokes.Length > 0)
            //    {
            //        if(rect.strokes[0].type == "SOLID")
            //        {
            //            separator.BorderBrush = rect.strokes[0].color.ToColor();
            //            //TODO: investigate separator thickness. Solution may involve custom control template or use of Rectangle...
            //            separator.BorderThickness = new Thickness(rect.strokeWeight);
            //            //separator.Height = rect.strokeWeight;
            //        } 
            //    }
                    
            //    separator.Opacity = rect.opacity;
                    
            //}
                            

            var wrapper = new View(stackPanel);
            return wrapper;
        }

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        }
    }
}
