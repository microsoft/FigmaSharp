/* 
 * CustomButtonConverter.cs 
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
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace FigmaSharp.NativeControls.Cocoa
{
	public abstract class FigmaNativeControlConverter : FigmaViewConverter
	{
		public virtual Type ControlType { get; }

		public override bool ScanChildren(FigmaNode currentNode)
		{
			return false;
		}

		public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			var converted = OnConvertToView(currentNode, parent, rendererService);
			if (converted != null)
			{
				var nativeView = converted.NativeObject as AppKit.NSView;
				if (currentNode.IsA11Group())
					nativeView.AccessibilityRole = AppKit.NSAccessibilityRoles.GroupRole;

				//label
				if (currentNode.TrySearchA11Label(out var label))
				{
					try
					{
						nativeView.AccessibilityTitle = label;
					}
					catch (Exception)
					{
						nativeView.AccessibilityLabel = label;
					}
				}
				//help
				if (currentNode.TrySearchA11Help(out var help))
					nativeView.AccessibilityHelp = help;
			}
			return converted;
		}

		protected abstract IView OnConvertToView(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService);

		string GetAccessibilityTitle(NativeControlType nativeControlType)
		{
			switch (nativeControlType)
			{
				case NativeControls.NativeControlType.Button:
				case NativeControls.NativeControlType.CheckBox:
				case NativeControls.NativeControlType.RadioButton:
					return nameof(AppKit.NSView.AccessibilityTitle);
				default:
					break;
			}
			return nameof(AppKit.NSView.AccessibilityLabel);
		}

		public override string ConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var builder = OnConvertToCode(currentNode, parentNode, rendererService);
			if (builder != null)
			{
				currentNode.Node.TryGetNativeControlType(out var nativeControlType);

				bool hasAccessibility = false;
				if (currentNode.Node.IsA11Group())
				{
					var fullRoleName = $"{typeof(AppKit.NSAccessibilityRoles).FullName}.{nameof(AppKit.NSAccessibilityRoles.GroupRole)}";

					builder.WriteEquality(currentNode.Name, nameof(AppKit.NSView.AccessibilityRole), fullRoleName);
					hasAccessibility = true;
				}
				if (currentNode.Node.TrySearchA11Label(out var label))
				{
					label = NativeControlHelper.GetTranslatableString(label, rendererService.CurrentRendererOptions.TranslateLabels);

					builder.WriteEquality(currentNode.Name, GetAccessibilityTitle(nativeControlType), label, inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);
					hasAccessibility = true;
				}
				if (currentNode.Node.TrySearchA11Help(out var help))
				{
					help = NativeControlHelper.GetTranslatableString(help, rendererService.CurrentRendererOptions.TranslateLabels);

					builder.WriteEquality(currentNode.Name, nameof(AppKit.NSView.AccessibilityHelp), help, inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);
					hasAccessibility = true;
				}

				if (hasAccessibility)
					builder.AppendLine();

				return builder.ToString();
			}
			return string.Empty;
		}

		protected abstract StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService);

	}
}
