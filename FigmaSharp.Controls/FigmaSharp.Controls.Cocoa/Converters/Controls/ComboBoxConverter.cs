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
using Foundation;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Controls.Cocoa.Helpers;

namespace FigmaSharp.Controls.Cocoa.Converters
{
    public class ComboBoxConverter : CocoaConverter
	{
		internal override bool HasHeightConstraint() => false;

		public override Type GetControlType(FigmaNode currentNode) => typeof(NSComboBox);

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var controlType) &&
                controlType == FigmaControlType.ComboBox;
		}


		protected override IView OnConvertToView(FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
		{
			var combobox = new NSComboBox ();

			var frame = (FigmaFrame)currentNode;
			frame.TryGetNativeControlVariant (out var controlVariant);

			combobox.ControlSize = ViewHelper.GetNSControlSize(controlVariant);
			combobox.Font = ViewHelper.GetNSFont(controlVariant);

			FigmaText text = frame.children
			   .OfType<FigmaText> ()
			   .FirstOrDefault (s => s.name == ComponentString.TITLE);

			if (text != null && !string.IsNullOrEmpty(text.characters))
				combobox.StringValue = rendererService.GetTranslatedText (text);

			return new View(combobox);
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

			code.WritePropertyEquality(name, nameof(NSButton.ControlSize), ViewHelper.GetNSControlSize(controlVariant));
			code.WritePropertyEquality(name, nameof(NSSegmentedControl.Font), CodeHelper.GetNSFontString(controlVariant));

			FigmaText text = frame.children
				.OfType<FigmaText> ()
				.FirstOrDefault (s => s.name == ComponentString.TITLE);

			if (text != null && !string.IsNullOrEmpty (text.characters)) {
				code.WriteTranslatedEquality (name, nameof(NSButton.StringValue), text, rendererService);

				//string textLabel = NativeControlHelper.GetTranslatableString(text.characters, rendererService.CurrentRendererOptions.TranslateLabels);

				//if (!rendererService.CurrentRendererOptions.TranslateLabels)
				//	textLabel = $"\"{textLabel}\"";

				//string nsstringcontructor = typeof (Foundation.NSString).GetConstructor (new[] { textLabel });
				//code.WriteMethod (name, nameof (NSComboBox.Add), nsstringcontructor);
			}

			return code;
		}
	}
}
