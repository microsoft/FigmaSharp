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
using System.Linq;

using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace FigmaSharp.NativeControls.Cocoa
{
	public abstract partial class CocoaConverter : FigmaViewConverter
	{
		public override bool ScanChildren(FigmaNode currentNode)
		{
			return false;
		}


		protected abstract IView OnConvertToView(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService);

		public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
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


		protected abstract StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService);

		public override string ConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var builder = OnConvertToCode(currentNode, parentNode, rendererService);
			if (builder != null)
			{
				currentNode.Node.TryGetNativeControlType(out var nativeControlType);

				if (!currentNode.Node.visible)
					builder.WriteEquality(currentNode.Name, nameof(NSView.Hidden), true);

				builder.WriteEquality(currentNode.Name, nameof(NSView.TranslatesAutoresizingMaskIntoConstraints), false);

				if (currentNode.Node.IsA11Enabled ()) {
					bool hasAccessibility = false;

					if (CanSetAccessibilityRole && currentNode.Node.IsA11Group()) {
						var fullRoleName = $"{typeof(AppKit.NSAccessibilityRoles).FullName}.{nameof(AppKit.NSAccessibilityRoles.GroupRole)}";
						builder.WriteEquality(currentNode.Name, nameof(AppKit.NSView.AccessibilityRole), fullRoleName);

                        hasAccessibility = true;
					}

					if (CanSetAccessibilityLabel && currentNode.Node.TrySearchA11Label(out var label))
                    {
						label = NativeControlHelper.GetTranslatableString(label, rendererService.CurrentRendererOptions.TranslateLabels);
						builder.WriteEquality(currentNode.Name, GetAccessibilityTitle(nativeControlType), label, inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);

                        hasAccessibility = true;
					}

					if (CanSetAccessibilityHelp && currentNode.Node.TrySearchA11Help(out var help))
					{
						help = NativeControlHelper.GetTranslatableString(help, rendererService.CurrentRendererOptions.TranslateLabels);
						builder.WriteEquality(currentNode.Name, nameof(AppKit.NSView.AccessibilityHelp), help, inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);

                        hasAccessibility = true;
					}

					if (hasAccessibility)
						builder.AppendLine();
				}

				return builder.ToString();
			}
			return string.Empty;
		}



		protected NSControlSize GetNSControlSize(NativeControlVariant controlVariant)
        {
			if (controlVariant == NativeControlVariant.Small)
				return NSControlSize.Small;

			return NSControlSize.Regular;
        }


		protected string GetNSFontSizeName(NativeControlVariant controlVariant)
		{
			if (controlVariant == NativeControlVariant.Small)
				return $"{ typeof(NSFont) }.{ nameof(NSFont.SmallSystemFontSize) }";

			return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontSize) }";
		}

		protected nfloat GetNSFontSize(NativeControlVariant controlVariant)
		{
			if (controlVariant == NativeControlVariant.Small)
				return NSFont.SmallSystemFontSize;

			return NSFont.SystemFontSize;
		}


		protected NSFont GetNSFont(NativeControlVariant controlVariant)
		{
			if (controlVariant == NativeControlVariant.Small)
				return NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize);

			return NSFont.SystemFontOfSize(NSFont.SystemFontSize);
		}

		protected NSFont GetNSFont(NativeControlVariant controlVariant, FigmaText text)
		{
			var fontWeight = GetNSFontWeight(text);

			if (controlVariant == NativeControlVariant.Regular)
			{
				// The system default Medium is slightly different, so let Cocoa handle that
				if (fontWeight == NSFontWeight.Medium)
					return NSFont.SystemFontOfSize(NSFont.SystemFontSize);
			}

			if (controlVariant == NativeControlVariant.Small)
			{
				// The system default Medium is slightly different, so let Cocoa handle that
				if (fontWeight == NSFontWeight.Medium)
					return NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize);
				else
					return NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize, fontWeight);
			}

			return NSFont.SystemFontOfSize(GetNSFontSize(controlVariant), fontWeight);
		}

		protected string GetNSFontName(NativeControlVariant controlVariant)
		{
			if (controlVariant == NativeControlVariant.Small)
				return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ GetNSFontSizeName(controlVariant) })";

			return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ GetNSFontSizeName(controlVariant) })";
		}

		protected string GetNSFontName(NativeControlVariant controlVariant, FigmaText text, bool withWeight = true)
		{
			var fontWeight = GetNSFontWeight(text);

			if (controlVariant == NativeControlVariant.Regular)
			{
				// The system default Medium is slightly different, so let Cocoa handle that
				if (fontWeight == NSFontWeight.Medium || !withWeight)
					return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ GetNSFontSizeName(controlVariant) })";
			}

			if (controlVariant == NativeControlVariant.Small)
			{
				// The system default Medium is slightly different, so let Cocoa handle that
				if (fontWeight == NSFontWeight.Medium || !withWeight)
				    return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ GetNSFontSizeName(controlVariant) })";
				else
					return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ GetNSFontSizeName(controlVariant) }, { GetNSFontWeightName(text) })";
			}

            if (withWeight)
			    return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ GetNSFontSizeName(controlVariant) }, { GetNSFontWeightName(text) })";
            else
				return $"{ typeof(NSFont) }.{ nameof(NSFont.SystemFontOfSize) }({ GetNSFontSizeName(controlVariant) })";

		}


		public nfloat GetNSFontWeight(FigmaText text)
		{
			string fontName = text?.style?.fontPostScriptName;

			if (fontName != null)
			{
				if (fontName.EndsWith("-Black"))
					return NSFontWeight.Black;

				if (fontName.EndsWith("-Heavy"))
					return NSFontWeight.Heavy;

				if (fontName.EndsWith("-Bold"))
					return NSFontWeight.Bold;

				if (fontName.EndsWith("-Semibold"))
					return NSFontWeight.Semibold;

				if (fontName.EndsWith("-Regular"))
					return NSFontWeight.Regular;

				if (fontName.EndsWith("-Light"))
					return NSFontWeight.Light;

				if (fontName.EndsWith("-Thin"))
					return NSFontWeight.Thin;

				if (fontName.EndsWith("-Ultralight"))
					return NSFontWeight.UltraLight;
			}

			// The default macOS font is of medium weight
			return NSFontWeight.Medium;
		}

		protected string GetNSFontWeightName(FigmaText text)
		{
            // The default macOS font is of medium weight
            string weight = nameof(NSFontWeight.Medium);
			string fontName = text?.style?.fontPostScriptName;

			if (fontName != null)
			{
				if (fontName.EndsWith("-Black"))
					weight = nameof(NSFontWeight.Black);

				if (fontName.EndsWith("-Heavy"))
					weight = nameof(NSFontWeight.Heavy);

				if (fontName.EndsWith("-Bold"))
					weight = nameof(NSFontWeight.Bold);

				if (fontName.EndsWith("-Semibold"))
					weight = nameof(NSFontWeight.Semibold);

				if (fontName.EndsWith("-Regular"))
					weight = nameof(NSFontWeight.Regular);

				if (fontName.EndsWith("-Light"))
					weight = nameof(NSFontWeight.Light);

				if (fontName.EndsWith("-Thin"))
					weight = nameof(NSFontWeight.Thin);

				if (fontName.EndsWith("-Ultralight"))
					weight = nameof(NSFontWeight.UltraLight);
			}

			return $"{ typeof(NSFontWeight) }.{ weight }";
		}


		protected NSTextAlignment GetNSTextAlignment(FigmaText text)
		{
			FigmaTypeStyle style = text.style;

			if (style.textAlignHorizontal == "RIGHT")
				return NSTextAlignment.Right;

			if (style.textAlignHorizontal == "CENTER")
				return NSTextAlignment.Center;

			return NSTextAlignment.Left;
		}

		public string GetNSTextAlignmentName(FigmaText text)
		{
			return $"{nameof(NSTextAlignment)}.{GetNSTextAlignment(text)}";
		}


		public NSColor GetNSColor(string colorStyleName)
		{
			return themeColors.FirstOrDefault(c => c.styleName == colorStyleName).themeColor;
		}

		public string GetNSColorName(string colorStyleName)
		{
			return $"{nameof(NSColor)}.{themeColors.FirstOrDefault(c => c.styleName == colorStyleName).themeColorName}";
		}


		// Not all controls in Cocoa can set every a11y property,
		// this lets converters indicate which properties they can provide
		public virtual bool CanSetAccessibilityLabel => true;
		public virtual bool CanSetAccessibilityHelp => true;
		public virtual bool CanSetAccessibilityRole => true;

		string GetAccessibilityTitle(NativeControlType nativeControlType)
		{
			switch (nativeControlType)
			{
				case NativeControlType.Button:
				case NativeControlType.CheckBox:
				case NativeControlType.Radio:
				case NativeControlType.PopUpButton:
				case NativeControlType.ComboBox:
					return nameof(AppKit.NSView.AccessibilityTitle);
				default:
					break;
			}

			return nameof(AppKit.NSView.AccessibilityLabel);
		}
	}
}
