/* 
 * CustomTextFieldConverter.cs
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Linq;
using System.Text;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
    public class SwitchConverter : CocoaConverter
    {
        public override Type GetControlType(FigmaNode currentNode)
        {
            return typeof(NSSwitch);
        }

        public override bool CanConvert(FigmaNode currentNode)
        {
            return currentNode.TryGetNativeControlType(out var value) &&
                value == NativeControlType.Switch;
        }

        protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var frame = (FigmaFrameEntity)currentNode;
            var switchControl = new NSSwitch();

            switchControl.Configure (frame);

            frame.TryGetNativeControlVariant (out var controlVariant);
            switchControl.ControlSize = NSControlSize.Regular;

            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => (s.name == "On" || s.name == "Off") && s.visible);

            if (group != null)
            {
                if (group.name == "On")
                    switchControl.State = 1;

                if (group.name == "Off")
                    switchControl.State = 0;
            }

            return new View(switchControl);
        }


        protected override StringBuilder OnConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            var frame = (FigmaFrameEntity)currentNode.Node;

            var code = new StringBuilder ();
            var name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;
            
            if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
                code.WriteConstructor (name, GetControlType(currentNode.Node), rendererService.NodeRendersVar (currentNode, parentNode));

            code.Configure (currentNode.Node, name);
            code.WriteEquality(name, nameof(NSButton.ControlSize), NSControlSize.Regular);

            FigmaGroup group = frame.children
                .OfType<FigmaGroup> ()
                .FirstOrDefault (s => (s.name == "On" || s.name == "Off") && s.visible);

            if (group != null) {
                if (group.name == "On")
                    code.WriteEquality (name, nameof (NSSwitch.State), "1", inQuotes: false);

                if (group.name == "Off")
                    code.WriteEquality(name, nameof(NSSwitch.State), "0", inQuotes: false);
            }

            return code;
        }
    }
}
