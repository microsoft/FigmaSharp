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
	public class RadioConverter : CocoaConverter
	{
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSButton);

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var controlType) &&
                controlType == NativeControlType.Radio;
		}

		public override bool ScanChildren(FigmaNode currentNode)
		{
			return false;
		}


		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			var frame = (FigmaFrame)currentNode;

			var radio = new NSButton();
			radio.SetButtonType(NSButtonType.Radio);

			FigmaText text = frame.children
				.OfType<FigmaText>()
                .FirstOrDefault(s => s.name == ComponentString.TITLE);

            if (text != null)
                radio.Title = text.characters;

            frame.TryGetNativeControlVariant(out var controlVariant);

            radio.ControlSize = CocoaHelpers.GetNSControlSize(controlVariant);
            radio.Font = CocoaHelpers.GetNSFont(controlVariant, text);

            FigmaGroup group = frame.children
				.OfType<FigmaGroup>()
				.FirstOrDefault(s => (s.name == ComponentString.STATE_ON || s.name == ComponentString.STATE_OFF) && s.visible);

			if (group != null)
			{
				if (group.name == ComponentString.STATE_ON)
					radio.State = NSCellStateValue.On;

				if (group.name == ComponentString.STATE_OFF)
					radio.State = NSCellStateValue.Off;
			}

			return new View(radio);
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
            var code = new StringBuilder();
            string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

            var frame = (FigmaFrame)currentNode.Node;
            currentNode.Node.TryGetNativeControlType(out NativeControlType controlType);
            currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

            if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
                code.WriteConstructor(name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

            code.WriteMethod(name, nameof(NSButton.SetButtonType), NSButtonType.Radio);

            code.WriteEquality(name, nameof(NSButton.ControlSize), CocoaHelpers.GetNSControlSize(controlVariant));
            code.WriteEquality(name, nameof(NSSegmentedControl.Font), CocoaCodeHelpers.GetNSFontName(controlVariant));

            FigmaText text = frame.children
                .OfType<FigmaText>()
                .FirstOrDefault();

            if (text != null)
            {
                var labelTranslated = NativeControlHelper.GetTranslatableString(text.characters, rendererService.CurrentRendererOptions.TranslateLabels);

                code.WriteEquality(name, nameof(NSButton.Title), labelTranslated,
                    inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);
            }

            FigmaGroup group = frame.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => (s.name == ComponentString.STATE_ON || s.name == ComponentString.STATE_OFF) && s.visible);

            if (group != null)
            {
                if (group.name == ComponentString.STATE_ON)
                    code.WriteEquality(name, nameof(NSButton.State), NSCellStateValue.On);

                if (group.name == ComponentString.STATE_OFF)
                    code.WriteEquality(name, nameof(NSButton.State), NSCellStateValue.Off);
            }

            return code;
        }
	}
}
