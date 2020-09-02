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
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Controls.Cocoa.Converters
{
    public class BoxConverter : CocoaConverter
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSBox);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var value);

            return value == FigmaControlType.Box ||
                   value == FigmaControlType.BoxCustom ||
                   value == FigmaControlType.Separator;
        }


        protected override IView OnConvertToView(FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
        {
            var frame = (FigmaFrame)currentNode;
            var box = new NSBox();

            currentNode.TryGetNativeControlType(out FigmaControlType controlType);

            if (controlType == FigmaControlType.Separator)
                box.BoxType = NSBoxType.NSBoxSeparator;

            if (controlType == FigmaControlType.BoxCustom)
            {
                box.BoxType = NSBoxType.NSBoxCustom;
                box.BorderWidth = 0;

                RectangleVector rectangle = frame.children
                    .OfType<RectangleVector>()
                    .FirstOrDefault();

                if (rectangle != null)
                {
                    foreach (var styleMap in rectangle.styles)
                    {
                        if (rendererService.NodeProvider.TryGetStyle(styleMap.Value, out FigmaStyle style))
                        {
                            if (styleMap.Key == "fill")
                                box.FillColor = ColorService.GetNSColor(style.name);

                            if (styleMap.Key == "stroke")
                            {
                                box.BorderColor = ColorService.GetNSColor(style.name);
                                box.BorderWidth = rectangle.strokeWeight;
                            }
                        }
                    }

                    box.CornerRadius = rectangle.cornerRadius;
                }
            }

            if (controlType == FigmaControlType.Box)
            {
                FigmaText text = frame.children
                   .OfType<FigmaText>()
                   .FirstOrDefault(s => (s.name == ComponentString.TITLE && s.visible));

                if (text != null)
                    box.Title = rendererService.GetTranslatedText (text);
                else
                    box.Title = string.Empty;
            }

            return new View(box);
        }

        protected override StringBuilder OnConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            var code = new StringBuilder();
            string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

            var frame = (FigmaFrame)currentNode.Node;
            currentNode.Node.TryGetNativeControlType(out FigmaControlType controlType);
            currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

            if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
                code.WriteConstructor(name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

            if (controlType == FigmaControlType.Separator)
                code.WritePropertyEquality(name, nameof(NSBox.BoxType), NSBoxType.NSBoxSeparator);

            if (controlType == FigmaControlType.BoxCustom)
            {
                code.WritePropertyEquality(name, nameof(NSBox.BoxType), NSBoxType.NSBoxCustom);
                bool borderSet = false;

                RectangleVector rectangle = frame.children
                    .OfType<RectangleVector>()
                    .FirstOrDefault();

                foreach (var styleMap in rectangle?.styles)
                {
                    if ((rendererService.NodeProvider as NodeProvider).TryGetStyle(styleMap.Value, out FigmaStyle style))
                    {
                        if (styleMap.Key == "fill")
                            code.WritePropertyEquality(name, nameof(NSBox.FillColor), ColorService.GetNSColorString(style.name));

                        if (styleMap.Key == "stroke")
                        {
                            code.WritePropertyEquality(name, nameof(NSBox.BorderColor), ColorService.GetNSColorString(style.name));
                            code.WritePropertyEquality(name, nameof(NSBox.BorderWidth), rectangle.strokeWeight.ToString());
                            borderSet = true;
                        }
                    }
                }

                code.WritePropertyEquality(name, nameof(NSBox.CornerRadius), rectangle.cornerRadius.ToString());

                if (!borderSet)
                    code.WritePropertyEquality(name, nameof(NSBox.BorderWidth), "0");
            }

            FigmaText text = frame.children
               .OfType<FigmaText>()
               .FirstOrDefault(s => (s.name == ComponentString.TITLE && s.visible));

            if (text != null)
                code.WriteTranslatedEquality(name, nameof(NSBox.Title), text.characters, rendererService);
            else
                code.WritePropertyEquality(name, nameof(NSBox.Title), string.Empty, inQuotes: true);

            return code;
        }
    }
}
