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
using System.Text;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
    public class DisclossureConverter : FigmaNativeControlConverter
	{
		public override Type ControlType => typeof(NSButton);

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var value) && value == NativeControlType.DisclosureTriange;
		}

		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var instance = (FigmaFrameEntity)currentNode;
			var view = new DisclosureTriangle ();
			var nativeView = (FNSButton)view.NativeObject;
			nativeView.Configure (instance);

			instance.TryGetNativeControlComponentType (out var controlType);
			switch (controlType) {
				case NativeControlComponentType.DisclosureTriangleStandard:
				case NativeControlComponentType.DisclosureTriangleStandardDark:
					nativeView.ControlSize = NSControlSize.Regular;
					break;
			}

			return view;
		}

		protected override StringBuilder OnConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var figmaInstance = (FigmaFrameEntity)currentNode.Node;

			StringBuilder builder = new StringBuilder ();
			string name = currentNode.Name;

			if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
				builder.WriteConstructor (name, ControlType, !currentNode.Node.TryGetNodeCustomName(out var _));

			builder.Configure (figmaInstance, name);

			builder.WriteMethod (name, nameof (NSButton.SetButtonType), NSButtonType.PushOnPushOff);
			builder.WriteEquality (name, nameof (NSButton.BezelStyle), NSBezelStyle.Disclosure);
			builder.WriteEquality (name, nameof (NSButton.Title), CodeGenerationHelpers.StringEmpty);
			builder.WriteMethod (name, nameof (NSButton.Highlight), false);

			figmaInstance.TryGetNativeControlComponentType (out var controlType);
			switch (controlType) {
				case NativeControlComponentType.DisclosureTriangleStandard:
				case NativeControlComponentType.DisclosureTriangleStandardDark:
					builder.WriteEquality (name, nameof (NSButton.ControlSize), NSControlSize.Regular);
					break;
			}

			return builder;
		}
	}
}
