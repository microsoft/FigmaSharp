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
    public class ButtonConverter : CocoaConverter
    {
        internal override bool HasHeightConstraint() => false;
        internal override bool IsFlexibleHorizontal(FigmaNode node) => true;
        public override bool CanSetAccessibilityLabel => false;


        public override Type GetControlType(FigmaNode currentNode) => typeof(NSButton);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);

            return controlType == FigmaControlType.Button ||
                   controlType == FigmaControlType.ButtonHelp ||
                   controlType == FigmaControlType.ButtonRoundRect;
        }


        protected override IView OnConvertToView (FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
        {
            var button = new NSButton();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            switch (controlType)
            {
                case FigmaControlType.Button:
                    button.BezelStyle = NSBezelStyle.Rounded;
                    break;
                case FigmaControlType.ButtonRoundRect:
                    button.BezelStyle = NSBezelStyle.RoundRect;
                    break;
                case FigmaControlType.ButtonHelp:
                    button.BezelStyle = NSBezelStyle.HelpButton;
                    button.Title = string.Empty;
                    break;
            }

            button.ControlSize = ViewHelper.GetNSControlSize(controlVariant);
            button.Font = ViewHelper.GetNSFont(controlVariant);

            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.visible);

            if (group != null)
            {
                FigmaText text = group.children
                    .OfType<FigmaText>()
                    .FirstOrDefault(s => s.name == ComponentString.TITLE);

                if (text != null && controlType != FigmaControlType.ButtonHelp)
                    button.Title = rendererService.GetTranslatedText (text);

                if (group.name == ComponentString.STATE_DISABLED)
                    button.Enabled = false;

                if (group.name == ComponentString.STATE_DEFAULT)
                    button.KeyEquivalent = "\r";
            }

            return new View(button);
        }


        protected override StringBuilder OnConvertToCode (CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            var code = new StringBuilder();
            string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

            var frame = (FigmaFrame)currentNode.Node;
            currentNode.Node.TryGetNativeControlType(out FigmaControlType controlType);
            currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

            if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
                code.WriteConstructor (name, GetControlType(currentNode.Node).FullName, rendererService.NodeRendersVar (currentNode, parentNode));

            switch (controlType)
            {
                case FigmaControlType.Button:
                    code.WritePropertyEquality(name, nameof(NSButton.BezelStyle), NSBezelStyle.Rounded);
                    break;
                case FigmaControlType.ButtonRoundRect:
                    code.WritePropertyEquality(name, nameof(NSButton.BezelStyle), NSBezelStyle.RoundRect);
                    break;
                case FigmaControlType.ButtonHelp:
                    code.WritePropertyEquality(name, nameof(NSButton.BezelStyle), NSBezelStyle.HelpButton);
                    code.WritePropertyEquality(name, nameof(NSButton.Title), string.Empty, inQuotes: true);
                    break;
            }

            code.WritePropertyEquality(name, nameof(NSButton.ControlSize), ViewHelper.GetNSControlSize(controlVariant));
            code.WritePropertyEquality(name, nameof(NSSegmentedControl.Font), CodeHelper.GetNSFontString(controlVariant));

            FigmaGroup group = frame.children
                .OfType<FigmaGroup> ()
                .FirstOrDefault(s => s.visible);

            if (group != null) {
                FigmaText text = group.children
                    .OfType<FigmaText> ()
                    .FirstOrDefault (s => s.name == ComponentString.TITLE);

                if (text != null && controlType != FigmaControlType.ButtonHelp)
                    code.WriteTranslatedEquality(name, nameof(NSButton.Title), text, rendererService);

                if (group.name == ComponentString.STATE_DISABLED)
                    code.WritePropertyEquality(name, nameof(NSButton.Enabled), false);

                if (group.name == ComponentString.STATE_DEFAULT)
                    code.WritePropertyEquality (name, nameof(NSButton.KeyEquivalent), "\\r", true);
            }

            return code;
        }
    }
}
