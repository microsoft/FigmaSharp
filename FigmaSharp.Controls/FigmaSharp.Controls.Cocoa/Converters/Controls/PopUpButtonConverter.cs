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
    public class PopUpButtonConverter : CocoaConverter
	{
		internal override bool HasHeightConstraint() => false;

		public override Type GetControlType(FigmaNode currentNode) => typeof(NSPopUpButton);

		public override bool CanConvert(FigmaNode currentNode)
		{
			currentNode.TryGetNativeControlType(out var value);

            return (value == FigmaControlType.PopUpButton ||
                    value == FigmaControlType.PopUpButtonPullDown);
		}


		protected override IView OnConvertToView(FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
		{
			var frame = (FigmaFrame)currentNode;
			frame.TryGetNativeControlVariant(out var controlVariant);
			frame.TryGetNativeControlType(out var controlType);

			var popUp = new NSPopUpButton();

			if (controlType == FigmaControlType.PopUpButtonPullDown)
				popUp.PullsDown = true;

			popUp.ControlSize = ViewHelper.GetNSControlSize(controlVariant);
			popUp.Font = ViewHelper.GetNSFont(controlVariant);

			FigmaText text = frame.children
				   .OfType<FigmaText>()
				   .FirstOrDefault(s => s.name == ComponentString.TITLE);

			if (text != null && !string.IsNullOrEmpty (text.characters))
				popUp.AddItem(rendererService.GetTranslatedText (text.characters));

			return new View(popUp);
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

			code.WritePropertyEquality(name, nameof(NSButton.BezelStyle), NSBezelStyle.Rounded);

			if (controlType == FigmaControlType.PopUpButtonPullDown)
				code.WritePropertyEquality(name, nameof(NSPopUpButton.PullsDown), true);

			code.WritePropertyEquality(name, nameof(NSButton.ControlSize), ViewHelper.GetNSControlSize(controlVariant));
			code.WritePropertyEquality(name, nameof(NSSegmentedControl.Font), CodeHelper.GetNSFontString(controlVariant));

			FigmaText text = frame.children
			   .OfType<FigmaText>()
			   .FirstOrDefault(s => s.name == ComponentString.TITLE);

			if (text != null && !string.IsNullOrEmpty(text.characters)) {
				var stringLabel = rendererService.GetTranslatedText(text);
				code.WriteMethod(name, nameof(NSPopUpButton.AddItem), stringLabel,
					inQuotes: !rendererService.Options.TranslateLabels);
			}

			return code;
		}
	}
}
