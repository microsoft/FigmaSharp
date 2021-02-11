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
    public class ProgressBarConverter : FrameConverterBase
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(IProgressBar);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);
            return controlType == FigmaControlType.ProgressBar;
        }

        public override IView ConvertToView (FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            var progressBar = new ProgressBar();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            switch (controlType)
            {
                case FigmaControlType.ProgressBar:
                    // apply any styles here
                    break;
            }

            if (currentNode.TrySearchA11Label(out var label))
            {
                if (label != null)
                {
                    AutomationProperties.SetName(progressBar, label);
                }
            }

            if (currentNode.TrySearchA11Help(out var help))
            {
                if (help != null)
                {
                    AutomationProperties.SetHelpText(progressBar, help);
                }
            }

            if (currentNode.TrySearchTooltip(out var tooltip))
            {
                if (tooltip != null)
                {
                    progressBar.ToolTip = tooltip;
                }
            }

            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => (s.name == ComponentString.STYLE_DETERMINATE || s.name == ComponentString.STYLE_INDETERMINATE) && s.visible);

            if (group != null)
            {
                FigmaVector rect = group.children
                    .OfType<FigmaVector>()
                    .FirstOrDefault(s => s.name == ComponentString.BACKGROUND);

                FigmaVector value = group.children
                    .OfType<FigmaVector>()
                    .FirstOrDefault(s => s.name == ComponentString.VALUE);

                progressBar.Configure(rect);

                if (group.name == ComponentString.STYLE_DETERMINATE)
                {
                    progressBar.IsIndeterminate = false;
                    progressBar.Minimum = 0;
                    progressBar.Maximum = 1;
                    progressBar.Value = value.absoluteBoundingBox.Width / rect.absoluteBoundingBox.Width;
                    progressBar.Foreground = value.fills[0].color.ToColor();
                }

                if(group.name == ComponentString.STYLE_INDETERMINATE)
                {
                    progressBar.IsIndeterminate = true;
                    progressBar.Foreground = rect.fills[0].color.ToColor();
                }
            }

            var wrapper = new View(progressBar);
            return wrapper;
        }
         
        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            return string.Empty;
        } 
    }
}
