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
using System.Text;
using System.Linq;
using AppKit;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Native.Cocoa;
using Foundation;

namespace FigmaSharp.Cocoa.Converters
{
    public class TextConverter : TextConverterBase
    {
        public override IView ConvertToView (FigmaNode node, ViewNode parent, ViewRenderService rendererService)
        {
            var text = (FigmaText)node;
            var label = new Label();
			var nativeLabel = label.NativeObject as FNSTextField;
            nativeLabel.Font = text.style.ToNSFont();

            nativeLabel.StringValue = text.characters ?? string.Empty;
            nativeLabel.Hidden = !text.visible;
            nativeLabel.Alignment = text.style.textAlignHorizontal == "CENTER" ? NSTextAlignment.Center : text.style.textAlignHorizontal == "LEFT" ? NSTextAlignment.Left : NSTextAlignment.Right;
            nativeLabel.AlphaValue = text.opacity;

            if (nativeLabel.Cell is VerticalAlignmentTextCell cell)
                cell.VerticalAligment = text.style.textAlignVertical == "CENTER" ? VerticalTextAlignment.Center : text.style.textAlignVertical == "TOP" ? VerticalTextAlignment.Top : VerticalTextAlignment.Bottom;

            //text color
            if (text.TryGetNSColorByStyleKey(rendererService, FigmaStyle.Keys.Fill, out var color))
                nativeLabel.TextColor = color;
            else
            {
                var fills = text.fills.FirstOrDefault();
                if (fills?.visible ?? false)
                    nativeLabel.TextColor = fills.color.ToNSColor();
            }

            if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0)
            {
                var attributedText = new NSMutableAttributedString(nativeLabel.AttributedStringValue);
                for (int i = 0; i < text.characterStyleOverrides.Length; i++)
                {
                    var range = new NSRange(i, 1);

                    var key = text.characterStyleOverrides[i].ToString();
                    if (!text.styleOverrideTable.ContainsKey(key))
                    {
                        //we want the default values
                        attributedText.AddAttribute(NSStringAttributeKey.ForegroundColor, nativeLabel.TextColor, range);
                        attributedText.AddAttribute(NSStringAttributeKey.Font, nativeLabel.Font, range);
                        continue;
                    }

                    //if there is a style to override
                    var styleOverrided = text.styleOverrideTable[key];

                    //set the color
                    NSColor fontColorOverrided = nativeLabel.TextColor;
                    var fillOverrided = styleOverrided.fills?.FirstOrDefault();
                    if (fillOverrided != null && fillOverrided.visible)
                        fontColorOverrided = fillOverrided.color.ToNSColor();

                    attributedText.AddAttribute(NSStringAttributeKey.ForegroundColor, fontColorOverrided, range);

                    //TODO: we can improve this
                    //set the font for this character
                    NSFont fontOverrided = nativeLabel.Font;
                    if (styleOverrided.fontFamily != null)
                    {
                        fontOverrided = FigmaExtensions.ToNSFont(styleOverrided);
                    }
                    attributedText.AddAttribute(NSStringAttributeKey.Font, fontOverrided, range);
                }

                nativeLabel.AttributedStringValue = attributedText;
            }
            return label;
        }

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            var figmaText = (FigmaText)currentNode.Node;

            StringBuilder builder = new StringBuilder();
            if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
                builder.WritePropertyEquality (currentNode.Name, null, FigmaExtensions.CreateLabelToDesignerString (figmaText.characters), instanciate: true);

            //builder.Configure(figmaText, currentNode.Name);
            builder.Configure (currentNode.Node, currentNode.Name);

            var alignment = FigmaExtensions.ToNSTextAlignment (figmaText.style.textAlignHorizontal);
			if (alignment != default) {
                builder.WritePropertyEquality (currentNode.Name, nameof (AppKit.NSTextField.Alignment), alignment);
            }
            return builder.ToString();
        }

        public override Type GetControlType(FigmaNode currentNode)
            => typeof (AppKit.NSTextField);
    }
}
