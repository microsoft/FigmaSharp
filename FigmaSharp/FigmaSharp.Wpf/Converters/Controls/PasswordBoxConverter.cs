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
    class PasswordBoxConverter: FrameConverterBase
    {
       
        public override Type GetControlType(FigmaNode currentNode) => typeof(PasswordBox);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);
            return controlType == FigmaControlType.PasswordBox;
        }
        public override IView ConvertToView(FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var passwordBox = new PasswordBox();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            FigmaNode optionsGroup = frame.Options();

            FigmaText placeholderText = optionsGroup?.GetChildren()
                .OfType<FigmaText>()
                .FirstOrDefault(s => s.name == ComponentString.PLACEHOLDER && s.visible);

            passwordBox.Password = placeholderText.characters;

            passwordBox.Configure(frame);
            passwordBox.ConfigureAutomationProperties(frame);
            passwordBox.ConfigureTooltip(frame);
            passwordBox.ConfigureTabIndex(frame);
            passwordBox.configureAlignment(parent);

            if (currentNode.TrySearchControlGroupName(out var name))
            {
                if (name != null)
                {
                    passwordBox.Name = name;
                }
            }

            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.visible);

            if (group != null)
            {
                FigmaText text = frame.children
                    .OfType<FigmaText>()
                    .FirstOrDefault(s => s.name == ComponentString.TITLE);
                passwordBox.Foreground = text.fills[0].color.ToColor();
                passwordBox.Foreground.Opacity = text.opacity;

                // Not elegant, but cannot figure out how to add System.Windows.VerticalAlignment to ViewHelper
                passwordBox.VerticalContentAlignment = (System.Windows.VerticalAlignment)ViewHelper.GetTextVerticalAlignment(text);
                
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
                            passwordBox.Background = rect.fills[0].color.ToColor();
                        } 
                    }

                    passwordBox.Background.Opacity = rect.opacity;
                    if (rect.strokes.Length > 0)
                    {
                        passwordBox.BorderBrush = rect.strokes[0].color.ToColor();
                        passwordBox.BorderThickness = new System.Windows.Thickness(rect.strokeWeight);
                    }
                    else
                    {
                        passwordBox.BorderThickness = new System.Windows.Thickness(0);
                    }
                    
                }
                if (group.name == ComponentString.STATE_DISABLED)
                {
                    passwordBox.IsEnabled = false;
                }    
            }

            var wrapper = new View(passwordBox);
            return wrapper;
        }

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        }
    }
}
