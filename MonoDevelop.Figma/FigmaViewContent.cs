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
using System.Threading.Tasks;
using AppKit;
using MonoDevelop.Components;
using MonoDevelop.Components.Mac;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using FigmaSharp;
using System;
using Foundation;
using CoreGraphics;
using System.Collections.Generic;
using MonoDevelop.Projects;
using System.IO;
using FigmaSharp.Services;
using System.Linq;
using System.Xml.Linq;
using FigmaSharp.Designer;
using MonoDevelop.DesignerSupport;
using Gtk;
using System.ComponentModel;
using MonoDevelop.Components.PropertyGrid;
using MonoDevelop.Ide.Gui.Documents;

namespace MonoDevelop.Figma
{
    [ExportFileDocumentController(
        Id = "FigmaDesignerViewer",
        Name = "Figma Designer",
        FileExtension = ".figma",
        CanUseAsDefault = true,
        InsertBefore = "DefaultDisplayBinding")]
    class FigmaViewContent : FileDocumentController, IOutlinedDocument, ICustomPropertyPadProvider
    {
        FigmaDesignerSession session;
        IFigmaDesignerDelegate figmaDelegate;
        FigmaDesignerSurface surface;

        FigmaDesignerOutlinePad outlinePad;

        private FilePath fileName;
        NSStackView container;

        Gtk.Widget _content;
        PropertyGrid grid;

        readonly IScrollViewWrapper scrollViewWrapper;

        public FigmaViewContent()
        {

            container = new NSStackView();

            container.Spacing = 10;
            container.WantsLayer = true;
            container.Layer.BackgroundColor = NSColor.DarkGray.CGColor;

            container.Distribution = NSStackViewDistribution.Fill;
            container.Orientation = NSUserInterfaceLayoutOrientation.Vertical;

            _content = GtkMacInterop.NSViewToGtkWidget(container);
            _content.CanFocus = true;
            _content.Sensitive = true;

            var scrollView = new NSScrollView()
            {
                HasVerticalScroller = true,
                HasHorizontalScroller = true,
            };

            scrollView.AutohidesScrollers = false;
            scrollView.BackgroundColor = NSColor.DarkGray;
            scrollView.ScrollerStyle = NSScrollerStyle.Legacy;

            scrollViewWrapper = new ScrollViewWrapper(scrollView);

            var contentView = new NSView();
            contentView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
            scrollView.DocumentView = contentView;

            container.AddArrangedSubview(scrollView);

            _content.ShowAll();

            grid = PropertyPad.Instance.Control;
            grid.Changed += PropertyPad_Changed;
        }

        void PropertyPad_Changed(object sender, EventArgs e)
        {
            session.Reload();
        }

        protected override Task OnSave()
        {
            session.Save(fileName);
            HasUnsavedChanges = false;
            return Task.FromResult(true);
        }

        protected override async Task OnInitialize(ModelDescriptor modelDescriptor, Properties status)
        {
            if (!(modelDescriptor is FileDescriptor fileDescriptor))
                throw new InvalidOperationException();

            if (session == null)
            {
                Owner = fileDescriptor.Owner;
                fileName = fileDescriptor.FilePath;
                DocumentTitle = fileDescriptor.FilePath.FileName;

                figmaDelegate = new FigmaDesignerDelegate();

                session = new FigmaDesignerSession();
                //session.ModifiedChanged += HandleModifiedChanged;
                session.ReloadFinished += Session_ReloadFinished;

                surface = new FigmaDesignerSurface(figmaDelegate)
                {
                    Session = session
                };

                surface.FocusedViewChanged += Surface_FocusedViewChanged;

                var window = NSApplication.SharedApplication.MainWindow;
                surface.SetWindow(new WindowInternalWrapper(window));
                surface.StartHoverSelection();

                //IdeApp.Workbench.ActiveDocumentChanged += OnActiveDocumentChanged;
                IdeApp.Workbench.DocumentOpened += OnDocumentOpened;
            }

            if (fileDescriptor.Owner is DotNetProject project)
            {
                session.Reload(fileName, project.BaseDirectory);
            }
            await base.OnInitialize(modelDescriptor, status);
        }
        
        void Surface_FocusedViewChanged(object sender, IViewWrapper e)
        {
            var model = session.GetModel(e);
            if (outlinePad != null) {
                outlinePad.Focus(model);
            }
            PropertyPad.Instance.Control.CurrentObject = GetWrapper (model);
        }

