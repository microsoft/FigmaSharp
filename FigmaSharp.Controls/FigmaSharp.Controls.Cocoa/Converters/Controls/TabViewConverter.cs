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
using System.Collections.Generic;
using System.Text;

using AppKit;

using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Cocoa;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Controls.Cocoa.Converters
{
    public class TabViewConverter : CocoaConverter
    {
        internal override bool HasHeightConstraint() => true;

        public override Type GetControlType(FigmaNode currentNode) => typeof(NSTabView);

        public override bool CanConvert(FigmaNode currentNode)
        {
            return currentNode.TryGetNativeControlType(out var controlType) &&
                controlType == FigmaControlType.TabView;
        }


        protected override IView OnConvertToView (FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
        {
            var frame = (FigmaFrame)currentNode;
            var tabView = new NSTabView();
           
            List<NSTabViewItem> items = new List<NSTabViewItem>();
            var tabNodes = frame.FirstChild (s => s.name == ComponentString.ITEMS);

            if (tabNodes == null)
                return new View(tabView);

            foreach (FigmaNode tabNode in tabNodes.GetChildren (t => t.visible, reverseChildren: true))
            {
                var firstChild = tabNode.FirstChild(s => s.name.In(ComponentString.STATE_REGULAR, ComponentString.STATE_SELECTED) && s.visible);

                if (firstChild != null)
                {
                    FigmaText text = firstChild.FirstChild (s => s.name == ComponentString.TITLE) as FigmaText;

                    if (text != null)
                        items.Add(new NSTabViewItem() { Label = rendererService.GetTranslatedText (text.characters ?? string.Empty) });
                }
            }

            tabView.SetItems(items.ToArray());

			return new View(tabView);
        }

        protected override StringBuilder OnConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            var code = new StringBuilder();
            string name = FigmaSharp.Resources.Ids.Conversion.NameIdentifier;

            var frame = (FigmaFrame)currentNode.Node;
            currentNode.Node.TryGetNativeControlType(out FigmaControlType controlType);
            currentNode.Node.TryGetNativeControlVariant(out NativeControlVariant controlVariant);

            if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
                code.WriteConstructor(name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

            var itemNodes = frame.FirstChild(s => s.name == ComponentString.ITEMS);

            if (itemNodes == null)
                return null;

            code.AppendLine();
            code.AppendLine($"{ name }.{ nameof(NSTabView.SetItems) }(");
            code.AppendLine($"\tnew { typeof(NSTabViewItem[]) }");
            code.AppendLine("\t{");

            foreach (FigmaNode tabNode in itemNodes.GetChildren(t => t.visible, reverseChildren: true))
            {
                var firstChild = tabNode.FirstChild(s => s.name.In(ComponentString.STATE_REGULAR, ComponentString.STATE_SELECTED) && s.visible);

                if (firstChild != null)
                {
                    FigmaText text = firstChild.FirstChild(s => s.name == ComponentString.TITLE) as FigmaText;

                    if (text != null)
                        code.AppendLine($"\t\tnew {typeof(NSTabViewItem)}() {{ {nameof(NSTabViewItem.Label)} = \"{text.characters}\" }},");
                }
            }

            code.AppendLine("\t}");
            code.AppendLine(");");

            return code;
        }
    }
}
