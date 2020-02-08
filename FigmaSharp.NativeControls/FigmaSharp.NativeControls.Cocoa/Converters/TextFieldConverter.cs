/* 
 * CustomTextFieldConverter.cs
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
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
using AppKit;
using FigmaSharp.NativeControls.Base;
using System.Linq;
using System.Text;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Views;
using FigmaSharp.Services;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class TextFieldConverter : TextFieldConverterBase
	{
		public override IView ConvertTo (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var figmaInstance = (FigmaFrameEntity) currentNode;

			figmaInstance.TryGetNativeControlType (out var controlType);
			ITextBox textBox = controlType == NativeControlType.Filter ? (ITextBox) new SearchBox () : new TextBox ();
			var view = (NSTextField)textBox.NativeObject;

			view.Configure (figmaInstance);

			figmaInstance.TryGetNativeControlComponentType (out var controlComponentType);
			switch (controlComponentType) {
				case NativeControlComponentType.TextFieldSmall:
				case NativeControlComponentType.TextFieldSmallDark:
				case NativeControlComponentType.FilterSmall:
				case NativeControlComponentType.FilterSmallDark:
					view.ControlSize = NSControlSize.Small;
					break;
				case NativeControlComponentType.FilterStandard:
				case NativeControlComponentType.FilterStandardDark:
				case NativeControlComponentType.TextFieldStandard:
				case NativeControlComponentType.TextFieldStandardDark:
					view.ControlSize = NSControlSize.Regular;
					break;
			}
	
			var texts = figmaInstance.children
				.OfType<FigmaText> ();

			var text = texts.FirstOrDefault (s => s.name == "lbl");
			if (text != null) {
				textBox.Text = text.characters;
				view.Configure (text);
			}

			var placeholder = texts.FirstOrDefault (s => s.name == "placeholder");
			if (placeholder != null)
				view.PlaceholderString = placeholder.characters;

			if (controlComponentType.ToString ().EndsWith ("Dark", System.StringComparison.Ordinal)) {
				view.Appearance = NSAppearance.GetAppearance (NSAppearance.NameDarkAqua);
			}

			return textBox;
		}

		public override string ConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var instance = (FigmaInstance)currentNode.Node;
			var name = currentNode.Name;

			var builder = new StringBuilder ();

			if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
				builder.WriteConstructor (name, typeof (NSTextField));

			builder.Configure (instance, name);

			var texts = instance.children.OfType<FigmaText> ();

			var figmaTextNode = texts.FirstOrDefault (s => s.name == "lbl");

			if (figmaTextNode != null) {
				var text = figmaTextNode.characters ?? string.Empty;
				
				builder.WriteEquality (name, nameof (NSTextField.StringValue), text, true);
				builder.Configure (figmaTextNode, name);
			}

			var placeholderTextNode = texts.FirstOrDefault (s => s.name == "placeholder");
			if (placeholderTextNode != null)
				builder.WriteEquality (name, nameof (NSTextField.PlaceholderString), placeholderTextNode.characters, true);

			return builder.ToString ();
		}
	}
}
