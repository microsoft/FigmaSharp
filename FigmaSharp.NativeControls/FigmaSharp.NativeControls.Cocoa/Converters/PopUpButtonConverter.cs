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
	public class PopUpButtonConverter : CocoaConverter
	{
		public override Type GetControlType(FigmaNode currentNode) => typeof(NSPopUpButton);

		public override bool CanConvert(FigmaNode currentNode)
		{
			currentNode.TryGetNativeControlType(out var value);

            return (value == NativeControlType.PopUpButton ||
                    value == NativeControlType.PopUpButtonPullDown);
		}


		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			var frame = (FigmaFrame)currentNode;
			frame.TryGetNativeControlVariant(out var controlVariant);
			frame.TryGetNativeControlType(out var controlType);

			var popUp = new NSPopUpButton();

			if (controlType == NativeControlType.PopUpButtonPullDown)
				popUp.PullsDown = true;

			popUp.ControlSize = GetNSControlSize(controlVariant);
			popUp.Font = GetNSFont(controlVariant);

			FigmaText text = frame.children
				   .OfType<FigmaText>()
				   .FirstOrDefault(s => s.name == ComponentString.TITLE);

			if (text != null)
				popUp.AddItem(text.characters);

			return new View(popUp);
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

			code.WriteEquality(name, nameof(NSButton.BezelStyle), NSBezelStyle.Rounded);

			if (controlType == NativeControlType.PopUpButtonPullDown)
				code.WriteEquality(name, nameof(NSPopUpButton.PullsDown), true);

			code.WriteEquality(name, nameof(NSButton.ControlSize), GetNSControlSize(controlVariant));
			code.WriteEquality(name, nameof(NSSegmentedControl.Font), GetNSFontName(controlVariant));

			FigmaText text = frame.children
			   .OfType<FigmaText>()
			   .FirstOrDefault(s => s.name == ComponentString.TITLE);

			if (text != null && !string.IsNullOrEmpty(text.characters)) {
				var stringLabel = NativeControlHelper.GetTranslatableString(text.characters,
                    rendererService.CurrentRendererOptions.TranslateLabels);

				code.WriteMethod(name, nameof(NSPopUpButton.AddItem), stringLabel,
					inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);
			}

			return code;
		}
	}
}
