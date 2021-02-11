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
using System.Windows.Media;

namespace FigmaSharp.Wpf.Converters
{
    public class SliderConverter : FrameConverterBase
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(ISlider);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);
            return controlType == FigmaControlType.Slider;
        }

        public override IView ConvertToView (FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var slider = new Slider();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            switch (controlType)
            {
                case FigmaControlType.Slider:
                    // apply any styles here
                    break;
            }

            slider.Configure(frame);

            if (currentNode.TrySearchA11Label(out var label))
            {
                if (label != null)
                {
                    AutomationProperties.SetName(slider, label);
                }
            }

            if (currentNode.TrySearchA11Help(out var help))
            {
                if (help != null)
                {
                    AutomationProperties.SetHelpText(slider, help);
                }
            }

            if (currentNode.TrySearchTooltip(out var tooltip))
            {
                if (tooltip != null)
                {
                    slider.ToolTip = tooltip;
                }
            }

            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.visible);

            if (group != null)
            {
                if(group.name == ComponentString.STATE_DISABLED)
                {
                    slider.IsEnabled = false;
                }

                FigmaVector thumb = group.children
                    .OfType<FigmaVector>()
                    .FirstOrDefault(s => s.name == ComponentString.THUMB);

                if (thumb != null)
                {
                    if (slider.Orientation == Orientation.Horizontal)
                    {
                        slider.Value = (thumb.absoluteBoundingBox.X + thumb.absoluteBoundingBox.Width/2) / frame.absoluteBoundingBox.Width;
                    }
                    if (slider.Orientation == Orientation.Vertical)
                    {
                        slider.Value = 1 - ((thumb.absoluteBoundingBox.Y + thumb.absoluteBoundingBox.Height/2) / frame.absoluteBoundingBox.Height);
                    }
                }
            }

            var wrapper = new View(slider);
            return wrapper;
        }
         
        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        } 
    }
}
