/* 
 * CustomButtonConverter.cs 
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *   Hylke Bons <hylbo@microsoft.com>
 *
 * Copyright (C) 2020 Microsoft, Corp
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
    public class ButtonConverter : FigmaNativeControlConverter
    {
        public override Type GetControlType(FigmaNode currentNode)
        {
            return typeof(NSButton);
        }

        public override bool CanConvert(FigmaNode currentNode)
        {
            return currentNode.TryGetNativeControlType(out var value) &&
                value == NativeControlType.Button ||
                value == NativeControlType.ButtonHelp;
        }


        protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
        {
            var button = new NSButton();

            var frame = (FigmaFrameEntity)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            button.Configure(frame);

            switch (controlType)
            {
                case NativeControlType.Button:
                    button.BezelStyle = NSBezelStyle.Rounded;
                    break;
                case NativeControlType.ButtonHelp:
                    button.BezelStyle = NSBezelStyle.HelpButton;
                    button.Title = string.Empty;
                    break;
            }

            switch (controlVariant)
            {
                case NativeControlVariant.Regular:
                    button.ControlSize = NSControlSize.Regular;
                    button.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
                    break;
                case NativeControlVariant.Small:
                    button.ControlSize = NSControlSize.Small;
                    button.Font = NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize);
                    break;
            }

            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.visible);

            if (group != null)
            {
                FigmaText label = group.children
                    .OfType<FigmaText>()
                    .FirstOrDefault();

                if (label != null && controlType != NativeControlType.ButtonHelp)
                    button.Title = label.characters;

                if (group.name == "Disabled")
                {
                    button.Enabled = false;
                }

                if (group.name == "Default")
                {
                    button.KeyEquivalent = "\r";
                }
            }

            return new View(button);
        }


        protected override StringBuilder OnConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            var code = new StringBuilder();
            var frame = (FigmaFrameEntity) currentNode.Node;

            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

            if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
                code.WriteConstructor (name, GetControlType(currentNode.Node).FullName, rendererService.NodeRendersVar (currentNode, parentNode));

            code.Configure (currentNode.Node, name);

            switch (controlType)
            {
                case NativeControlType.Button:
                    code.WriteEquality(name, nameof(NSButton.BezelStyle), NSBezelStyle.Rounded);
                    break;
                case NativeControlType.ButtonHelp:
                    code.WriteEquality(name, nameof(NSButton.BezelStyle), NSBezelStyle.HelpButton);
                    code.WriteEquality(name, nameof(NSButton.Title), string.Empty, inQuotes: true);
                    break;
            }

            switch (controlVariant)
            {
                case NativeControlVariant.Regular:
                    code.WriteEquality(name, nameof(NSButton.ControlSize), NSControlSize.Regular);
                    code.WriteEquality(name, nameof(NSButton.Font), CodeGenerationHelpers.Font.SystemFontOfSize(CodeGenerationHelpers.Font.SystemFontSize));
                    break;
                case NativeControlVariant.Small:
                    code.WriteEquality(name, nameof(NSButton.ControlSize), NSControlSize.Small);
                    code.WriteEquality(name, nameof(NSButton.Font), CodeGenerationHelpers.Font.SystemFontOfSize(CodeGenerationHelpers.Font.SmallSystemFontSize));
                    break;
            }

            FigmaGroup group = frame.children
                .OfType<FigmaGroup> ()
                .FirstOrDefault (s => s.visible);

            if (group != null) {
                FigmaText text = group.children
                    .OfType<FigmaText> ()
                    .FirstOrDefault ();

                if (text != null && controlType != NativeControlType.ButtonHelp)
                {
                    string labelTranslated = NativeControlHelper.GetTranslatableString(text.characters, rendererService.CurrentRendererOptions.TranslateLabels);
                    code.WriteEquality(name, nameof(NSButton.Title), labelTranslated, inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);
                }

                if (group.name == "Disabled")
                {
                    code.WriteEquality(name, nameof(NSButton.Enabled), false);
                }

                if (group.name == "Default")
                {
                    code.WriteEquality (name, nameof(NSButton.KeyEquivalent), "\\r", true);
                }
            }

            return code;
        }
    }
}
