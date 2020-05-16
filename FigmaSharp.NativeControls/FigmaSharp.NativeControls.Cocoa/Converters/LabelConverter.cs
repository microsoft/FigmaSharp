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

using AppKit;

namespace FigmaSharp.NativeControls.Cocoa
{
    public class LabelConverter : CocoaConverter
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSTextField);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var controlType);

            return controlType == NativeControlType.Label ||
                   controlType == NativeControlType.LabelHeader ||
                   controlType == NativeControlType.LabelGroup ||
                   controlType == NativeControlType.LabelSecondary;
        }


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
                .FirstOrDefault();

            currentNode.TryGetNativeControlType(out NativeControlType controlType);
            currentNode.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

            if (text != null)
            {
                label.StringValue = text.characters;
                label.Alignment = GetNSTextAlignment(text);
                label.Font = GetNSFont(controlVariant, text);
            }

            if (controlType == NativeControlType.LabelSecondary)
                label.TextColor = NSColor.SecondaryLabelColor;

            if (controlType == NativeControlType.LabelHeader)
                label.Font = NSFont.SystemFontOfSize(16, GetNSFontWeight(text));

            return new View(label);
        }

        protected override StringBuilder OnConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            var figmaInstance = (FigmaFrame)currentNode.Node;
            var figmaText = (FigmaText)figmaInstance.children
                .FirstOrDefault(s => s.name == "lbl");

            if (figmaText == null)
                return null;

            StringBuilder builder = new StringBuilder();
            /*
            if (rendererService.NeedsRenderConstructor(currentNode, parentNode)) {
                builder.WriteConstructor(currentNode.Name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

                builder.WriteEquality(currentNode.Name, nameof(NSTextField.Editable), false);
                builder.WriteEquality(currentNode.Name, nameof(NSTextField.Bordered), false);
                builder.WriteEquality(currentNode.Name, nameof(NSTextField.Bezeled), false);
                builder.WriteEquality(currentNode.Name, nameof(NSTextField.DrawsBackground), false);

                var labelComponent = NativeControlHelper.GetTranslatableString(figmaText.characters, rendererService.CurrentRendererOptions.TranslateLabels);
                builder.WriteEquality(currentNode.Name, nameof(NSTextField.StringValue), labelComponent, inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);
            }

            currentNode.Node.TryGetNativeControlComponentType(out NativeControlComponentType componentType);

            switch (componentType)
            {
                default:
                    builder.WriteEquality(currentNode.Name, nameof(NSTextField.Font),
                        CodeGenerationHelpers.Font.SystemFontOfSize(CodeGenerationHelpers.Font.SystemFontSize));
                    break;
                case NativeControlComponentType.LabelGroup:
                    builder.WriteEquality(currentNode.Name, nameof(NSButton.Font),
                        CodeGenerationHelpers.Font.BoldSystemFontOfSize(CodeGenerationHelpers.Font.SystemFontSize));
                    break;
                case NativeControlComponentType.LabelSmall:
                    builder.WriteEquality(currentNode.Name, nameof(NSButton.Font),
                        CodeGenerationHelpers.Font.SystemFontOfSize(CodeGenerationHelpers.Font.SmallSystemFontSize));
                    break;
                case NativeControlComponentType.LabelSecondary:
                    builder.WriteEquality(currentNode.Name, nameof(NSTextField.TextColor), string.Format("{0}.{1}", typeof(NSColor), nameof(NSColor.SecondaryLabelColor)));
                    builder.WriteEquality(currentNode.Name, nameof(NSButton.Font),
                        CodeGenerationHelpers.Font.SystemFontOfSize(CodeGenerationHelpers.Font.SmallSystemFontSize));
                    break;
            }

            //builder.Configure(figmaText, currentNode.Name);
            builder.Configure(currentNode.Node, currentNode.Name);


            var alignment = FigmaExtensions.ToNSTextAlignment(figmaText.style.textAlignHorizontal);
            if (alignment != default) {
                builder.WriteEquality(currentNode.Name, nameof(AppKit.NSTextField.Alignment), alignment);
            }
            */
            return builder;
        }

        // void CreateLink(NSTextField textField)
        // {
        //     var attributedString = new NSMutableAttributedString(textField.AttributedStringValue);
        //
        //     attributedString.AddAttribute(NSStringAttributeKey.UnderlineStyle,
        //                                  NSNumber.FromNInt((int)NSUnderlineStyle.Single),
        //                                  new NSRange(0, attributedString.Length));
        //
        //    attributedString.AddAttribute(NSStringAttributeKey.ForegroundColor,
        //                                  NSColor.LinkColor,
        //                                  new NSRange(0, attributedString.Length));
        //
        //    textField.AttributedStringValue = attributedString;
        //}
    }
}
