﻿/* 
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
	public class ButtonConverter : FigmaNativeControlConverter
    {
		public override bool CanConvert(FigmaNode currentNode)
        {
            return currentNode.TryGetNativeControlType(out var value) && value == NativeControlType.Button;
        }

        protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var figmaInstance = (FigmaFrameEntity)currentNode;

            var button = new Button();
            var view = (NSButton)button.NativeObject;
            view.Title = "";

            bool writesTitle = true;

            figmaInstance.TryGetNativeControlComponentType(out var controlType);

            if (controlType == NativeControlComponentType.ButtonHelp || controlType == NativeControlComponentType.ButtonHelpDark)
            {
                writesTitle = false;
                view.BezelStyle = NSBezelStyle.HelpButton;
            }
            else
            {
                view.BezelStyle = NSBezelStyle.Rounded;
            }

            view.Configure(figmaInstance);

            switch (controlType)
            {
                case NativeControlComponentType.ButtonLarge:
                case NativeControlComponentType.ButtonLargeDark:
                    view.ControlSize = NSControlSize.Regular;
                    view.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
                    break;
                case NativeControlComponentType.ButtonStandard:
                case NativeControlComponentType.ButtonStandardDark:
                    view.ControlSize = NSControlSize.Regular;
                    view.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
                    break;
                case NativeControlComponentType.ButtonSmall:
                case NativeControlComponentType.ButtonSmallDark:
                    view.ControlSize = NSControlSize.Small;
                    view.Font = NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize);
                    break;
            }

            var group = figmaInstance.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.visible);

            if (group != null)
            {
                var label = group.children
                    .OfType<FigmaText>()
                    .FirstOrDefault();

                if (writesTitle && label != null) {
                    button.Text = label.characters;
                    //view.Font = label.style.ToNSFont();
                } else {
                    button.Text = string.Empty;
                }

                if (group.name == "Disabled")
                {
                    button.Enabled = false;
                }
                else if (group.name == "Default")
                {
                    view.KeyEquivalent = "\r";
                }
            }
            else
            {
                var label = figmaInstance.children
                   .OfType<FigmaText>()
                   .FirstOrDefault();

                if (writesTitle && label != null)
                {
                    button.Text = label.characters;
                    //view.Font = label.style.ToNSFont();
                }
                else
                {
                    button.Text = string.Empty;
                }
            }

            return button;
        }

        protected override StringBuilder OnConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            var builder = new StringBuilder();

            var figmaInstance = (FigmaFrameEntity)currentNode.Node;
            var name = Resources.Ids.Conversion.NameIdentifier;

            if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
                builder.WriteConstructor (name, GetControlType(currentNode.Node).FullName, rendererService.NodeRendersVar (currentNode, parentNode));

            builder.Configure (currentNode.Node, name);

            figmaInstance.TryGetNativeControlComponentType(out var controlType);

            bool writesTitle = true;
            if (controlType == NativeControlComponentType.ButtonHelp || controlType == NativeControlComponentType.ButtonHelpDark)
            {
                writesTitle = false;
                builder.WriteEquality(name, nameof(NSButton.BezelStyle), NSBezelStyle.HelpButton);
            }
            else
                builder.WriteEquality(name, nameof(NSButton.BezelStyle), NSBezelStyle.Rounded);
           
            switch (controlType) {
                case NativeControlComponentType.ButtonLarge:
                case NativeControlComponentType.ButtonLargeDark:
                    builder.WriteEquality (name, nameof (NSButton.ControlSize), NSControlSize.Regular);
                    builder.WriteEquality(name, nameof(NSButton.Font), CodeGenerationHelpers.Font.SystemFontOfSize(CodeGenerationHelpers.Font.SystemFontSize));
                    break;
                case NativeControlComponentType.ButtonStandard:
                case NativeControlComponentType.ButtonStandardDark:
                    builder.WriteEquality (name, nameof (NSButton.ControlSize), NSControlSize.Regular);
                    builder.WriteEquality(name, nameof(NSButton.Font), CodeGenerationHelpers.Font.SystemFontOfSize(CodeGenerationHelpers.Font.SystemFontSize));
                    break;
                case NativeControlComponentType.ButtonSmall:
                case NativeControlComponentType.ButtonSmallDark:
                    builder.WriteEquality (name, nameof (NSButton.ControlSize), NSControlSize.Small);
                    builder.WriteEquality(name, nameof(NSButton.Font), CodeGenerationHelpers.Font.SystemFontOfSize(CodeGenerationHelpers.Font.SmallSystemFontSize));
                    break;
            }


            //first figma 
            var group = figmaInstance.children
                .OfType<FigmaGroup> ()
                .FirstOrDefault (s => s.visible);

            if (group != null) {
                var label = group.children
                    .OfType<FigmaText> ()
                    .FirstOrDefault ();

                if (writesTitle && label != null) {
                    var labelTranslated = NativeControlHelper.GetTranslatableString(label.characters, rendererService.CurrentRendererOptions.TranslateLabels);
                    builder.WriteEquality (name, nameof (NSButton.Title), labelTranslated, inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);
                } else {
                    builder.WriteEquality(name, nameof(NSButton.Title), string.Empty, inQuotes: true);
                }

                if (group.name == "Disabled") {
                    builder.WriteEquality (name, nameof (NSButton.Enabled), false);
                } else if (group.name == "Default") {
                    builder.WriteEquality (name, nameof (NSButton.KeyEquivalent), "\\r", true);
                }
            } else {
                var label = figmaInstance.children
                   .OfType<FigmaText> ()
                   .FirstOrDefault ();

                if (writesTitle && label != null) {
                    var labelTranslated = NativeControlHelper.GetTranslatableString(label.characters, rendererService.CurrentRendererOptions.TranslateLabels);
                    builder.WriteEquality (name, nameof (NSButton.Title), labelTranslated, inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);
                    //view.Font = label.style.ToNSFont ();
                } else {
                    builder.WriteEquality(name, nameof(NSButton.Title), string.Empty, inQuotes: true);
                }
            }
            return builder;
        }

        public override Type GetControlType(FigmaNode currentNode)
        {
            return typeof(NSButton);
        }
	}
}
