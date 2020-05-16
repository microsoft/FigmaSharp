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

namespace FigmaSharp.NativeControls.Cocoa
{
    public class BoxConverter : CocoaConverter
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSBox);

        public override bool CanConvert(FigmaNode currentNode)
        {
            currentNode.TryGetNativeControlType(out var value);

            return value == NativeControlType.Box ||
                   value == NativeControlType.BoxCustom ||
                   value == NativeControlType.Separator;
        }


        protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
        {
            var frame = (FigmaFrame) currentNode;
            var box = new NSBox();

            currentNode.TryGetNativeControlType(out NativeControlType controlType);

            if (controlType == NativeControlType.Separator)
                box.BoxType = NSBoxType.NSBoxSeparator;

            if (controlType == NativeControlType.BoxCustom)
            {
                box.BoxType = NSBoxType.NSBoxCustom;

                // TODO
                // box.CornerRadius = frame.radius
                //
                // if (frame.HasFills)
                //     box.FillColor = frame.fills[0];
                //
                // if (frame.HasStrokes)
                //     box.BorderColor = frame.strokes[0]
            }

            if (controlType == NativeControlType.Box)
            {
                FigmaText text = frame.children
                   .OfType<FigmaText>()
                   .FirstOrDefault(s => (s.name == "Title" && s.visible));

                if (text != null)
                    box.Title = text.characters;
                else
                    box.Title = "";
            }

            return new View(box);
        }

        protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            var code = new StringBuilder();

            string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;
            var frame = (FigmaFrame)currentNode.Node;

            currentNode.Node.TryGetNativeControlType(out NativeControlType controlType);

            code.WriteConstructor(name, typeof(NSBox));

            if (controlType == NativeControlType.Separator)
                code.WriteEquality(name, nameof(NSBox.BoxType), NSBoxType.NSBoxSeparator);

            if (controlType == NativeControlType.BoxCustom)
            {
                code.WriteEquality(name, nameof(NSBox.BoxType), NSBoxType.NSBoxCustom);

                // TODO
                // box.CornerRadius = frame.radius
                //
                // if (frame.HasFills)
                //     box.FillColor = frame.fills[0];
                //
                // if (frame.HasStrokes)
                //     box.BorderColor = frame.strokes[0]
            }

            FigmaText text = frame.children
               .OfType<FigmaText>()
               .FirstOrDefault(s => (s.name == "Title" && s.visible));

            if (text != null)
            {
                string stringTitle = NativeControlHelper.GetTranslatableString(text.characters,
                    rendererService.CurrentRendererOptions.TranslateLabels);

                code.WriteMethod(name, nameof(NSBox.Title), stringTitle,
                    inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);
            }
            else
            {
                code.WriteMethod(name, nameof(NSBox.Title), "", inQuotes: true);
            }

            return code;
        }
    }
}
