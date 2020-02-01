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
using FigmaSharp.Views;
using FigmaSharp.Services;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
    public class ButtonConverter : ButtonConverterBase
    {
		public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var figmaInstance = (FigmaInstance)currentNode;

			var button = new Button();
			var view = (NSButton)button.NativeObject;
			view.Title = "";
            view.BezelStyle = NSBezelStyle.Rounded;

			button.Size = new Size(figmaInstance.absoluteBoundingBox.Width, 30);

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

                if (label != null) {
					button.Text = label.characters;
                    view.Font = label.style.ToNSFont();
                }

                if (group.name == "Disabled") {
					button.Enabled = false;
                }
            } else
			{
				var label = figmaInstance.children
				   .OfType<FigmaText>()
				   .FirstOrDefault();

				if (label != null)
				{
					button.Text = label.characters;
					view.Font = label.style.ToNSFont();
				}
			}

            //if (controlType.ToString().EndsWith("Dark", StringComparison.Ordinal))
            //{
            //    view.Appearance = NSAppearance.GetAppearance(NSAppearance.NameDarkAqua);
            //}
            return button;
        }

        public override string ConvertToCode(FigmaNode currentNode, FigmaCodeRendererService rendererService)
        {
            var builder = new StringBuilder();

            var figmaInstance = (FigmaInstance)currentNode;
            var name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

            if (rendererService.NeedsRenderInstance (currentNode))
                builder.AppendLine($"var {FigmaSharp.Resources.Ids.Conversion.NameIdentifier} = new {typeof(NSButton).FullName}();");

            builder.AppendLine(string.Format("{0}.BezelStyle = {1};", FigmaSharp.Resources.Ids.Conversion.NameIdentifier, NSBezelStyle.Rounded.GetFullName ()));
            builder.Configure(FigmaSharp.Resources.Ids.Conversion.NameIdentifier, currentNode);

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

            string title = null;
			if (currentNode is IFigmaDocumentContainer instance) {
				var figmaText = instance.children.OfType<FigmaText> ().FirstOrDefault ();
				if (figmaText != null) {

                    title = figmaText.characters;
                    builder.AppendLine (string.Format ("{0}.AlphaValue = {1};", FigmaSharp.Resources.Ids.Conversion.NameIdentifier, figmaText.opacity.ToDesignerString ()));
				
					//button.Font = figmaText.style.ToNSFont();
				}
			}

            builder.AppendLine (string.Format ("{0}.Title = \"{1}\";", FigmaSharp.Resources.Ids.Conversion.NameIdentifier, title ?? string.Empty));

            return builder.ToString();
        }
    }
}
