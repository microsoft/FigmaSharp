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
    public class TextFieldConverter : CocoaConverter
	{
		public override Type GetControlType(FigmaNode currentNode)
		{
			FigmaNode optionsGroup = currentNode.GetChildren()
                .FirstOrDefault(s => s.name == "!options" && s.visible);

			FigmaNode passwordNode = optionsGroup?.GetChildren()
				.OfType<FigmaNode>()
				.FirstOrDefault(s => s.name == ComponentString.PASSWORD && s.visible);

			if (passwordNode != null)
				return typeof(NSSecureTextField);


			currentNode.TryGetNativeControlType(out var controlType);

			if (controlType == NativeControlType.SearchField)
				return typeof(NSSearchField);

			return typeof(NSTextField);
		}

		public override bool CanConvert(FigmaNode currentNode)
		{
			currentNode.TryGetNativeControlType(out var controlType);

			return controlType == NativeControlType.TextField ||
				   controlType == NativeControlType.SearchField;
		}


		protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
		{
			var textField = new NSTextField();

			var frame = (FigmaFrame) currentNode;
			frame.TryGetNativeControlType(out var controlType);
			frame.TryGetNativeControlVariant(out var controlVariant);


			if (controlType == NativeControlType.SearchField)
				textField = new NSSearchField();


			FigmaNode optionsGroup = frame.children.FirstOrDefault(s => s.name == "!options" && s.visible);

			FigmaNode passwordNode = optionsGroup?.GetChildren()
	            .OfType<FigmaNode>()
	            .FirstOrDefault(s => s.name == ComponentString.PASSWORD && s.visible);

			if (passwordNode != null)
			{
				textField = new NSSecureTextField();
				textField.StringValue = "Password";
			}


			FigmaText placeholderText = optionsGroup?.GetChildren()
				.OfType<FigmaText>()
				.FirstOrDefault(s => s.name == ComponentString.PLACEHOLDER && s.visible);

			if (placeholderText != null && !placeholderText.characters.Equals(ComponentString.PLACEHOLDER, StringComparison.InvariantCultureIgnoreCase))
				textField.PlaceholderString = placeholderText.characters;



			FigmaText text = frame.children
				.OfType<FigmaText> ()
                .FirstOrDefault (s => s.name == ComponentString.TITLE && s.visible);

			if (text != null)
			{
				textField.Alignment = GetNSTextAlignment(text);
				textField.StringValue = text.characters;
			}

			textField.ControlSize = GetNSControlSize(controlVariant);
			textField.Font = GetNSFont(controlVariant);

			return new View(textField);
		}


		protected override StringBuilder OnConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var code = new StringBuilder();
			string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

			var frame = (FigmaFrame)currentNode.Node;
			currentNode.Node.TryGetNativeControlType(out NativeControlType controlType);
			currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

			if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
				code.WriteConstructor(name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

			FigmaNode optionsGroup = frame.children.FirstOrDefault(s => s.name == "!options" && s.visible);

			FigmaNode passwordNode = optionsGroup?.GetChildren()
				.OfType<FigmaNode>()
				.FirstOrDefault(s => s.name == ComponentString.PASSWORD && s.visible);

			if (passwordNode != null)
				code.WriteEquality(name, nameof(NSSecureTextField.StringValue), ComponentString.PASSWORD, inQuotes: true);

			FigmaText placeholderText = optionsGroup?.GetChildren().
				OfType<FigmaText>().
				FirstOrDefault (s => s.name == ComponentString.PLACEHOLDER && s.visible);

			if (placeholderText != null && !placeholderText.characters.Equals(ComponentString.PLACEHOLDER, StringComparison.InvariantCultureIgnoreCase)) {
				var stringLabel = NativeControlHelper.GetTranslatableString(placeholderText.characters, rendererService.CurrentRendererOptions.TranslateLabels);
				code.WriteEquality(name, nameof(NSTextField.PlaceholderString), stringLabel, true);
			}

			FigmaText text = frame.children.
                OfType<FigmaText> ().
                FirstOrDefault (s => s.name == ComponentString.TITLE && s.visible);

			if (text != null) {
				code.WriteEquality(name, nameof(NSTextField.Font), GetNSFontName(controlVariant, text, withWeight: false));

				var stringLabel = NativeControlHelper.GetTranslatableString(text.characters, rendererService.CurrentRendererOptions.TranslateLabels);
				code.WriteEquality (name, nameof (NSTextField.StringValue), stringLabel, inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);
			}

			return code;
		}
	}
}