        FigmaNodeWrapper GetWrapper (FigmaNode node)
        {
            if (node is FigmaFrameEntity figmaFrameEntity)
            {
                return new FigmaFrameEntityWrapper(figmaFrameEntity);
            }

            if (node is FigmaText figmaText)
            {
                return new FigmaTextWrapper(figmaText);
            }

            if (node is FigmaRectangleVector figmaRectangleVector)
            {
                return new FigmaRectangleVectorWrapper(figmaRectangleVector);
            }

            if (node is FigmaCanvas canvas)
            {
                return new FigmaCanvasWrapper(canvas);
            }
            if (node is FigmaVectorEntity vectorEntity)
            {
                return new FigmaVectorEntityWrapper(vectorEntity);
            }

            return new FigmaNodeWrapper(node);
        }

        void Session_ReloadFinished(object sender, EventArgs e)
        {
            scrollViewWrapper.ClearSubviews();

            foreach (var items in session.MainViews) {
                scrollViewWrapper.AddChild(items.View);
            }

            var mainNodes = session.ProcessedNodes
               .Where(s => s.ParentView == null)
               .ToArray();

            Reposition(mainNodes);

            //we need reload after set the content to ensure the scrollview
            scrollViewWrapper.AdjustToContent();
        }

        public void Reposition(ProcessedNode[] mainNodes)
        {
            //Alignment 
            const int Margin = 20;
            float currentX = Margin;
            foreach (var processedNode in mainNodes)
            {
                var view = processedNode.View;
                scrollViewWrapper.AddChild(view);

                view.X = currentX;
                view.Y = 0; //currentView.Height + currentHeight;
                currentX += view.Width + Margin;
            }
        }

        private void OnDocumentOpened(object sender, DocumentEventArgs e)
        {
            UpdateLayout();
        }

        private void OnActiveDocumentChanged(object sender, EventArgs e)
        {
            UpdateLayout();
        }

        string lastLayout;
        private void UpdateLayout()
        {
            var current = IdeApp.Workbench.ActiveDocument?.GetContent<string>();
            if (current == null)
            {
                if (lastLayout != null && IdeApp.Workbench.CurrentLayout == "Visual Design")
                    IdeApp.Workbench.CurrentLayout = lastLayout;
                lastLayout = null;
            }
            else
            {
                if (IdeApp.Workbench.CurrentLayout != "Visual Design")
                {
                    if (lastLayout == null)
                    {
                        lastLayout = IdeApp.Workbench.CurrentLayout;
                        IdeApp.Workbench.CurrentLayout = "Visual Design";
                    }
                }
                //current.widget.SetFocus();
            }
        }

        #region IOutlinedDocument

        public Widget GetOutlineWidget()
        {
            outlinePad = FigmaDesignerOutlinePad.Instance;
            outlinePad.GenerateTree(session.Response.document, figmaDelegate);

            outlinePad.RaiseFirstResponder += OutlinePad_RaiseFirstResponder;
            outlinePad.RaiseDeleteItem += OutlinePad_RaiseDeleteItem;
            return outlinePad;
        }

        void OutlinePad_RaiseDeleteItem(object sender, FigmaNode e)
        {
            HasUnsavedChanges = true;
            session.DeleteView(e);
            RefreshAll();
        }

        void RefreshAll ()
        {
            session.Reload();
            if (outlinePad != null)
            {
                var selectedView = surface.SelectedView;
                var selectedModel = session.GetModel(selectedView);

                outlinePad.GenerateTree(session.Response.document, figmaDelegate);
                outlinePad.Focus(selectedModel);
            }
        }

        void OutlinePad_RaiseFirstResponder(object sender, FigmaNode e)
        {
            var view = session.GetViewWrapper(e);
            surface.ChangeFocusedView(view);
        }

        public IEnumerable<Widget> GetToolbarWidgets()
        {
            yield break;
        }

        public void ReleaseOutlineWidget()
        {
            // throw new NotImplementedException();
        }

        #endregion

        #region ICustomPropertyPadProvider

        PropertyContentPad propertyPad;

        public Widget GetCustomPropertyWidget()
        {
            PropertyPad.Initialize(session);
            propertyPad = PropertyPad.Instance;
            //FigmaDesignerPropertyPad.Instance.SetSource(this, ds);
            return propertyPad.Control;
        }

        public void DisposeCustomPropertyWidget()
        {
            //throw new NotImplementedException();
        }

        #endregion

        protected override Control OnGetViewControl(DocumentViewContent view)
        {
            return _content;
        }

        protected override void OnDispose()
        {
            surface.StopHover();
            base.OnDispose();
        }
    }
}