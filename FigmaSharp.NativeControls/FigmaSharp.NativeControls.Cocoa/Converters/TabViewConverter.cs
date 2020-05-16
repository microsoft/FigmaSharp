﻿// Authors:
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
using System.Collections.Generic;
using System.Text;

using AppKit;

using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.NativeControls.Cocoa
{
    public class TabViewConverter : CocoaConverter
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSTabView);

        public override bool CanConvert(FigmaNode currentNode)
        {
            return currentNode.TryGetNativeControlType(out var controlType) &&
                controlType == NativeControlType.TabView;
        }


        protected override IView OnConvertToView (FigmaNode currentNode, ProcessedNode parentNode, FigmaRendererService rendererService)
        {
            var frame = (FigmaFrame)currentNode;
            var tabView = new NSTabView();
           
            List<NSTabViewItem> tabs = new List<NSTabViewItem>();
            var tabNodes = frame.FirstChild (s => s.name == "Buttons");

            if (tabNodes == null)
                return new View(tabView);

            foreach (FigmaNode tabNode in tabNodes.GetChildren (t => t.visible, reverseChildren: true))
            {
                var firstChild = tabNode.FirstChild(s => s.name.In("Basic", "Default") && s.visible);

                if (firstChild != null)
                {
                    FigmaText text = firstChild.FirstChild (s => s.name == "lbl") as FigmaText;

                    if (text != null)
                    {
                        var item = new NSTabViewItem() {
                            Label = text.characters
                        };

                        tabs.Add(item);
                    }
                }
            }

            tabView.SetItems(tabs.ToArray());

			return new View(tabView);
        }

        protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
        {
            var code = new StringBuilder();

            // TODO output:

            // var tabView = new NSTabView();
            // tabView.SetItems(new NSTabViewItem[] {
            //     new NSTabViewItem() { Label = "label1" },
            //     new NSTabViewItem() { Label = "label2" },
            //     new NSTabViewItem() { Label = "label3" }
            // });

            return code;
        }
    }
}
