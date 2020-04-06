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
	public class PopUpButtonConverter : FigmaNativeControlConverter
	{
		public override Type ControlType => typeof(NSPopUpButton);

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var value) && value == NativeControlType.PopupButton;
		}

		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var figmaInstance = (FigmaFrameEntity)currentNode;

			var button = new ComboBox();
			var view = (NSPopUpButton)button.NativeObject;
			view.Configure(figmaInstance);

			figmaInstance.TryGetNativeControlComponentType(out var controlType);
			switch (controlType)
			{
				case NativeControlComponentType.PopUpButtonSmall:
				case NativeControlComponentType.PopUpButtonSmallDark:
					view.ControlSize = NSControlSize.Small;
					break;
				case NativeControlComponentType.PopUpButtonStandard:
				case NativeControlComponentType.PopUpButtonStandardDark:
					view.ControlSize = NSControlSize.Regular;
					break;
			}

			var label = figmaInstance.children
				   .OfType<FigmaText>()
				   .FirstOrDefault(s => s.name == "lbl");

			if (label != null)
			{
				button.AddItem(label.characters);
				//view.Font = label.style.ToNSFont ();
			}

			return button;
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var figmaInstance = (FigmaFrameEntity)currentNode.Node;

			var builder = new StringBuilder();
			var name = Resources.Ids.Conversion.NameIdentifier;

			if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
				builder.WriteConstructor(name, ControlType, !currentNode.Node.TryGetNodeCustomName(out var _));

			builder.Configure(currentNode.Node, name);

			builder.WriteEquality(name, nameof(NSButton.BezelStyle), NSBezelStyle.Rounded);

			figmaInstance.TryGetNativeControlComponentType(out var controlType);

			switch (controlType)
			{
				case NativeControlComponentType.PopUpButtonSmall:
				case NativeControlComponentType.PopUpButtonSmallDark:
					builder.WriteEquality(name, nameof(NSButton.ControlSize), NSControlSize.Small);
					break;
				case NativeControlComponentType.PopUpButtonStandard:
				case NativeControlComponentType.PopUpButtonStandardDark:
					builder.WriteEquality(name, nameof(NSButton.ControlSize), NSControlSize.Regular);
					break;
			}

			var label = figmaInstance.children
			   .OfType<FigmaText>()
			   .FirstOrDefault(s => s.name == "lbl");

			if (label != null && !string.IsNullOrEmpty(label.characters))
			{
				var stringLabel = NativeControlHelper.GetTranslatableString(label.characters, rendererService.CurrentRendererOptions.TranslateLabels);
				builder.WriteMethod(name, nameof(NSPopUpButton.AddItem), stringLabel, inQuotes: true);
			}

			//if (controlType.ToString ().EndsWith ("Dark", StringComparison.Ordinal)) {
			//	builder.AppendLine (string.Format ("{0}.Appearance = NSAppearance.GetAppearance ({1});", name, NSAppearance.NameDarkAqua.GetType ().FullName));
			//}

			return builder;
		}
	}
}
