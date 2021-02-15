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
    class SeparatorConverter: FrameConverterBase
    {
       
        public override Type GetControlType(FigmaNode currentNode) => typeof(Separator);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);
            return controlType == FigmaControlType.Separator;
        }
        public override IView ConvertToView(FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var separator = new Separator();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.visible);

            separator.Configure(frame);


            FigmaVector rect = frame.children
                .OfType<FigmaVector>()
                .FirstOrDefault(s => s.name == ComponentString.BACKGROUND);

            if(rect != null)
            {
                if (rect.strokes.Length > 0)
                {
                    if(rect.strokes[0].type == "SOLID")
                    {
                        separator.BorderBrush = rect.strokes[0].color.ToColor();
                        //TODO: investigate separator thickness. Solution may involve custom control template or use of Rectangle...
                        separator.BorderThickness = new Thickness(rect.strokeWeight);
                        //separator.Height = rect.strokeWeight;
                    } 
                }
                    
                separator.Opacity = rect.opacity;
                    
            }
                            

            var buttonWrapper = new View(separator);
            return buttonWrapper;
        }

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        }
    }
}
