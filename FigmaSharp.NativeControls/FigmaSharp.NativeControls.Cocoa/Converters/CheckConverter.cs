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
using FigmaSharp.NativeControls;

namespace FigmaSharp.NativeControls.Cocoa
{
    public class CheckConverter : FigmaNativeControlConverter
    {
        public override Type GetControlType(FigmaNode currentNode)
        {
            return typeof(NSButton);
        }

        public override bool CanConvert(FigmaNode currentNode)
        {
            return currentNode.TryGetNativeControlType(out var value) && value == NativeControlType.CheckBox;
        }

        protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var figmaInstance = (FigmaFrame)currentNode;

			var button = new CheckBox() { Text = string.Empty };
			var view = (NSButton)button.NativeObject;
            view.Configure (figmaInstance);

            figmaInstance.TryGetNativeControlComponentType (out var controlType);

            switch (controlType)
            {
                case NativeControlComponentType.CheckboxSmall:
                case NativeControlComponentType.CheckboxSmallDark:
                    view.ControlSize = NSControlSize.Mini;
                    break;
                case NativeControlComponentType.CheckboxStandard:
                case NativeControlComponentType.CheckboxStandardDark:
                    view.ControlSize = NSControlSize.Regular;
                    view.Font = NSFont.SystemFontOfSize(NSFont.SystemFontSize);
                    break;
            }

            var label = figmaInstance.children
                  .OfType<FigmaText>()
                  .FirstOrDefault(s => s.visible);

            if (label != null) {
				button.Text = label.characters;
                //view.Font = label.style.ToNSFont();
            }

            //check with labels needs check in child
            var checkButtonFigmaNode = figmaInstance.children
				.FirstOrDefault(s => s.TryGetNativeControlType(out var value) && value == NativeControlType.CheckBox)
				as FigmaFrame;

            button.IsChecked = figmaInstance.children
                .OfType<FigmaGroup> ()
                .Any (s => s.name == "On" && s.visible);

            button.Enabled = !figmaInstance.children
                .OfType<FigmaGroup> ()
                .Any (s => s.name == "Disabled" && s.visible);

            return new View(view);
        }

        protected override StringBuilder OnConvertToCode (FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            var figmaInstance = (FigmaFrame)currentNode.Node;

            var builder = new StringBuilder ();
            var name = Resources.Ids.Conversion.NameIdentifier;

            if (rendererService.NeedsRenderConstructor (currentNode, parentNode))
                builder.WriteConstructor (name, GetControlType(currentNode.Node), rendererService.NodeRendersVar (currentNode, parentNode));

            builder.Configure (currentNode.Node, name);

            builder.WriteEquality (name, nameof (NSButton.BezelStyle), NSBezelStyle.Rounded);
            builder.WriteMethod (name, nameof (NSButton.SetButtonType), NSButtonType.Switch);

            figmaInstance.TryGetNativeControlComponentType (out var controlType);
            switch (controlType) {
                case NativeControlComponentType.CheckboxSmall:
                case NativeControlComponentType.CheckboxSmallDark:
                    builder.WriteEquality (name, nameof (NSButton.ControlSize), NSControlSize.Mini);
                    break;
                case NativeControlComponentType.CheckboxStandard:
                case NativeControlComponentType.CheckboxStandardDark:
                    builder.WriteEquality (name, nameof (NSButton.ControlSize), NSControlSize.Regular);
                    builder.WriteEquality (currentNode.Name, nameof (NSButton.Font),
                        CodeGenerationHelpers.Font.SystemFontOfSize (CodeGenerationHelpers.Font.SystemFontSize));
                    break;
            }

            var label = figmaInstance.children
                .OfType<FigmaText> ()
                .FirstOrDefault ();

            if (label != null) {
                var labelTranslated = NativeControlHelper.GetTranslatableString(label.characters, rendererService.CurrentRendererOptions.TranslateLabels);

                builder.WriteEquality(name, nameof(NSButton.Title),
                    labelTranslated,
                    inQuotes: !rendererService.CurrentRendererOptions.TranslateLabels);
            }
            //check with labels needs check in child

            var checkButtonFigmaNode = figmaInstance.children
                .FirstOrDefault(s => s.TryGetNativeControlType(out var value) && value == NativeControlType.CheckBox)
                as FigmaFrame;

            if (checkButtonFigmaNode != null) {
                figmaInstance = checkButtonFigmaNode;
            }

            //first figma 
            var group = figmaInstance.children
                .OfType<FigmaGroup> ()
                .FirstOrDefault (s => (s.name == "On" || s.name == "Off") && s.visible);

            if (group != null) {
                if (group.name == "On") {
                    builder.WriteEquality (name, nameof (NSButton.State), NSCellStateValue.On);
                }

                if (group.name == "Off")
                {
                    builder.WriteEquality(name, nameof(NSButton.State), NSCellStateValue.Off);
                }

                // TODO: Fix this in the component first
                // if (group.name == "Disabled") {
                //    builder.WriteEquality (name, nameof (NSButton.Enabled), false);
                // }
            }

            //if (controlType.ToString ().EndsWith ("Dark", StringComparison.Ordinal)) {
            //    builder.AppendLine (string.Format ("{0}.Appearance = NSAppearance.GetAppearance ({1});", name, NSAppearance.NameDarkAqua.GetType ().FullName));
            //}

            //if (currentNode is IFigmaDocumentContainer instance) {
            //    var figmaText = instance.children.OfType<FigmaText> ().FirstOrDefault ();
            //    if (figmaText != null) {
            //        builder.AppendLine (string.Format ("{0}.AlphaValue = {1};", name, figmaText.opacity.ToDesignerString ()));
            //        builder.AppendLine (string.Format ("{0}.Title = \"{1}\";", name, figmaText.characters));
            //        //button.Font = figmaText.style.ToNSFont();
            //    }
            //}
            return builder;

        }
    }
}
