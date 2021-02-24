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
    class GridConverter: FrameConverterBase
    {
       
        public override Type GetControlType(FigmaNode currentNode) => typeof(Grid);
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

            if (currentNode.TrySearchColumnDefinitions(out var columnDefinitions))
            {
                if (columnDefinitions != null)
                {
                    foreach (ColumnDefinition columnDefinition in columnDefinitions)
                    {
                        grid.ColumnDefinitions.Add(columnDefinition);
                    }
                }
            }
            if (currentNode.TrySearchRowDefinitions(out var rowDefinitions))
            {
                if (rowDefinitions != null)
                {
                    foreach (RowDefinition rowDefinition in rowDefinitions)
                    {
                        grid.RowDefinitions.Add(rowDefinition);
                    }
                }
            }

            grid.ShowGridLines = true;




            var wrapper = new View(grid);
            return wrapper;
        }

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        }
    }
}
