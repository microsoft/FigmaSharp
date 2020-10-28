// Authors:
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
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Controls.Cocoa.Converters
{
    public class ButtonSymbolConverter : CocoaConverter
    {
        internal override bool HasHeightConstraint() => false;
        internal override bool IsFlexibleHorizontal(FigmaNode node) => true;
        internal override bool IsFlexibleVertical(FigmaNode node) => true;

        public override bool CanSetAccessibilityLabel => false;


        public override Type GetControlType(FigmaNode currentNode) => typeof(NSButton);

        public override bool CanConvert(FigmaNode currentNode)
        {
            return currentNode.TryGetNativeControlType(out var controlType) &&
                controlType == FigmaControlType.ButtonSymbol;
        }


        protected override IView OnConvertToView (FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
        {
            var button = new NSButton();

            var frame = (FigmaFrame)currentNode;
            frame.TryGetNativeControlType(out var controlType);
            frame.TryGetNativeControlVariant(out var controlVariant);

            FigmaText text = frame.children
                .OfType<FigmaText>()
                .FirstOrDefault(s => s.name == ComponentString.SYMBOL);

            button.BezelStyle = NSBezelStyle.RegularSquare;
            button.Bordered = false;
            button.Font = NSFont.SystemFontOfSize(text.style.fontSize, ViewHelper.GetNSFontWeight(text));
            button.Title = text.characters;

            foreach (var styleMap in text.styles)
            {
                if (rendererService.NodeProvider.TryGetStyle(styleMap.Value, out FigmaStyle style))
                {
                    if (styleMap.Key == "fill")
                        button.ContentTintColor = ColorService.GetNSColor(style.name);
                }
            }

            if (frame.TrySearchA11Label(out var tooltip))
                button.ToolTip = tooltip; // TODO: code generation

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

            FigmaText text = frame.children
                .OfType<FigmaText>()
                .FirstOrDefault(s => s.name == ComponentString.SYMBOL);

            code.WritePropertyEquality(name, nameof(NSButton.BezelStyle), NSBezelStyle.RegularSquare);
            code.WritePropertyEquality(name, nameof(NSButton.Bordered), false);
            code.WritePropertyEquality(name, nameof(NSButton.Title), text.characters, inQuotes: true);

            code.WritePropertyEquality(name, nameof(NSButton.Font),
                $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ text.style.fontSize }, { CodeHelper.GetNSFontWeightString(text) })");

            foreach (var styleMap in text.styles)
            {
                if (rendererService.NodeProvider.TryGetStyle(styleMap.Value, out FigmaStyle style))
                {
                    if (styleMap.Key == "fill")
                        code.WritePropertyEquality(name, nameof(NSButton.ContentTintColor), ColorService.GetNSColorString(style.name));
                }
            }

            return code;
        }
    }
}
