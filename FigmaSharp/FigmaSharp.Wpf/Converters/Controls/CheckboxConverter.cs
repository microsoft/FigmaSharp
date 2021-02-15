// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using System.Windows.Controls;
using FigmaSharp.Controls;
using FigmaSharp.Converters;
using FigmaSharp.Extensions;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Wpf;
using System.Windows.Automation;

namespace FigmaSharp.Wpf.Converters
{
    public class CheckboxConverter : FrameConverterBase
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(ICheckBox);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);
            return controlType == FigmaControlType.Checkbox;
        }

        public override IView ConvertToView (FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var checkbox = new CheckBox();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            switch (controlType)
            {
                case FigmaControlType.TextBlock:
                    // apply any styles here
                    break;
            }

            FigmaText text = frame.children
                    .OfType<FigmaText>()
                    .FirstOrDefault(s => s.name == ComponentString.TITLE);

            checkbox.Configure(frame);
            checkbox.ConfigureAutomationProperties(frame);
            checkbox.ConfigureTooltip(frame);

            if (currentNode.TrySearchAcceleratorKey(out var key))
            {
                if (key != null)
                {
                    checkbox.ConfigureAcceleratorKey(text.characters, key);
                }
            }
            else
            {
                checkbox.Content = text.characters;
            }
            checkbox.Foreground = text.fills[0].color.ToColor();
            checkbox.Foreground.Opacity = text.opacity;

            //TODO: investigate how to apply style to check box

            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.name.In(ComponentString.STATE_ON,
                                                ComponentString.STATE_OFF) && s.visible);

            if (group != null)
            {
                if(group.name == ComponentString.STATE_ON)
                {
                    checkbox.IsChecked = true;
                }
                if (group.name == ComponentString.STATE_OFF)
                {
                    checkbox.IsChecked = false;
                }
            }

            var wrapper = new View(checkbox);
            return wrapper;
        }
         
        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        } 
    }
}
