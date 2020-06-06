// Authors:
//   Jose Medrano <josmed@microsoft.com>
//   Hylke Bons <hylbo@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
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
using System.Text;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Controls.Cocoa
{
    public class SegmentedControlConverter : CocoaConverter
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSSegmentedControl);

        public override bool CanConvert(FigmaNode currentNode)
        {
            return currentNode.TryGetNativeControlType(out var controlType) &&
                controlType == FigmaControlType.SegmentedControl;
        }


        protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
        {
            var segmentedControl = new NSSegmentedControl();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            segmentedControl.ControlSize = CocoaHelpers.GetNSControlSize(controlVariant);
            segmentedControl.Font = CocoaHelpers.GetNSFont(controlVariant);

            FigmaNode items = frame.FirstChild(s => s.name == ComponentString.ITEMS);

            if (items != null)
            {
                segmentedControl.SegmentCount = items.GetChildren(t => t.visible).Count();
                segmentedControl.SegmentDistribution = NSSegmentDistribution.FillEqually;
                segmentedControl.SegmentStyle = NSSegmentStyle.Rounded;
                segmentedControl.SelectedSegment = 0;

                int i = 0;
                foreach (FigmaNode button in items.GetChildren(t => t.visible))
                {
                    FigmaNode state = button.FirstChild(s => s.visible &&
                        s.name.In(ComponentString.STATE_REGULAR, ComponentString.STATE_SELECTED));

                    if (state == null)
                        continue;
                    
                    var text = (FigmaText)state.FirstChild(s => s.name == ComponentString.TITLE);
                    segmentedControl.SetLabel(text.characters, i);

                    i++;
                }
            }

            segmentedControl.TrackingMode = NSSegmentSwitchTracking.SelectOne;

            return new View(segmentedControl);
        }

        protected override StringBuilder OnConvertToCode(CodeNode currentNode, CodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            var code = new StringBuilder();
            string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

            var frame = (FigmaFrame)currentNode.Node;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
                code.WriteConstructor(name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

            code.WriteEquality(name, nameof(NSButton.ControlSize), CocoaHelpers.GetNSControlSize(controlVariant));
            code.WriteEquality(name, nameof(NSSegmentedControl.Font), CocoaCodeHelpers.GetNSFontString(controlVariant));

            FigmaNode items = frame.FirstChild(s => s.name == ComponentString.ITEMS);

            if (items != null)
            {
                code.WriteEquality(name, nameof(NSSegmentedControl.SegmentCount), "" + items.GetChildren(t => t.visible).Count());
                code.WriteEquality(name, nameof(NSSegmentedControl.SegmentDistribution), NSSegmentDistribution.FillEqually);
                code.WriteEquality(name, nameof(NSSegmentedControl.SegmentStyle), NSSegmentStyle.Rounded);
                code.WriteEquality(name, nameof(NSSegmentedControl.SelectedSegment), "0");
                code.WriteEquality(name, nameof(NSSegmentedControl.TrackingMode), NSSegmentSwitchTracking.SelectOne);
                code.AppendLine();

                int i = 0;
                foreach (FigmaNode button in items.GetChildren(t => t.visible))
                {
                    FigmaNode state = button.FirstChild(s => s.visible &&
                        s.name.In(ComponentString.STATE_REGULAR, ComponentString.STATE_SELECTED));

                    if (state == null)
                        continue;

                    var text = (FigmaText)state.FirstChild(s => s.name == ComponentString.TITLE);
                    code.WriteMethod(name, nameof(NSSegmentedControl.SetLabel), $"\"{ text.characters }\", { i }");
                    i++;
                }

                code.AppendLine();
            }

            return code;
        }
    }
}
