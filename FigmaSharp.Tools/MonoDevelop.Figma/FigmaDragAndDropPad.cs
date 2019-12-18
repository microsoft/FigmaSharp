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
using Gtk;
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
        Gtk.TargetList targets = CreateDefaultTargeList ();

        public const string ToolBoxDragDropFormat = "MonoDevelopToolBox";
        private static TargetList CreateDefaultTargeList () =>
            new Gtk.TargetList (new TargetEntry[] { new TargetEntry (ToolBoxDragDropFormat, TargetFlags.OtherWidget, 0) });

        Gtk.Widget widget;
        FigmaDragAndDropContent dragPad;
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
                    DesignerSupport.DesignerSupport.Service.ToolboxService.DragSelectedItem (widget, args.Context);
                    isDragging = true;
                }
			};

            widget.DragDataGet += (object o, DragDataGetArgs args) => {
                if (selectedNode is IDragDataToolboxNode node) {
                    foreach (var format in node.Formats) {
                        args.SelectionData.Set (Gdk.Atom.Intern (format, false), 8, node.GetData (format));
                    }
                }
            };

            widget.DragEnd += (o, args) => {
                isDragging = false;
            };

            dragPad.SelectCode += (sender, e) =>
            {
                if (!string.IsNullOrEmpty (e))
                {
                    var selected = new TextToolboxNode (e);
                    DesignerSupport.DesignerSupport.Service.ToolboxService.SelectItem (selected);
                    DesignerSupport.DesignerSupport.Service.ToolboxService.UseSelectedItem ();
                }
            };

            dragPad.DragSourceSet += (s, e) => {
                targets = CreateDefaultTargeList ();
                targets.AddTable(e);
            };

            dragPad.DragBegin += (object sender, EventArgs e) => {
                var code = dragPad.GetCode (dragPad.SelectedNode);
                selectedNode = new TextToolboxNode (code);
                DesignerSupport.DesignerSupport.Service.ToolboxService.SelectItem (selectedNode);

                Gtk.Drag.SourceUnset (widget);
                if (selectedNode is IDragDataToolboxNode node) {
                    foreach (var format in node.Formats) {
                        targets.Add (format, 0, 0);
                    }
                }
                // Gtk.Application.CurrentEvent and other copied gdk_events seem to have a problem
                // when used as they use gdk_event_copy which seems to crash on de-allocating the private slice.
                IntPtr currentEvent = Components.GtkWorkarounds.GetCurrentEventHandle ();
                Gtk.Drag.Begin (widget, targets, Gdk.DragAction.Copy | Gdk.DragAction.Move, 1, new Gdk.Event (currentEvent, false));

				// gtk_drag_begin does not store the event, so we're okay
				Components.GtkWorkarounds.FreeEvent (currentEvent);
            };

            widget.ShowAll();

            if (IdeApp.Workbench != null)
            {
                //IdeApp.Workbench.ActiveDocumentChanged += Workbench_ActiveDocumentChanged;
                 IdeApp.Workbench.ActiveDocumentChanged += onActiveDocChanged; // += new EventHandler(onActiveDocChanged);
                onActiveDocChanged(null, null);
            }
        }

        TextToolboxNode selectedNode;

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