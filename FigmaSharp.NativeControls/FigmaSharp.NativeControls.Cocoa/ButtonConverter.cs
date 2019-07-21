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
using AppKit;
using FigmaSharp.NativeControls.Base;
using System.Linq;
using System.Text;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using System;
using LiteForms;
using LiteForms.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
    public class ButtonConverter : ButtonConverterBase
    {
		public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
            var figmaInstance = (FigmaInstance)currentNode;

            var view = new NSButton();
			view.Title = "";
            view.BezelStyle = NSBezelStyle.Rounded;

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

            //first figma 
            var group = figmaInstance.children
                .OfType<FigmaGroup>()
                .FirstOrDefault(s => s.visible);

            if (group != null)
            {
                var label = group.children
                    .OfType<FigmaText>()
                    .FirstOrDefault();

                if (label != null)
                {
                    view.Title = label.characters;
                    view.Font = label.style.ToNSFont();
                }

                if (group.name == "Disabled")
                {
                    view.Enabled = false;
                }
            }

            if (controlType.ToString().EndsWith("Dark", StringComparison.Ordinal))
            {
                view.Appearance = NSAppearance.GetAppearance(NSAppearance.NameDarkAqua);
            }
            return new View(view);
        }

        public override string ConvertToCode(FigmaNode currentNode)
        {
            var builder = new StringBuilder();
            var name = "[NAME]";
            builder.AppendLine($"var {name} = new {nameof(NSButton)}();");
            builder.AppendLine(string.Format("{0}.BezelStyle = {1};", name, NSBezelStyle.Rounded.ToString ()));
            builder.Configure(name, currentNode);

			if (currentNode is IFigmaDocumentContainer instance) {
				var figmaText = instance.children.OfType<FigmaText> ().FirstOrDefault ();
				if (figmaText != null) {
					builder.AppendLine (string.Format ("{0}.AlphaValue = {1};", name, figmaText.opacity.ToDesignerString ()));
					builder.AppendLine (string.Format ("{0}.Title = \"{1}\";", name, figmaText.characters));
					//button.Font = figmaText.style.ToNSFont();
				}
			}
            return builder.ToString();
        }
    }
}
