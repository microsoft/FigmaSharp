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
    class RadioButtonConverter: FrameConverterBase
    {
       
        public override Type GetControlType(FigmaNode currentNode) => typeof(RadioButton);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);
            return controlType == FigmaControlType.RadioButton;
        }
        public override IView ConvertToView(FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var radioButton = new RadioButton();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            switch (controlType)
            {
                case FigmaControlType.RadioButton:
                    // apply any styles here
                    break;
            }

            FigmaText text = frame.children
                    .OfType<FigmaText>()
                    .FirstOrDefault(s => s.name == ComponentString.TITLE);

            radioButton.Configure(frame);
            if (currentNode.TrySearchA11Label(out var label))
            {
                if (label != null)
                {
                    AutomationProperties.SetName(radioButton, label);
                }
            }

            if (currentNode.TrySearchA11Help(out var help))
            {
                if (help != null)
                {
                    AutomationProperties.SetHelpText(radioButton, help);
                }
            }

            if (currentNode.TrySearchTooltip(out var tooltip))
            {
                if (tooltip != null)
                {
                    radioButton.ToolTip = tooltip;
                }
            }

            radioButton.Content = text.characters;
            radioButton.Foreground = text.fills[0].color.ToColor();
            radioButton.Foreground.Opacity = text.opacity;


            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.name.In(ComponentString.STATE_ON,
                                                ComponentString.STATE_OFF) && s.visible);

            if (group != null)
            {
                if (group.name == ComponentString.STATE_ON)
                {
                    radioButton.IsChecked = true;
                }
                if (group.name == ComponentString.STATE_OFF)
                {
                    radioButton.IsChecked = false;
                }
            }




            var buttonWrapper = new View(radioButton);
            return buttonWrapper;
        }

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        }
    }
}
