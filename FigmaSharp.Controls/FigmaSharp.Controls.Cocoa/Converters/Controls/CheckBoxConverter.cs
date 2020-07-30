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
using FigmaSharp.Controls.Cocoa.Helpers;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Controls.Cocoa.Converters
{
    public class CheckBoxConverter : CocoaConverter
    {
        internal override bool HasHeightConstraint() => false;

        public override Type GetControlType(FigmaNode currentNode) => typeof(NSButton);

        public override bool CanConvert(FigmaNode currentNode)
        {
            return currentNode.TryGetNativeControlType(out var controlType) &&
                controlType == FigmaControlType.CheckBox;
        }

        protected override IView OnConvertToView (FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
        {
            var frame = (FigmaFrame)currentNode;

            var checkBox = new NSButton();
            checkBox.SetButtonType(NSButtonType.Switch);

            FigmaText text = frame.children
                  .OfType<FigmaText>()
                  .FirstOrDefault(s => s.name == ComponentString.TITLE);

            if (text != null)
                checkBox.Title = rendererService.GetTranslatedText(text);

            frame.TryGetNativeControlVariant(out var controlVariant);

            checkBox.ControlSize = ViewHelper.GetNSControlSize(controlVariant);
            checkBox.Font = ViewHelper.GetNSFont(controlVariant, text);

            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.name.In(ComponentString.STATE_ON,
                                               ComponentString.STATE_OFF,
                                               ComponentString.STATE_MIXED) && s.visible);

            if (group != null)
            {
                if (group.name == ComponentString.STATE_ON)
                    checkBox.State = NSCellStateValue.On;

                if (group.name == ComponentString.STATE_OFF)
                    checkBox.State = NSCellStateValue.Off;

                if (group.name == ComponentString.STATE_MIXED)
                {
                    checkBox.AllowsMixedState = true;
                    checkBox.State = NSCellStateValue.Mixed;
                }
            }

            return new View(checkBox);
        }


        protected override StringBuilder OnConvertToCode (CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            var code = new StringBuilder ();
            string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

            var frame = (FigmaFrame)currentNode.Node;
            currentNode.Node.TryGetNativeControlType(out FigmaControlType controlType);
            currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);
            
            if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
                code.WriteConstructor (name, GetControlType(currentNode.Node), rendererService.NodeRendersVar (currentNode, parentNode));

            code.WriteMethod (name, nameof (NSButton.SetButtonType), NSButtonType.Switch);

            code.WritePropertyEquality(name, nameof(NSButton.ControlSize), ViewHelper.GetNSControlSize(controlVariant));
            code.WritePropertyEquality(name, nameof(NSSegmentedControl.Font), CodeHelper.GetNSFontString(controlVariant));

            FigmaText text = frame.children
                .OfType<FigmaText> ()
                .FirstOrDefault ();

            if (text != null) {
                code.WriteTranslatedEquality(name, nameof(NSButton.Title), text, rendererService);
            }
            
            FigmaGroup group = frame.children
                .OfType<FigmaGroup> ()
                .FirstOrDefault(s => s.name.In(ComponentString.STATE_ON,
                                               ComponentString.STATE_OFF,
                                               ComponentString.STATE_MIXED) && s.visible);

            if (group != null) {
                if (group.name == ComponentString.STATE_ON)
                    code.WritePropertyEquality (name, nameof (NSButton.State), NSCellStateValue.On);

                if (group.name == ComponentString.STATE_OFF)
                    code.WritePropertyEquality(name, nameof(NSButton.State), NSCellStateValue.Off);

                if (group.name == ComponentString.STATE_MIXED)
                {
                    code.WritePropertyEquality(name, nameof(NSButton.AllowsMixedState), true);
                    code.WritePropertyEquality(name, nameof(NSButton.State), NSCellStateValue.Mixed);
                }
            }

            return code;
        }
    }
}
