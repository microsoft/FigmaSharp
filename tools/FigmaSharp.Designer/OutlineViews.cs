// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
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

using AppKit;
using Foundation;

using FigmaSharp.Cocoa;
using FigmaSharp.Views;

namespace FigmaSharp
{
    public class OutlineView : NSOutlineView
    {
        public event EventHandler<Node> StartDrag;
        public event EventHandler<ushort> KeyPress;

        public override void KeyDown(NSEvent theEvent)
        {
            base.KeyDown(theEvent);
            KeyPress?.Invoke(this, theEvent.KeyCode);
        }

        readonly MainNode Data = new MainNode ();

        public event EventHandler SelectionNodeChanged;

        Node selectedNode;
        public Node SelectedNode {
            get => selectedNode;
            set {
                if (selectedNode == value) {
                    return;
                }

                selectedNode = value;
                SelectionNodeChanged?.Invoke (this, EventArgs.Empty);
            }
        }

        public OutlineView ()
        {
            AllowsExpansionToolTips = true;
            AllowsMultipleSelection = false;
            AutosaveTableColumns = false;
            FocusRingType = NSFocusRingType.None;
            IndentationPerLevel = 16;
            RowHeight = 32;
            NSTableColumn column = new NSTableColumn ("Values");
            column.Title = "Layers";
            AddColumn (column);
            OutlineTableColumn = column;
            Delegate = new OutlineViewDelegate ();
            var outlineViewDataSource = new OutlineViewDataSource(Data);
            DataSource = outlineViewDataSource;
            outlineViewDataSource.StartDrag += (sender, e) =>
            {
                StartDrag?.Invoke(this, e as Node);
            };
        }

        public void SetData (Node data)
        {
            this.Data.Node = data;
            ReloadData ();
            ExpandItem (ItemAtRow (0), true);
        }

        class OutlineViewDelegate : NSOutlineViewDelegate
        {
            public OutlineViewDelegate ()
            {
            }

            const string identifer = "myCellIdentifier";
            public override NSView GetView (NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
            {
                var view = (NSTextField)outlineView.MakeView (identifer, this);
                if (view == null) {
                    view = FigmaExtensions.CreateLabel (((Node)item).Name);
                }
                return view;
            }

            public override bool ShouldSelectItem (NSOutlineView outlineView, NSObject item)
            {
                ((OutlineView) outlineView).SelectedNode = (Node)item;
                return true;
            }
        }

        internal void FocusNode (Node node)
        {
            if (this.RowCount < 0 ) {
                return;
            }
            var index = RowForItem (node);
            if (index >= 0) {
                SelectRow (index, false);
            }
        }
    }

    class MainNode
    {
        public Node Node {
            get;
            set;
        }

        public MainNode ()
        {
            Node = new Node ("test");
        }
    }


    public class NodeView : Node
    {
        static string GetName (IView view)
        {
            var name = string.Format("{0} ({1})", view.NodeName, view.Identifier ?? "N.I");
            if (view.Hidden) {
                name += " (hidden)";
            }
            return name;
        }


        public readonly IView Wrapper;

        public NodeView (IView view) : base (GetName (view))
        {
            this.Wrapper = view;
        }
    }

    public class Node : NSObject
    {
        public string Name { get; private set; }
        List<Node> Children;

        public Node (string name)
        {
            Name = name;
            Children = new List<Node> ();
        }

        public Node AddChild (string name)
        {
            Node n = new Node (name);
            Children.Add (n);
            return n;
        }

        public void AddChild (Node node)
        {
            Children.Add (node);
        }

        public Node GetChild (int index)
        {
            return Children [index];
        }

        public int ChildCount { get { return Children.Count; } }
        public bool IsLeaf { get { return ChildCount == 0; } }
    }

    class OutlineViewDataSource : NSOutlineViewDataSource
    {
        MainNode mainNode;
        public OutlineViewDataSource (MainNode mainNode)
        {
            this.mainNode = mainNode; 
        }

        public event EventHandler<NSObject> StartDrag;

        public override INSPasteboardWriting PasteboardWriterForItem(NSOutlineView outlineView, NSObject item)
        {
            StartDrag?.Invoke(this, item);
            return null;
        }

        public override nint GetChildrenCount (NSOutlineView outlineView, NSObject item)
        {
            item = item == null ? mainNode.Node : item;
            return ((Node)item).ChildCount;
        }

        public override NSObject GetChild (NSOutlineView outlineView, nint childIndex, NSObject item)
        {
            item = item == null ? mainNode.Node : item;
            return ((Node)item).GetChild ((int)childIndex);
        }

        public override bool ItemExpandable (NSOutlineView outlineView, NSObject item)
        {
            item = item == null ? mainNode.Node : item;
            return !((Node)item).IsLeaf;
        }
    }
}