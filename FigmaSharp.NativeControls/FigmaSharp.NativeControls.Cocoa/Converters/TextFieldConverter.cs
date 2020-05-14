/* 
 * CustomTextFieldConverter.cs
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *   Hylke Bons <hylbo@microsoft.com>
 *
 * Copyright (C) 2020 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

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
    public class TextFieldConverter : FigmaNativeControlConverter
	{
		public override Type GetControlType(FigmaNode currentNode)
		{
			FigmaNode optionsGroup = currentNode.GetChildren()
                .FirstOrDefault(s => s.name == "!options" && s.visible);

			FigmaNode passwordNode = optionsGroup?.GetChildren()
				.OfType<FigmaNode>()
				.FirstOrDefault(s => s.name == "password" && s.visible);

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


		protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var textField = new NSTextField();

			var frame = (FigmaFrameEntity) currentNode;
			frame.TryGetNativeControlType(out var controlType);
			frame.TryGetNativeControlVariant(out var controlVariant);

			if (controlType == NativeControlType.SearchField)
				textField = new NSSearchField();

			textField.Configure (currentNode);


			FigmaNode optionsGroup = frame.children.FirstOrDefault(s => s.name == "!options" && s.visible);

			FigmaNode passwordNode = optionsGroup?.GetChildren()
	            .OfType<FigmaNode>()
	            .FirstOrDefault(s => s.name == "password" && s.visible);

			if (passwordNode != null)
				textField = new NSSecureTextField();


			FigmaText placeholderText = optionsGroup?.GetChildren()
				.OfType<FigmaText>()
				.FirstOrDefault(s => s.name == "placeholder" && s.visible);

			if (placeholderText != null && !placeholderText.characters.Equals("Placeholder", StringComparison.InvariantCultureIgnoreCase))
				textField.PlaceholderString = placeholderText.characters;


			switch (controlVariant) {
				case NativeControlVariant.Regular:
					textField.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
					break;
				case NativeControlVariant.Small:
					textField.ControlSize = NSControlSize.Small;
					break;
			}


			FigmaText text = frame.children
				.OfType<FigmaText> ()
                .FirstOrDefault (s => s.name == "lbl" && s.visible);

            if (text != null)
				textField.StringValue = text.characters;


			return new View(textField);
		}


		protected override StringBuilder OnConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var instance = (FigmaFrameEntity)currentNode.Node;
			var name = currentNode.Name;

			var builder = new StringBuilder ();
            /*
			if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
				builder.WriteConstructor (name,  GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

			builder.Configure (instance, name);

			instance.TryGetNativeControlComponentType(out var controlType);
			switch (controlType)
			{
				case NativeControlComponentType.TextFieldStandard:
					builder.WriteEquality(currentNode.Name, nameof(NSButton.Font),
						CodeGenerationHelpers.Font.SystemFontOfSize(CodeGenerationHelpers.Font.SystemFontSize));
					break;
			}

			var texts = instance.children.OfType<FigmaText> ();

			var figmaTextNode = texts.FirstOrDefault (s => s.name == "lbl" && s.visible);

			if (figmaTextNode != null) {
				var stringLabel = NativeControlHelper.GetTranslatableString(figmaTextNode.characters, rendererService.CurrentRendererOptions.TranslateLabels);
				builder.WriteEquality (name, nameof (NSTextField.StringValue), stringLabel, inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);
				//builder.Configure (figmaTextNode, name);
			}

			var placeholderTextNode = texts.FirstOrDefault (s => s.name == "placeholder");
			if (placeholderTextNode != null && !placeholderTextNode.characters.Equals("Placeholder", StringComparison.InvariantCultureIgnoreCase)) {
				var stringLabel = NativeControlHelper.GetTranslatableString(placeholderTextNode.characters, rendererService.CurrentRendererOptions.TranslateLabels);
				builder.WriteEquality(name, nameof(NSTextField.PlaceholderString), stringLabel, true);
			}
            */
			return builder;
		}
	}
}
