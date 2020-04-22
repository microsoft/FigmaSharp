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
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
	public class SpinnerConverter : FigmaNativeControlConverter
	{
		public override Type ControlType => typeof (NSProgressIndicator);

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var value) && value == NativeControlType.ProgressSpinner;
		}

		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var instance = (FigmaFrameEntity)currentNode;
			var view = new Spinner ();
			var nativeView = (FNSProgressIndicator)view.NativeObject;
			nativeView.Configure (instance);

			instance.TryGetNativeControlComponentType (out var controlType);
			switch (controlType) {
				case NativeControlComponentType.ProgressSpinnerSmall:
				case NativeControlComponentType.ProgressSpinnerSmallDark:
					nativeView.ControlSize = NSControlSize.Small;
					break;
				case NativeControlComponentType.ProgressSpinner:
				case NativeControlComponentType.ProgressSpinnerDark:
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
				builder.WriteConstructor (name,  ControlType, !currentNode.Node.TryGetNodeCustomName(out var _));

			builder.Configure (figmaInstance, name);

			builder.WriteEquality (name, nameof (NSProgressIndicator.Style), NSProgressIndicatorStyle.Spinning);

			//hidden by default
			builder.WriteEquality (name, nameof (NSProgressIndicator.Hidden), true);

			figmaInstance.TryGetNativeControlComponentType (out var controlType);

			switch (controlType) {
				case NativeControlComponentType.ProgressSpinnerSmall:
				case NativeControlComponentType.ProgressSpinnerSmallDark:
					builder.WriteEquality (name, nameof (NSButton.ControlSize), NSControlSize.Small);
					break;
				case NativeControlComponentType.ProgressSpinner:
				case NativeControlComponentType.ProgressSpinnerDark:
					builder.WriteEquality (name, nameof (NSButton.ControlSize), NSControlSize.Regular);
					break;
			}

			return builder;
		}
	}


	public class ProgressBarConverter : FigmaNativeControlConverter
	{
		public override Type ControlType => typeof(NSProgressIndicator);

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.TryGetNativeControlType(out var value) && value == NativeControlType.ProgressBar;
		}

		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var instance = (FigmaFrameEntity)currentNode;
			var view = new ProgressBar();

			var nativeView = (NSProgressIndicator)view.NativeObject;
			nativeView.Style = NSProgressIndicatorStyle.Bar;

			var indeterminateNode = currentNode
				.GetChildren()
				.FirstOrDefault(s => (s.name == "Indeterminate" && s.visible));

			if (indeterminateNode != null)
			{
				nativeView.Indeterminate = true;

			} else {
				nativeView.Indeterminate = false;
				nativeView.MinValue = 0;
				nativeView.MaxValue = 1;
				nativeView.DoubleValue = 0.6180339;
			}

			instance.TryGetNativeControlComponentType(out var controlType);
			switch (controlType)
			{
				case NativeControlComponentType.ProgressBarSmall:
				case NativeControlComponentType.ProgressBarSmallDark:
					nativeView.ControlSize = NSControlSize.Small;
					break;
				case NativeControlComponentType.ProgressBar:
				case NativeControlComponentType.ProgressBarDark:
					nativeView.ControlSize = NSControlSize.Regular;
					break;
			}

			return view;
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var figmaInstance = (FigmaFrameEntity)currentNode.Node;

			StringBuilder builder = new StringBuilder();
			string name = currentNode.Name;

			if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
				builder.WriteConstructor(name, ControlType, !currentNode.Node.TryGetNodeCustomName(out var _));

			builder.Configure(figmaInstance, name);
			builder.WriteEquality(name, nameof(NSProgressIndicator.Style), NSProgressIndicatorStyle.Bar);

			var indeterminateNode = currentNode.Node.GetChildren()
				.FirstOrDefault(s => (s.name == "Indeterminate" && s.visible));

			if (indeterminateNode != null)
			{
				builder.WriteEquality(name, nameof(NSProgressIndicator.Indeterminate), true);
			} else {
				builder.WriteEquality(name, nameof(NSProgressIndicator.Indeterminate), false);
				builder.WriteEquality(name, nameof(NSProgressIndicator.MinValue), "0");
				builder.WriteEquality(name, nameof(NSProgressIndicator.MaxValue), "1");
				builder.WriteEquality(name, nameof(NSProgressIndicator.DoubleValue), "0.6180339");
			}

			figmaInstance.TryGetNativeControlComponentType(out var controlType);

			switch (controlType)
			{
				case NativeControlComponentType.ProgressBarSmall:
				case NativeControlComponentType.ProgressBarSmallDark:
					builder.WriteEquality(name, nameof(NSButton.ControlSize), NSControlSize.Small);
					break;
				case NativeControlComponentType.ProgressBar:
				case NativeControlComponentType.ProgressBarDark:
					builder.WriteEquality(name, nameof(NSButton.ControlSize), NSControlSize.Regular);
					break;
			}

			return builder;
		}
	}
}
