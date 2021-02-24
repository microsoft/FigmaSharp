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
    class RowDefinitionConverter: FrameConverterBase
    {
       
        public override Type GetControlType(FigmaNode currentNode) => typeof(RowDefinition);
        public override bool ScanChildren(FigmaNode currentNode) => true;

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);
            return controlType == FigmaControlType.Grid;
        }
        public override IView ConvertToView(FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var grid = new Grid();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            grid.Configure(frame);

            if(frame.fills.Length > 0)
            {
                if(frame.fills[0].type == "SOLID")
                {
                    grid.Background = frame.fills[0].color.ToColor();
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


            //var wrapper = new View(grid);
            //return wrapper;
            return null;
        }

        public RowDefinition GetRowDefinition()
        {
            var rowDefinition = new RowDefinition();


            return rowDefinition;
        }

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        }
    }
}
