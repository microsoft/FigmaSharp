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

using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

using FigmaSharp.Cocoa;

using AppKit;

namespace FigmaSharp.Controls.Cocoa
{
    public class LabelConverter : CocoaConverter
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSTextField);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);

            return controlType == FigmaControlType.Label ||
                   controlType == FigmaControlType.LabelHeader ||
                   controlType == FigmaControlType.LabelGroup ||
                   controlType == FigmaControlType.LabelSecondary;
        }


        const int headerFontSize = 16;

        protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
        {
            var label = new NSTextField();

            label.Editable = false;
            label.DrawsBackground = false;
            label.Bordered = false;
            label.PreferredMaxLayoutWidth = 1;

            var frame = (FigmaFrame)currentNode;

            FigmaText text = frame.children
                .OfType<FigmaText>()
                .FirstOrDefault(s => s.name == ComponentString.TITLE);

            currentNode.TryGetNativeControlType(out FigmaControlType controlType);
            currentNode.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

            if (text != null)
            {
                label.StringValue = text.characters;
                label.Alignment = CocoaHelpers.GetNSTextAlignment(text);
                label.Font = CocoaHelpers.GetNSFont(controlVariant, text);
            }

            if (controlType == FigmaControlType.LabelHeader)
                label.Font = NSFont.SystemFontOfSize(headerFontSize, CocoaHelpers.GetNSFontWeight(text));

            if (text?.styles != null) {
                foreach (var styleMap in text.styles)
                {
                    if (rendererService.FileProvider.TryGetStyle(styleMap.Value, out FigmaStyle style))
                    {
                        if (styleMap.Key == "fill")
                            label.TextColor = CocoaHelpers.GetNSColor(style.name);
                    }
                }
            }

            return new View(label);
        }

        protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            var code = new StringBuilder();
            string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

            var frame = (FigmaFrame)currentNode.Node;
            currentNode.Node.TryGetNativeControlType(out FigmaControlType controlType);
            currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

            if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
                code.WriteConstructor(name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

            FigmaText text = (FigmaText)frame.children
                .OfType<FigmaText>()
                .FirstOrDefault(s => s.name == ComponentString.TITLE);

            if (text == null)
                return null;

            code.WriteEquality(name, nameof(NSTextField.Editable), false);
            code.WriteEquality(name, nameof(NSTextField.Bordered), false);
            code.WriteEquality(name, nameof(NSTextField.DrawsBackground), false);
            code.WriteEquality(name, nameof(NSTextField.PreferredMaxLayoutWidth), "1");

            var labelComponent = NativeControlHelper.GetTranslatableString(text.characters, rendererService.CurrentRendererOptions.TranslateLabels);
            code.WriteEquality(name, nameof(NSTextField.StringValue), labelComponent, inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);

            if (text != null)
                code.WriteEquality(name, nameof(NSTextField.Alignment), CocoaCodeHelpers.GetNSTextAlignmentString(text).ToString());

            if (controlType == FigmaControlType.LabelHeader)
            {
                code.WriteEquality(name, nameof(NSTextField.Font),
                    $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ headerFontSize }, { CocoaCodeHelpers.GetNSFontWeightString(text) })");
            }
            else
            {
                code.WriteEquality(name, nameof(NSTextField.Font), CocoaCodeHelpers.GetNSFontString(controlVariant, text));
            }

            foreach (var styleMap in text?.styles)
            {
                if ((rendererService.figmaProvider as FigmaFileProvider).TryGetStyle(styleMap.Value, out FigmaStyle style))
                {
                    if (styleMap.Key == "fill")
                        code.WriteEquality(name, nameof(NSTextField.TextColor), CocoaCodeHelpers.GetNSColorString(style.name));
                }
            }

            return code;
        }
    }
}
