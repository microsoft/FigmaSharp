﻿/* 
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

namespace FigmaSharp.NativeControls.Cocoa
{
    public class CheckConverter : CheckConverterBase
    {
        public override bool ScanChildren(FigmaNode currentNode)
        {
            return false;
        }

        public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var figmaInstance = (FigmaInstance)currentNode;

			var button = new CheckBox() { Text = "" };
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
                .FirstOrDefault(s => s.visible);

            if (group != null) {
                if (group.name == "On") {
					button.IsChecked = true;
                }

				if (group.name == "Disabled") {
					button.Enabled = false;
				}
			}

            if (controlType.ToString().EndsWith("Dark", StringComparison.Ordinal)) {
                view.Appearance = NSAppearance.GetAppearance(NSAppearance.NameDarkAqua);
            }

            return new View(view);
        }

        public override string ConvertToCode(FigmaNode currentNode, FigmaCodeRendererService rendererService)
        {
            return string.Empty;
        }
    }
}
