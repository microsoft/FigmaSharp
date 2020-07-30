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
using FigmaSharp.Controls.Cocoa.Helpers;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace FigmaSharp.Controls.Cocoa
{
	public abstract class CocoaConverter : NodeConverter
	{
		public override bool ScanChildren(FigmaNode currentNode)
		{
			return false;
		}

		protected abstract IView OnConvertToView(FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService);

		public override IView ConvertToView (FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
		{
			var converted = OnConvertToView(currentNode, parentNode, rendererService);

			if (converted != null) {
				var nativeView = converted.NativeObject as AppKit.NSView;

				if (!currentNode.visible)
					nativeView.Hidden = true;

				if (currentNode.IsA11Group())
					nativeView.AccessibilityRole = AppKit.NSAccessibilityRoles.GroupRole;

				//label
				if (currentNode.TrySearchA11Label(out var label)) {
					try {
						nativeView.AccessibilityTitle = label;
					} catch (Exception) {
						nativeView.AccessibilityLabel = label;
					}
				}
				//help
				if (currentNode.TrySearchA11Help(out var help))
					nativeView.AccessibilityHelp = help;
			}

			return converted;
		}


		protected abstract StringBuilder OnConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService);

		public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
		{
			var builder = OnConvertToCode(currentNode, parentNode, rendererService);
			if (builder != null)
			{
				currentNode.Node.TryGetNativeControlType(out var nativeControlType);

				if (!currentNode.Node.visible)
					builder.WritePropertyEquality(currentNode.Name, nameof(NSView.Hidden), true);

				builder.WritePropertyEquality(currentNode.Name, nameof(NSView.TranslatesAutoresizingMaskIntoConstraints), false);

				if (currentNode.Node.IsA11Enabled ()) {
					bool hasAccessibility = false;

					if (CanSetAccessibilityRole && currentNode.Node.IsA11Group()) {
						var fullRoleName = $"{typeof(AppKit.NSAccessibilityRoles).FullName}.{nameof(AppKit.NSAccessibilityRoles.GroupRole)}";
						builder.WritePropertyEquality(currentNode.Name, nameof(AppKit.NSView.AccessibilityRole), fullRoleName);

						hasAccessibility = true;
					}

					if (CanSetAccessibilityLabel && currentNode.Node.TrySearchA11Label(out var label))
					{
						builder.WriteTranslatedEquality(currentNode.Name, GetAccessibilityTitle(nativeControlType), label, rendererService);
						hasAccessibility = true;
					}

					if (CanSetAccessibilityHelp && currentNode.Node.TrySearchA11Help(out var help))
					{
						help = rendererService.GetTranslatedText (help);
						builder.WritePropertyEquality(currentNode.Name, nameof(AppKit.NSView.AccessibilityHelp), help, inQuotes: !rendererService.Options.TranslateLabels);

						hasAccessibility = true;
					}

					if (hasAccessibility)
						builder.AppendLine();
				}

				return builder.ToString();
			}
			return string.Empty;
		}


		// Not all controls in Cocoa can set every a11y property,
		// this lets converters indicate which properties they can provide
		public virtual bool CanSetAccessibilityLabel => true;
		public virtual bool CanSetAccessibilityHelp => true;
		public virtual bool CanSetAccessibilityRole => true;

		string GetAccessibilityTitle(FigmaControlType nativeControlType)
		{
			switch (nativeControlType)
			{
				case FigmaControlType.Button:
				case FigmaControlType.CheckBox:
				case FigmaControlType.Radio:
				case FigmaControlType.PopUpButton:
				case FigmaControlType.ComboBox:
					return nameof(AppKit.NSView.AccessibilityTitle);
				default:
					break;
			}

			return nameof(AppKit.NSView.AccessibilityLabel);
		}
	}
}
