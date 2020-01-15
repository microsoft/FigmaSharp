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
using AppKit;
using FigmaSharp.NativeControls.Base;
using System;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using System.Linq;
using FigmaSharp.Views;
using FigmaSharp.Services;
using FigmaSharp.Views.Cocoa;
using System.Text;

namespace FigmaSharp.NativeControls.Cocoa
{
    public class RadioConverter : RadioConverterBase
    {
        public override bool ScanChildren(FigmaNode currentNode)
        {
            return false;
        }

        public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var figmaInstance = (FigmaInstance)currentNode;

			var button = new RadioBox() { Text = "" };
			var view = (NSButton)button.NativeObject;

            var controlType = figmaInstance.ToControlType();
            switch (controlType)
            {
                case NativeControlType.ButtonLarge:
                case NativeControlType.ButtonLargeDark:
                    view.ControlSize = NSControlSize.Regular;
                    break;
                case NativeControlType.ButtonStandard:
                case NativeControlType.ButtonStandardDark:
                    view.ControlSize = NSControlSize.Regular;
                    break;
                case NativeControlType.ButtonSmall:
                case NativeControlType.ButtonSmallDark:
                    view.ControlSize = NSControlSize.Small;
                    break;
            }

            var label = figmaInstance.children
                  .OfType<FigmaText>()
                  .FirstOrDefault();

            if (label != null) {
				button.Text = label.characters;
                view.Font = label.style.ToNSFont();
            }

            //first figma 
            var group = figmaInstance.children
                .OfType<FigmaGroup>()
                .FirstOrDefault();

            if (group != null)
            {
                if (group.name == "Disabled")
                {
					button.Enabled = false;
                }
            }

            if (controlType.ToString().EndsWith("Dark", StringComparison.Ordinal))
            {
                view.Appearance = NSAppearance.GetAppearance(NSAppearance.NameDarkAqua);
            }

            return new View(view);
        }

        public override string ConvertToCode(FigmaNode currentNode, FigmaCodeRendererService rendererService)
        {
            var figmaInstance = (FigmaInstance)currentNode;

            var builder = new StringBuilder ();
            var name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;
            builder.AppendLine ($"var {name} = new {typeof (NSButton).FullName}();");

            builder.AppendLine (string.Format ("{0}.BezelStyle = {1};", name, NSBezelStyle.Rounded.GetFullName ()));
            builder.AppendLine (string.Format ("{0}.SetButtonType ({1});", name, NSButtonType.Radio.GetFullName ()));
            builder.Configure (name, currentNode);

            var controlType = figmaInstance.ToControlType ();
            switch (controlType) {
                case NativeControlType.ButtonLarge:
                case NativeControlType.ButtonLargeDark:
                    builder.AppendLine (string.Format ("{0}.ControlSize = {1};", name, NSControlSize.Regular.GetFullName ()));
                    break;
                case NativeControlType.ButtonStandard:
                case NativeControlType.ButtonStandardDark:
                    builder.AppendLine (string.Format ("{0}.ControlSize = {1};", name, NSControlSize.Regular.GetFullName ()));
                    break;
                case NativeControlType.ButtonSmall:
                case NativeControlType.ButtonSmallDark:
                    builder.AppendLine (string.Format ("{0}.ControlSize = {1};", name, NSControlSize.Small.GetFullName ()));
                    break;
            }

			var label = figmaInstance.children.OfType<FigmaText> ().FirstOrDefault ();

            builder.AppendLine (string.Format ("{0}.Title = \"{1}\";", name, label?.characters ?? ""));

            //first figma 
            var group = figmaInstance.children
                .OfType<FigmaGroup> ()
                .FirstOrDefault (s => s.visible);

            if (group != null) {
                if (group.name == "On") {
                    //button.State = value ? NSCellStateValue.On : NSCellStateValue.Off
                    builder.AppendLine (string.Format ("{0}.State = \"{1}\";", name, NSCellStateValue.On.GetFullName ()));
                }

                if (group.name == "Disabled") {
                    builder.AppendLine (string.Format ("{0}.Enabled = {1};", name, false));
                }
            }

            if (controlType.ToString ().EndsWith ("Dark", StringComparison.Ordinal)) {
                builder.AppendLine (string.Format ("{0}.Appearance = NSAppearance.GetAppearance ({1});", name, NSAppearance.NameDarkAqua.GetType ().FullName));
            }

            //if (currentNode is IFigmaDocumentContainer instance) {
            //    var figmaText = instance.children.OfType<FigmaText> ().FirstOrDefault ();
            //    if (figmaText != null) {
            //        builder.AppendLine (string.Format ("{0}.AlphaValue = {1};", name, figmaText.opacity.ToDesignerString ()));
            //        builder.AppendLine (string.Format ("{0}.Title = \"{1}\";", name, figmaText.characters));
            //        //button.Font = figmaText.style.ToNSFont();
            //    }
            //}
            return builder.ToString ();
        }
    }
}
