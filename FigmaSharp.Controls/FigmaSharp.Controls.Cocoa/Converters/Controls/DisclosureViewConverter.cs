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
using System.Text;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.Controls.Cocoa.Converters
{
    public class DisclosureViewConverter : CocoaConverter
	{
		internal override bool HasHeightConstraint() => false;

		public override Type GetControlType(FigmaNode currentNode) => typeof(NSView);

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var controlType) &&
                controlType == FigmaControlType.DisclosureView;
		}

		protected override IView OnConvertToView(FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
		{
			var frame = (FigmaFrame)currentNode;

			var disclosureView = new NSView();

			/* TODO
			var subView = new NSView(disclosureView.Bounds);
			subView.WantsLayer = true;
		    subView.Layer.BackgroundColor = NSColor.Red.CGColor;


			var disclosureTriangle = new NSButton();
			disclosureTriangle.SetFrameSize(new CoreGraphics.CGSize(130, 13));
			disclosureTriangle.SetButtonType(NSButtonType.PushOnPushOff);
			disclosureTriangle.BezelStyle = NSBezelStyle.Disclosure;

            disclosureTriangle.Activated += delegate {

			//	disclosureTriangle.State = NSCellStateValue.Off;

	        };


			disclosureView.AddSubview(disclosureTriangle);
			disclosureView.AddSubview(subView);


			disclosureTriangle.LeftAnchor.ConstraintEqualToAnchor(disclosureView.LeftAnchor, 8)
							.Active = true;

			disclosureTriangle.TopAnchor.ConstraintEqualToAnchor(disclosureView.TopAnchor, 8)
							.Active = true;
			*/
			return new View(disclosureView);
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

			/* TODO

			code.WriteMethod (name, nameof (NSButton.SetButtonType), NSButtonType.PushOnPushOff);
			code.WriteEquality (name, nameof (NSButton.BezelStyle), NSBezelStyle.Disclosure);
			code.WriteEquality (name, nameof (NSButton.Title), CodeGenerationHelpers.StringEmpty);
			code.WriteMethod (name, nameof (NSButton.Highlight), false);

			figmaInstance.TryGetNativeControlComponentType (out var controlType);
			switch (controlType) {
				case NativeControlComponentType.DisclosureTriangleStandard:
				case NativeControlComponentType.DisclosureTriangleStandardDark:
					code.WriteEquality (name, nameof (NSButton.ControlSize), NSControlSize.Regular);
					break;
			}
            */
			return code;
		}
	}
}
