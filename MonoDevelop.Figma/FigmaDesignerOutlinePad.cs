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
using FigmaSharp.Designer;
using Xwt.GtkBackend;

namespace MonoDevelop.Figma
{
    public class FigmaDesignerOutlinePad : Gtk.VBox
    {
        public event EventHandler<FigmaNode> RaiseFirstResponder;
        public event EventHandler<FigmaNode> RaiseDeleteItem;

        static FigmaDesignerOutlinePad instance;

        public static FigmaDesignerOutlinePad Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FigmaDesignerOutlinePad();
                }

                return instance;
            }
        }

        OutlinePanel outlinePanel;
        public FigmaDesignerOutlinePad()
        {
            outlinePanel = new OutlinePanel();

            outlinePanel.RaiseFirstResponder += (s, e) =>
            {
                RaiseFirstResponder?.Invoke(s, e);
            };
            outlinePanel.RaiseDeleteItem += (s, e) =>
            {
                RaiseDeleteItem?.Invoke(s, e);
            };

            var widget = GtkMacInterop.NSViewToGtkWidget(outlinePanel.EnclosingScrollView);
            CanFocus = widget.CanFocus = true;
            Sensitive = widget.Sensitive = true;

            widget.Focused += FigmaDesignerOutlinePad_Focused;

            PackStart(widget, true, true, 0);
            ShowAll();

            Focused += FigmaDesignerOutlinePad_Focused;
        }

        void FigmaDesignerOutlinePad_Focused(object o, Gtk.FocusedArgs args)
        {
            outlinePanel.FocusSelectedView();
        }

        public void Focus(FigmaNode model)
        {
            if (data != null && model != null)
            {
                var found = Search (data, model);
                if (found != null)
                {
                    outlinePanel.FocusNode(found);
                }
            }
        }

        static FigmaNodeView Search (FigmaNodeView nodeView, FigmaNode view)
        {
            if (nodeView.Wrapper != null && nodeView.Wrapper == view)
            {
                return nodeView;
            }

            if (nodeView.ChildCount == 0)
            {
                return null;
            }

            for (int i = 0; i < nodeView.ChildCount; i++)
            {
                var node = (FigmaNodeView)nodeView.GetChild(i);
                var found = Search(node, view);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        FigmaNodeView data;
        internal void GenerateTree(FigmaDocument document, IDesignerDelegate figmaDelegate)
        {
            data = new FigmaNodeView(document);
            figmaDelegate.ConvertToNodes (document, data);
            outlinePanel.GenerateTree(data);
        }
    }

}