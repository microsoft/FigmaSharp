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
    class ButtonConverter: FrameConverterBase
    {
       
        public override Type GetControlType(FigmaNode currentNode) => typeof(IButton);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);
            return controlType == FigmaControlType.Button;
        }
        public override IView ConvertToView(FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var button = new Button();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            switch (controlType)
            {
                case FigmaControlType.Button:
                    // apply any styles here
                    break;
            }

            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.visible);

            button.Configure(frame);
            button.ConfigureAutomationProperties(frame);
            button.ConfigureTooltip(frame);

            if (group != null)
            {
                FigmaText text = group.children
                    .OfType<FigmaText>()
                    .FirstOrDefault(s => s.name == ComponentString.TITLE);
                button.Foreground = text.fills[0].color.ToColor();
                button.Foreground.Opacity = text.opacity;
                button.FontSize = text.style.fontSize;
                button.FontWeight = FontWeight.FromOpenTypeWeight(text.style.fontWeight);

                if (currentNode.TrySearchAcceleratorKey(out var key))
                {
                    if (key != null)
                    {
                        button.ConfigureAcceleratorKey(text.characters, key);
                    }
                }
                else
                {
                    button.Content = text.characters;
                }

                FigmaVector rect = group.children
                    .OfType<FigmaVector>()
                    .FirstOrDefault(s => s.name == ComponentString.BACKGROUND);

                if(rect != null)
                {
                    if (rect.fills.Length > 0)
                    {
                        if(rect.fills[0].type == "SOLID")
                        {
                            button.Background = rect.fills[0].color.ToColor();
                        } 
                    }
                    
                    button.Background.Opacity = rect.opacity;
                    if (rect.strokes.Length > 0)
                    {
                        button.BorderBrush = rect.strokes[0].color.ToColor();
                        button.BorderThickness = new System.Windows.Thickness(rect.strokeWeight);
                    }
                    else
                    {
                        button.BorderThickness = new System.Windows.Thickness(0);
                    }
                    
                }
                if (group.name == ComponentString.STATE_DISABLED)
                {
                    Console.WriteLine("Disabling button");
                    button.IsEnabled = false;
                }
                
            }


            

            var buttonWrapper = new View(button);
            return buttonWrapper;
        }

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        }
    }
}
