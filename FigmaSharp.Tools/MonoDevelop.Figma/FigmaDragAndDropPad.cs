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
using Microsoft.VisualStudio.Text.Editor;
using MonoDevelop.Components;
using MonoDevelop.DesignerSupport.Toolbox;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using Xwt.GtkBackend;

namespace MonoDevelop.Figma
{
    public class FigmaDragAndDropPad : PadContent
    {
        Gtk.Widget widget;
        FigmaDragAndDropContent dragPad;
        TextToolboxNode selected;
        IPadWindow window;

        protected override void Initialize(IPadWindow window)
        {
            this.window = window;
            dragPad = new FigmaDragAndDropContent();

            window.PadContentHidden += Container_PadHidden;
            window.PadContentShown += Container_PadShown;

            widget = new Gtk.GtkNSViewHost (dragPad);

            widget.DragBegin += (o, args) => {
				if (!isDragging) {
                    var code = dragPad.GetCode (dragPad.SelectedNode);

                    selected = new TextToolboxNode (code);
					
                    DesignerSupport.DesignerSupport.Service.ToolboxService.SelectItem (selected);
                    CurrentConsumer.DragItem (selected, widget, args.Context);
                    //	DesignerSupport.Service..(widget, args.Context);
                    isDragging = true;
				}
			};

            widget.DragEnd += (o, args) => {
                isDragging = false;
            };

            widget.Focused += (s, e) => {
                // toolbox
            };

            dragPad.SelectCode += (sender, e) =>
            {
                if (!string.IsNullOrEmpty (e))
                {
                    try
                    {
                        var editor = IdeApp.Workbench.ActiveDocument.GetContent<ITextView>();
                        var position = editor.Caret.Position.BufferPosition.Position;
                        editor.TextBuffer.Insert(position, e);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            };

            dragPad.DragSourceSet += (s, e) => {
                targets = new Gtk.TargetList();
                targets.AddTable(e);
            };
            dragPad.DragBegin += (object sender, EventArgs e) => {
                if (!isDragging)
                {
                    Gtk.Drag.SourceUnset(widget);

                    // Gtk.Application.CurrentEvent and other copied gdk_events seem to have a problem
                    // when used as they use gdk_event_copy which seems to crash on de-allocating the private slice.
                    IntPtr currentEvent = Components.GtkWorkarounds.GetCurrentEventHandle();
                    Gtk.Drag.Begin(widget, targets, Gdk.DragAction.Copy | Gdk.DragAction.Move, 1, new Gdk.Event(currentEvent));

                    // gtk_drag_begin does not store the event, so we're okay
                    //Components.GtkWorkarounds.FreeEvent(currentEvent);
                }
            };

            widget.ShowAll();


            if (IdeApp.Workbench != null)
            {
                //IdeApp.Workbench.ActiveDocumentChanged += Workbench_ActiveDocumentChanged;
                 IdeApp.Workbench.ActiveDocumentChanged += onActiveDocChanged; // += new EventHandler(onActiveDocChanged);
                onActiveDocChanged(null, null);
            }
        }

        private void Container_PadShown(object sender, EventArgs e)
        {
            dragPad.RefreshUIStates();
            dragPad.Hidden = false;
        }
        private void Container_PadHidden(object sender, EventArgs e)
        {
            dragPad.Hidden = true;
        }

        void onActiveDocChanged (object sender, DocumentEventArgs e)
		{
			if (IdeApp.Workbench.ActiveDocument != null) {

                CurrentConsumer = IdeApp.Workbench.ActiveDocument.GetContent<IToolboxConsumer> ();
			} else {
				CurrentConsumer = null;
			}
		}

        IToolboxConsumer CurrentConsumer;

        Gtk.TargetList targets = new Gtk.TargetList();
        bool isDragging = false;

        public override void Dispose()
        {
            if (window != null)
            {
                window.PadHidden -= Container_PadHidden;
                window.PadShown -= Container_PadShown;
                window = null;
            }
            base.Dispose();
        }

        public override Control Control
        {
            get { return widget; }
        }
    }
}