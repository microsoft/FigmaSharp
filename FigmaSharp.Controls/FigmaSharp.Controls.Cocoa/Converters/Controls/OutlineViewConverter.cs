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

using Foundation;
using AppKit;

using FigmaSharp.Cocoa;
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Controls.Cocoa.Converters
{
    public class OutlineViewConverter : CocoaConverter
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSOutlineView);

        public override bool CanConvert(FigmaNode currentNode)
        {
            return currentNode.TryGetNativeControlType(out var controlType) &&
                controlType == FigmaControlType.OutlineView;
        }


        protected override IView OnConvertToView (FigmaNode currentNode, ViewNode parentNode, ViewRenderService rendererService)
        {
            var frame = (FigmaFrame)currentNode;
            var outlineView = new NSOutlineView();


            var columnNodes = frame.FirstChild (s => s.name == ComponentString.COLUMNS && s.visible);

            NSScrollView scrollView = new NSScrollView();
            scrollView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
            scrollView.BorderType = NSBorderType.BezelBorder;
            scrollView.DrawsBackground = true;
            scrollView.DocumentView = outlineView;

            outlineView.DataSource = new OutlineDataSource();
            outlineView.Delegate   = new OutlineDelegate();

            if (columnNodes == null)
            {
                outlineView.HeaderView = null;
                return new View(scrollView);
            }

            // TODO: Parse options layers
            outlineView.UsesAlternatingRowBackgroundColors = false;
            outlineView.AllowsMultipleSelection = false;
            outlineView.AllowsColumnResizing = true;
            outlineView.AllowsColumnReordering = false;
            outlineView.AllowsEmptySelection = false;

            int columnCount = 1;
            foreach (FigmaNode tableColumNode in columnNodes.GetChildren(t => t.visible))
            {
                FigmaText text = tableColumNode.FirstChild(s => s.name == ComponentString.TITLE) as FigmaText;

                if (text == null)
                    continue;
                
                string title = text.characters;

                NSTableColumn column = new NSTableColumn();

                column.HeaderCell.Alignment = Helpers.ViewHelper.GetNSTextAlignment(text);
                column.Identifier = "Column" + columnCount;
                column.Title = rendererService.GetTranslatedText(text);
                column.Width = (tableColumNode as FigmaFrame).absoluteBoundingBox.Width;

                if (columnCount == 1)
                    outlineView.OutlineTableColumn = column;

                outlineView.AddColumn(column);
                columnCount++;
            }


            var rectangle = (RectangleVector) frame.FirstChild(s => s.name == ComponentString.BACKGROUND && s.visible);

            if (rectangle != null)
            {
                foreach (var styleMap in rectangle.styles)
                {
                    if (rendererService.NodeProvider.TryGetStyle(styleMap.Value, out FigmaStyle style) &&
                        styleMap.Key == "fill")
                    {
                            outlineView.BackgroundColor = ColorService.GetNSColor(style.name);
                    }
                }
            }


            return new View(scrollView);
        }

        class OutlineDelegate : NSOutlineViewDelegate
        {
            public override NSView GetView(NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
            {
                if (tableColumn.Identifier == "Column1")
                {
                    return new NSView()
                    {
                        // Property1 = (tableView.DataSource as TableViewDataSource).Objects[row].Property1,
                        // Property2 = (tableView.DataSource as TableViewDataSource).Objects[row].Property2
                    };
                }

                if (tableColumn.Identifier == "Column2")
                {
                    return new NSView()
                    {
                        // Property3 = (tableView.DataSource as TableViewDataSource).Objects[row].Property3
                    };
                }

                // Etc.

                return null;
            }
        }

        class OutlineDataSource : NSOutlineViewDataSource
        {
            public List<object> Objects = new List<object>();

            public override nint GetChildrenCount(NSOutlineView outlineView, NSObject item)
            {
                return 0;
            }

            public override NSObject GetChild(NSOutlineView outlineView, nint childIndex, NSObject item)
            {
                return new NSObject();
            }

            public override bool ItemExpandable(NSOutlineView outlineView, NSObject item)
            {
                return false;
            }
        }


        protected override StringBuilder OnConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            var code = new StringBuilder();

            // TODO: Get name from generator
            var name = currentNode.Name + "ScrollView";
            string outlineViewName = currentNode.Name;

            currentNode.Name = name;
            var frame = (FigmaFrame)currentNode.Node;

            code.WriteConstructor(name, typeof(NSScrollView));
            code.WritePropertyEquality(name, nameof(NSScrollView.AutoresizingMask),
                $"{nameof(NSViewResizingMask)}.{nameof(NSViewResizingMask.WidthSizable)} | " +
                $"{nameof(NSViewResizingMask)}.{nameof(NSViewResizingMask.HeightSizable)}");

            code.WritePropertyEquality(name, nameof(NSScrollView.BorderType), NSBorderType.BezelBorder);
            code.WritePropertyEquality(name, nameof(NSScrollView.DrawsBackground), true);
            code.AppendLine();

            if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
                code.WriteConstructor(outlineViewName, typeof(NSOutlineView), rendererService.NodeRendersVar(currentNode, parentNode));

            code.WritePropertyEquality(outlineViewName, nameof(NSOutlineView.Frame),
                string.Format("new {0} ({1}, {2}, {3}, {4})",
                typeof(CoreGraphics.CGRect), 0, 0, name + ".ContentSize.Width", name + ".ContentSize.Height"));

            var columnNodes = frame.FirstChild(s => s.name == ComponentString.COLUMNS && s.visible);

            // TODO: Parse options layers
            code.WritePropertyEquality(outlineViewName, nameof(NSOutlineView.UsesAlternatingRowBackgroundColors), false);
            code.WritePropertyEquality(outlineViewName, nameof(NSOutlineView.AllowsMultipleSelection), false);
            code.WritePropertyEquality(outlineViewName, nameof(NSOutlineView.AllowsColumnResizing), true);
            code.WritePropertyEquality(outlineViewName, nameof(NSOutlineView.AllowsColumnReordering), false);
            code.WritePropertyEquality(outlineViewName, nameof(NSOutlineView.AllowsEmptySelection), false);
            code.AppendLine();

            int columnCount = 1;
            foreach (FigmaNode tableColumNode in columnNodes.GetChildren(t => t.visible))
            {
                FigmaText text = tableColumNode.FirstChild(s => s.name == ComponentString.TITLE) as FigmaText;

                if (text == null)
                    continue;

                string columnId = "Column" + columnCount;

                code.WriteConstructor(columnId, typeof(NSTableColumn));

                code.WritePropertyEquality(columnId,
                    $"{nameof(NSTableColumn.HeaderCell)}.{nameof(NSTableColumn.HeaderCell.Alignment)}",
                    Helpers.CodeHelper.GetNSTextAlignmentString(text));

                code.WritePropertyEquality(columnId, nameof(NSTableColumn.Identifier), columnId, inQuotes: true);
                code.WritePropertyEquality(columnId, nameof(NSTableColumn.Title), rendererService.GetTranslatedText(text), inQuotes: true);
                code.WritePropertyEquality(columnId, nameof(NSTableColumn.Width), text.absoluteBoundingBox.Width.ToString(), inQuotes: false);
                code.WriteMethod(outlineViewName, nameof(NSOutlineView.AddColumn), columnId);
                code.AppendLine();

                if (columnCount == 1)
                    code.WritePropertyEquality(outlineViewName, nameof(NSOutlineView.OutlineTableColumn), columnId);

                columnCount++;
            }

            code.WritePropertyEquality(name, nameof(NSScrollView.DocumentView), outlineViewName, inQuotes: false);
            code.AppendLine();


            var rectangle = (RectangleVector)frame.FirstChild(s => s.name == ComponentString.BACKGROUND && s.visible);

            if (rectangle != null)
            {
                foreach (var styleMap in rectangle.styles)
                {
                    if (rendererService.NodeProvider.TryGetStyle(styleMap.Value, out FigmaStyle style) &&
                        styleMap.Key == "fill")
                    {
                        code.WritePropertyEquality(outlineViewName, nameof(NSOutlineView.BackgroundColor), ColorService.GetNSColorString(style.name));
                    }
                }
            }


            return code;
        }
    }
}
