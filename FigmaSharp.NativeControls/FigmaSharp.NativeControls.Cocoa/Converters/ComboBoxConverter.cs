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

namespace FigmaSharp.NativeControls.Cocoa
{
    public class ComboBoxConverter : CocoaConverter
	{
		public override Type GetControlType(FigmaNode currentNode) => typeof(NSComboBox);

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var controlType) &&
                controlType == NativeControlType.ComboBox;
		}


		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			var combobox = new NSComboBox ();

			var frame = (FigmaFrame)currentNode;
			frame.TryGetNativeControlVariant (out var controlVariant);

			switch (controlVariant) {
				case NativeControlVariant.Regular:
					combobox.ControlSize = NSControlSize.Regular;
					combobox.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
					break;
				case NativeControlVariant.Small:
					combobox.ControlSize = NSControlSize.Small;
					combobox.Font = NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize);
					break;
			}

			FigmaText text = frame.children
			   .OfType<FigmaText> ()
			   .FirstOrDefault (s => s.name == "lbl");

			if (text != null && !string.IsNullOrEmpty(text.characters))
				combobox.StringValue = text.characters;

			return new View(combobox);
		}


		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var code = new StringBuilder ();
			string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

			if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
				code.WriteConstructor (name, GetControlType (currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

			code.Configure (currentNode.Node, name);

			var frame = (FigmaFrame) currentNode.Node;
			frame.TryGetNativeControlVariant (out var controlVariant);

			switch (controlVariant) {
				case NativeControlVariant.Small:
					code.WriteEquality(name, nameof(NSButton.Font),
					    CodeGenerationHelpers.Font.SystemFontOfSize(CodeGenerationHelpers.Font.SystemFontSize));
					break;
				case NativeControlVariant.Regular:
					code.WriteEquality (name, nameof (NSButton.ControlSize), NSControlSize.Small);
					code.WriteEquality(name, nameof(NSButton.Font),
	                    CodeGenerationHelpers.Font.SystemFontOfSize(CodeGenerationHelpers.Font.SmallSystemFontSize));
					break;
			}

			FigmaText text = frame.children
				.OfType<FigmaText> ()
				.FirstOrDefault (s => s.name == "lbl");

			if (text != null && !string.IsNullOrEmpty (text.characters)) {
				code.WriteEquality(name, nameof(NSButton.StringValue), text.characters);

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
