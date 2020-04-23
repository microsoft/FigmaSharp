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
using System.Collections.Generic;
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
    public class TabViewConverter : FigmaNativeControlConverter
    {
        public override Type ControlType => typeof(NSTabView);

        public override bool CanConvert(FigmaNode currentNode)
        {
            return currentNode.TryGetNativeControlType(out var value) && value == NativeControlType.TabView;
        }

        protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var figmaInstance = (FigmaFrameEntity)currentNode;

            var view = new TabView();
            var tabView = (NSTabView)view.NativeObject;

            List<NSTabViewItem> tabs = new List<NSTabViewItem>();
            var tabNodes = figmaInstance.children.FirstOrDefault(s => s.name == "tabs");

            foreach (FigmaNode tabNode in tabNodes.GetChildren().Reverse())
            {
                if (!tabNode.visible)
                    continue;

                var figmaText = (FigmaText)tabNode.GetChildren()
                    .FirstOrDefault(s => (s.name == "Basic" && s.visible) || (s.name == "Default" && s.visible))
                    .FindNode(s => (s.name == "lbl"))
                    .FirstOrDefault();

                var item = new NSTabViewItem();
                item.Label = figmaText.characters;
                tabs.Add(item);
            }

            tabView.SetItems(tabs.ToArray());

			return view;
        }

        protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            var builder = new StringBuilder();

            // TODO output:

            // var tabView = new NSTabView();
            // tabView.SetItems(new NSTabViewItem[] {
            //     new NSTabViewItem() { Label = "label1" },
            //     new NSTabViewItem() { Label = "label2" },
            //     new NSTabViewItem() { Label = "label3" }
            // });

            return builder;
        }
    }
}
