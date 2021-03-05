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
    class ListBoxConverter: FrameConverterBase
    {
       
        public override Type GetControlType(FigmaNode currentNode) => typeof(ListBox);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);
            return controlType == FigmaControlType.ListBox;
        }
        public override IView ConvertToView(FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var listBox = new ListBox();


            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            FigmaGroup items = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.visible && s.name == ComponentString.ITEMS);

            listBox.Configure(frame);
            listBox.ConfigureAutomationProperties(frame);
            listBox.ConfigureTooltip(frame);
            listBox.ConfigureTabIndex(frame);
            listBox.configureAlignment(parent);

            if (items != null)
            {
                foreach (FigmaText node in items.children.OfType<FigmaText>())
                {
                    var listBoxItem = new ListBoxItem();
                    listBoxItem.Content = node.characters;
                    listBox.Items.Add(listBoxItem);
                }
            }      

            var wrapper = new View(listBox);
            return wrapper;
        }

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        }
    }
}
