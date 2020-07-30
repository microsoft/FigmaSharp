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
    public class TextFieldConverter : CocoaConverter
	{
		internal override bool HasHeightConstraint() => false;

		public override Type GetControlType(FigmaNode currentNode)
		{
			FigmaNode optionsGroup = currentNode.Options();

			FigmaNode passwordNode = optionsGroup?.GetChildren()
				.OfType<FigmaNode>()
				.FirstOrDefault(s => s.name == ComponentString.PASSWORD && s.visible);

			if (passwordNode != null)
				return typeof(NSSecureTextField);


			currentNode.TryGetNativeControlType(out var controlType);

			if (controlType == FigmaControlType.SearchField)
				return typeof(NSSearchField);

			return typeof(NSTextField);
		}

		public override bool CanConvert(FigmaNode currentNode)
		{
			currentNode.TryGetNativeControlType(out var controlType);

			return controlType == FigmaControlType.TextField ||
				   controlType == FigmaControlType.SearchField;
		}


		protected override IView OnConvertToView (FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
		{
			var textField = new NSTextField();

			var frame = (FigmaFrame) currentNode;
			frame.TryGetNativeControlType(out var controlType);
			frame.TryGetNativeControlVariant(out var controlVariant);


			if (controlType == FigmaControlType.SearchField)
				textField = new NSSearchField();


			FigmaNode optionsGroup = frame.Options();

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
				textField.PlaceholderString = rendererService.GetTranslatedText (placeholderText);

			FigmaText text = frame.children
				.OfType<FigmaText> ()
                .FirstOrDefault (s => s.name == ComponentString.TITLE && s.visible);

			if (text != null)
			{
				textField.Alignment = ViewHelper.GetNSTextAlignment(text);
				textField.StringValue = rendererService.GetTranslatedText (text);
			}

			textField.ControlSize = ViewHelper.GetNSControlSize(controlVariant);
			textField.Font = ViewHelper.GetNSFont(controlVariant);

			return new View(textField);
		}


		protected override StringBuilder OnConvertToCode (CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
		{
			var code = new StringBuilder();
			string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

			var frame = (FigmaFrame)currentNode.Node;
			currentNode.Node.TryGetNativeControlType(out FigmaControlType controlType);
			currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

			if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
				code.WriteConstructor(name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

			FigmaNode optionsGroup = frame.Options();

			FigmaNode passwordNode = optionsGroup?.GetChildren()
				.OfType<FigmaNode>()
				.FirstOrDefault(s => s.name == ComponentString.PASSWORD && s.visible);

			if (passwordNode != null)
				code.WritePropertyEquality(name, nameof(NSSecureTextField.StringValue), ComponentString.PASSWORD, inQuotes: true);

			FigmaText placeholderText = optionsGroup?.GetChildren().
				OfType<FigmaText>().
				FirstOrDefault (s => s.name == ComponentString.PLACEHOLDER && s.visible);

			if (placeholderText != null && !placeholderText.characters.Equals(ComponentString.PLACEHOLDER, StringComparison.InvariantCultureIgnoreCase)) {
				code.WriteTranslatedEquality(name, nameof(NSTextField.PlaceholderString), placeholderText, rendererService);
			}

			FigmaText text = frame.children.
                OfType<FigmaText> ().
                FirstOrDefault (s => s.name == ComponentString.TITLE && s.visible);

			if (text != null) {
				code.WritePropertyEquality(name, nameof(NSTextField.Font), CodeHelper.GetNSFontString(controlVariant, text, withWeight: false));
				code.WriteTranslatedEquality (name, nameof (NSTextField.StringValue), text, rendererService);
			}

			return code;
		}
	}
}
