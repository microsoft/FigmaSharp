﻿// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
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

using System.Linq;
using System.Text;

using AppKit;
using Foundation;

using FigmaSharp.Models;

namespace FigmaSharp.Cocoa
{
	public static class CodeViewConfigureExtensions
	{
		public static void Configure (this StringBuilder builder, FigmaNode figmaNode, string name)
		{
			builder.WritePropertyEquality (name, nameof (NSView.WantsLayer), true);

			if (!figmaNode.visible) {
				builder.WritePropertyEquality (name, nameof (NSView.Hidden), !figmaNode.visible);
			}

			builder.WritePropertyEquality(name, nameof(NSView.TranslatesAutoresizingMaskIntoConstraints), false);

			//if (drawFrameSize && figmaNode is IAbsoluteBoundingBox container) {
			//	var sizeConstructor = typeof (CGSize).GetConstructor (
			//		container.absoluteBoundingBox.Width.ToDesignerString (),
			//		container.absoluteBoundingBox.Height.ToDesignerString ());

			//	builder.WriteMethod (
			//		name,
			//		nameof (NSView.SetFrameSize),
			//		sizeConstructor
			//		);
			//}
		}

		public static void Configure (this StringBuilder builder, FigmaVector figmaNode, string name)
		{
			Configure (builder, (FigmaNode)figmaNode, name);

			var fills = figmaNode.fills.FirstOrDefault ();
			if (fills != null && fills.visible && fills.color != null) {

				builder.AppendLine (string.Format ("{0}.Layer.BackgroundColor = {1};", name, fills.color.ToDesignerString (true)));
			}

			var strokes = figmaNode.strokes.FirstOrDefault ();
			if (strokes != null && strokes.visible) {
				if (strokes.color != null) {
					builder.AppendLine (string.Format ("{0}.Layer.BorderColor = {1};", name, strokes.color.ToDesignerString (true)));
				}
				builder.AppendLine (string.Format ("{0}.Layer.BorderWidth = {1};", name, figmaNode.strokeWeight));
			}
		}

		public static void Configure (this StringBuilder builder, RectangleVector child, string name)
		{
			Configure (builder, (FigmaVector)child, name);

			builder.AppendLine (string.Format ("{0}.Layer.CornerRadius = {1};", name, child.cornerRadius.ToDesignerString ()));
		}

		public static void Configure (this StringBuilder builder, FigmaText text, string name)
		{
			//Configure(builder, name, (FigmaNode)text);

		
			var propertyAttributedStringValue = $"{name}.{nameof (NSTextField.AttributedStringValue)}";
			var propertyTextColor = $"{name}.{nameof (NSTextField.TextColor)}";
			var propertyFont = $"{name}.{nameof (NSTextField.Font)}";
			var propertyAlignment = $"{name}.{nameof (NSTextField.Alignment)}";
			var propertyAlphaValue = $"{name}.{nameof (NSTextField.AlphaValue)}";

			builder.AppendLine (string.Format ("{0} = {1};", propertyFont, text.style.ToNSFontDesignerString ()));
		
			builder.AppendLine (string.Format ("{0} = {1};", propertyAlphaValue, text.opacity.ToDesignerString ()));

			//label.LineBreakMode = NSLineBreakMode.ByWordWrapping;
			//label.SetContentCompressionResistancePriority(250, NSLayoutConstraintOrientation.Horizontal);

			var fills = text.fills.FirstOrDefault () as FigmaPaint;
			if (fills != null) {
				builder.AppendLine (string.Format ("{0} = {1};", propertyTextColor, fills.color.ToDesignerString ()));
			}

			if (text.characterStyleOverrides != null && text.characterStyleOverrides.Length > 0) {

				var attributedTextName = string.Format ("{0}AttributedText", Resources.Ids.Conversion.NameIdentifier);
				builder.AppendLine (string.Format ("var {0} = new {1} ({2});", attributedTextName, typeof (NSMutableAttributedString).FullName, propertyAttributedStringValue));

				var attributedStringKey = typeof (NSStringAttributeKey).FullName;
				var attributtedStringForegroundColor = $"{attributedStringKey}.{nameof (NSStringAttributeKey.ForegroundColor)}";
				var attributtedStringFont = $"{attributedStringKey}.{nameof (NSStringAttributeKey.Font)}";

				//var attributedText = new NSMutableAttributedString (label.AttributedStringValue);
				for (int i = 0; i < text.characterStyleOverrides.Length; i++) {

					var range = $"new {typeof (NSRange).FullName} ({i}, 1)";

					var key = text.characterStyleOverrides[i].ToString ();
					if (!text.styleOverrideTable.ContainsKey (key)) {
						//we want the default values
						//builder.AppendLine (string.Format ("{0}.AddAttribute(AppKit.NSStringAttributeKey.Font, {1}, new NSRange({2}, 1));", attributedTextName, element.ToNSFontDesignerString (), i));

						builder.AppendLine (string.Format ("{0}.AddAttribute({1}, {2}, {3});", attributedTextName, attributtedStringForegroundColor, propertyTextColor, range));
						builder.AppendLine (string.Format ("{0}.AddAttribute({1}, {2}, {3});", attributedTextName, attributtedStringFont, propertyTextColor, range));
						continue;
					}

					//if there is a style to override
					var styleOverrided = text.styleOverrideTable[key];

					//set the color

					string fontColorOverrided;
					var fillOverrided = styleOverrided.fills?.FirstOrDefault ();
					if (fillOverrided != null && fillOverrided.visible)
						fontColorOverrided = fillOverrided.color.ToDesignerString ();
					else {
						fontColorOverrided = propertyTextColor;
					}

					builder.AppendLine (string.Format ("{0}.AddAttribute({1}, {2}, {3});", attributedTextName, attributtedStringForegroundColor, fontColorOverrided, range));

					//TODO: we can improve this
					//set the font for this character
					string fontOverrided;
					if (styleOverrided?.fontFamily != null) {
						fontOverrided = styleOverrided.ToNSFontDesignerString ();
					} else {
						fontOverrided = propertyFont;
					}

					builder.AppendLine (string.Format ("{0}.AddAttribute ({1}, {2}, {3});", attributedTextName, attributtedStringFont, fontOverrided, range));
				}

				builder.AppendLine (string.Format ("{0} = {1};", propertyAttributedStringValue, attributedTextName));
			}
		}
	}
}
