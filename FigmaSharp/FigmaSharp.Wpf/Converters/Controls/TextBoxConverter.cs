using System;
using System.Linq;

using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Extensions;
using FigmaSharp.Views;
using FigmaSharp.Services;
using FigmaSharp.Controls;
using FigmaSharp.Views.Wpf;
using FigmaSharp.Helpers;
using System.Windows.Controls;
using System.Windows.Automation;

namespace FigmaSharp.Wpf.Converters
{
    class TextBoxConverter: FrameConverterBase
    {
       
        public override Type GetControlType(FigmaNode currentNode) => typeof(ITextBox);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);
            return controlType == FigmaControlType.TextBox;
        }
        public override IView ConvertToView(FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var textBox = new TextBox();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            switch (controlType)
            {
                case FigmaControlType.TextBox:
                    // apply any styles here
                    break;
            }

            FigmaNode optionsGroup = frame.Options();

            FigmaText placeholderText = optionsGroup?.GetChildren()
                .OfType<FigmaText>()
                .FirstOrDefault(s => s.name == ComponentString.PLACEHOLDER && s.visible);

            if (placeholderText != null && !placeholderText.characters.Equals(ComponentString.PLACEHOLDER, StringComparison.InvariantCultureIgnoreCase))
            {
                // There is no placeholderText property for TextBox controls in WPF...
            }

            textBox.Configure(frame);
            textBox.ConfigureAutomationProperties(frame);
            textBox.ConfigureTooltip(frame);
            textBox.ConfigureTabIndex(frame);
            textBox.configureAlignment(parent);

            if (currentNode.TrySearchControlGroupName(out var name))
            {
                if (name != null)
                {
                    textBox.Name = name;
                }
            }

            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.visible);

            textBox.Configure(frame);

            if (group != null)
            {
                FigmaText text = frame.children
                    .OfType<FigmaText>()
                    .FirstOrDefault(s => s.name == ComponentString.TITLE);
                textBox.Text = text.characters;
                textBox.Foreground = text.fills[0].color.ToColor();
                textBox.Foreground.Opacity = text.opacity;

                // Hacky, but cannot figure out how to add System.Windows.VerticalAlignment to ViewHelper
                textBox.VerticalContentAlignment = (System.Windows.VerticalAlignment)ViewHelper.GetTextVerticalAlignment(text);
                
                //TODO: Inner padding

                FigmaVector rect = frame.children
                    .OfType<FigmaVector>()
                    .FirstOrDefault(s => s.name == ComponentString.BACKGROUND);

                if(rect != null)
                {
                    if (rect.fills.Length > 0)
                    {
                        if(rect.fills[0].type == "SOLID")
                        {
                            Console.WriteLine(rect.fills);
                            textBox.Background = rect.fills[0].color.ToColor();
                        } 
                    }

                    textBox.Background.Opacity = rect.opacity;
                    if (rect.strokes.Length > 0)
                    {
                        textBox.BorderBrush = rect.strokes[0].color.ToColor();
                        textBox.BorderThickness = new System.Windows.Thickness(rect.strokeWeight);
                    }
                    else
                    {
                        textBox.BorderThickness = new System.Windows.Thickness(0);
                    }
                    
                }
                if (group.name == ComponentString.STATE_DISABLED)
                {
                    Console.WriteLine("Disabling button");
                    textBox.IsEnabled = false;
                }
                
            }


            

            var textBoxWrapper = new View(textBox);
            return textBoxWrapper;
        }

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        }
    }
}
