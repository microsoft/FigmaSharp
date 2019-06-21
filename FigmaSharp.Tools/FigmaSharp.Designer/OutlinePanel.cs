/* 
 * FigmaViewContent.cs 
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
using FigmaSharp;

namespace MonoDevelop.Figma
{
    public class OutlinePanel
    {
        const ushort DeleteKey = 51;
        public ScrollContainerView EnclosingScrollView { get; }
        public OutlineView View { get; }
        public bool Hidden {
            get => View.Hidden;
            set => View.Hidden = value;
        }

        public event EventHandler<FigmaNode> RaiseFirstResponder;
        public event EventHandler<FigmaNode> RaiseDeleteItem;
        public event EventHandler<FigmaNode> DoubleClick;
        public event EventHandler<FigmaNode> StartDrag;

        public OutlinePanel()
        {
            View = new OutlineView();
            EnclosingScrollView = new ScrollContainerView(View);

            View.SelectionNodeChanged += (s, e) =>
            {
                if (View.SelectedNode is FigmaNodeView nodeView)
                {
                    RaiseFirstResponder?.Invoke(this, nodeView.Wrapper);
                }
            };

            View.KeyPress += (sender, e) =>
            {
                if (e == DeleteKey)
                {
                    if (View.SelectedNode is FigmaNodeView nodeView)
                    {
                        RaiseDeleteItem?.Invoke(this, nodeView.Wrapper);
                    }
                }
            };

            View.DoubleClick += (sender, e) =>
            {
                DoubleClick?.Invoke(this, (View.SelectedNode as FigmaNodeView)?.Wrapper);
            };

            View.StartDrag += (sender, e) =>
            {
                StartDrag?.Invoke(this, (e as FigmaNodeView)?.Wrapper);
            };
        }

        public void FocusSelectedView()
        {
            var window = View.Window;
            window.MakeFirstResponder(View);
        }


        public void GenerateTree(Node node)
        {
            //data = new NodeView(window.ContentView);
            //inspectorDelegate.ConvertToNodes(window.ContentView, new NodeWrapper(data), viewMode);
            View.SetData(node);
        }

        public void FocusNode(Node node)
        {
            View.FocusNode(node);
        }
    }
}