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

namespace FigmaSharp.Controls.Cocoa.Converters
{
    public class SwitchConverter : CocoaConverter
    {
        internal override bool HasHeightConstraint() => false;
        public override bool CanSetAccessibilityLabel => false;


        public override Type GetControlType(FigmaNode currentNode) => typeof(NSSwitch);

        public override bool CanConvert(FigmaNode currentNode)
        {
            return currentNode.TryGetNativeControlType(out var controlType) &&
                controlType == FigmaControlType.Switch;
        }


        protected override IView OnConvertToView (FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
        {
            var frame = (FigmaFrame)currentNode;
            var switchControl = new NSSwitch();

            frame.TryGetNativeControlVariant (out var controlVariant);
            switchControl.ControlSize = NSControlSize.Regular;

            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => (s.name == ComponentString.STATE_ON || s.name == ComponentString.STATE_OFF) && s.visible);

            if (group != null)
            {
                if (group.name == ComponentString.STATE_ON)
                    switchControl.State = 1;

                if (group.name == ComponentString.STATE_OFF)
                    switchControl.State = 0;
            }

            return new View(switchControl);
        }


        protected override StringBuilder OnConvertToCode (CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            var code = new StringBuilder();
            string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

            var frame = (FigmaFrame)currentNode.Node;
            currentNode.Node.TryGetNativeControlType(out FigmaControlType controlType);
            currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

            if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
                code.WriteConstructor(name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

            code.WritePropertyEquality(name, nameof(NSButton.ControlSize), NSControlSize.Regular);

            FigmaGroup group = frame.children
                .OfType<FigmaGroup> ()
                .FirstOrDefault (s => s.name.In(ComponentString.STATE_ON, ComponentString.STATE_OFF) && s.visible);

            if (group != null)
            {
                if (group.name == ComponentString.STATE_ON)
                    code.WritePropertyEquality (name, nameof (NSSwitch.State), "1", inQuotes: false);

                if (group.name == ComponentString.STATE_OFF)
                    code.WritePropertyEquality(name, nameof(NSSwitch.State), "0", inQuotes: false);
            }

            return code;
        }
    }
}
