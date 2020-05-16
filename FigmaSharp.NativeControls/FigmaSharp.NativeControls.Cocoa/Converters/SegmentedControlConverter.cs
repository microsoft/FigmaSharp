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
using System.Collections.Generic;
using System.Text;

using AppKit;

using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
    public class SegmentedControlConverter : CocoaConverter
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSSegmentedControl);

        public override bool CanConvert(FigmaNode currentNode)
        {
            return currentNode.TryGetNativeControlType(out var controlType) &&
                controlType == NativeControlType.SegmentedControl;
        }


        protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
        {
            var frame = (FigmaFrame)currentNode;
            FigmaNode buttons = frame.FirstChild(s => s.name == "Cells");

            if (buttons == null)
                return null;

            var labels = new List<string>();

            foreach (FigmaNode button in buttons.GetChildren(t => t.visible))
            {
                FigmaNode state = button.FirstChild(s => s.name.In("Basic", "Default") && s.visible);

                if (state != null)
                {
                    var text = (FigmaText)state.FirstChild(s => s.name == "lbl");
                    labels.Add(text.characters);
                }
            }

            var segmentedControl = NSSegmentedControl.FromLabels(
                labels.ToArray(),
                NSSegmentSwitchTracking.SelectOne,
                () => Console.WriteLine("SegmentedControl activated"));

            currentNode.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

            segmentedControl.ControlSize = GetNSControlSize(controlVariant);
            segmentedControl.SegmentStyle = NSSegmentStyle.Rounded;
            segmentedControl.SelectedSegment = 0;


            segmentedControl.Font = GetNSFont(controlVariant);

            return new View(segmentedControl);
        }

        protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            var code = new StringBuilder();

            // TODO output:

            // var tabView = new NSTabView();
            // tabView.SetItems(new NSTabViewItem[] {
            //     new NSTabViewItem() { Label = "label1" },
            //     new NSTabViewItem() { Label = "label2" },
            //     new NSTabViewItem() { Label = "label3" }
            // });

            return code;
        }
    }
}
