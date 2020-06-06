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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AppKit;
using FigmaSharp;
using FigmaSharp.Controls.Cocoa;
using FigmaSharp.Controls.Services;
using FigmaSharp.Designer;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;
using Gtk;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.DesignerSupport;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Documents;

namespace MonoDevelop.Figma
{
    [ExportFileDocumentController(
        Id = "FigmaDesignerViewer",
        Name = "Figma Designer",
        FileExtension = ".figma",
        CanUseAsDefault = true,
        InsertBefore = "DefaultDisplayBinding")]
    class FigmaFileDocumentController : FileDocumentController, IOutlinedDocument, IPropertyPadProvider
    {
        FigmaDesignerSession session;
        IFigmaDesignerDelegate figmaDelegate;
        FigmaDesignerSurface surface;

        FigmaDesignerOutlinePad outlinePad;

        ControlsLocalFileProvider fileProvider;
        NativeViewRenderingService rendererService;
        StoryboardLayoutManager layoutManager;

        private FilePath filePath;

        Gtk.Widget _content;
        //PropertyGrid grid;

        readonly IScrollView scrollview;

        public FigmaFileDocumentController()
        {
            scrollview = new ScrollView ();

            var nativeScrollview = (FigmaSharp.Views.Native.Cocoa.FNSScrollview)scrollview.NativeObject;
            nativeScrollview.TranslatesAutoresizingMaskIntoConstraints = true;

            _content = new GtkNSViewHost (nativeScrollview);
           
            _content.ShowAll();
        }

        async void PropertyPad_Changed(object sender, EventArgs e)
        {
            await session.ReloadAsync (scrollview.ContentView, filePath.FileName, fileOptions);
        }

        protected override Task OnSave()
        {
            session.Save(filePath);
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
                filePath = fileDescriptor.FilePath;
                DocumentTitle = fileDescriptor.FilePath.FileName;

                figmaDelegate = new FigmaDesignerDelegate();
              
                var localPath = Path.Combine (filePath.ParentDirectory.FullPath, FigmaBundle.ResourcesDirectoryName);

                fileProvider = new ControlsLocalFileProvider(localPath) { File = filePath.FullPath };
                rendererService = new NativeViewRenderingService (fileProvider);

                //we generate a new file provider for embeded windows
                var tmpRemoteProvider = new FileNodeProvider(localPath) { File = filePath.FullPath };
                rendererService.CustomConverters.Add (new EmbededWindowConverter(tmpRemoteProvider) { LiveButtonAlwaysVisible = false });
                rendererService.CustomConverters.Add (new EmbededSheetDialogConverter(tmpRemoteProvider));

                layoutManager = new StoryboardLayoutManager();
                session = new FigmaDesignerSession(fileProvider, rendererService, layoutManager);
                //session.ModifiedChanged += HandleModifiedChanged;
                session.ReloadFinished += Session_ReloadFinished;

                surface = new FigmaDesignerSurface(figmaDelegate, session) {
                    Session = session
                };

                surface.FocusedViewChanged += Surface_FocusedViewChanged;

                var window = NSApplication.SharedApplication.MainWindow;
                surface.SetWindow(new WindowInternalWrapper(window));
                surface.StartHoverSelection();

                //IdeApp.Workbench.ActiveDocumentChanged += OnActiveDocumentChanged;
                IdeApp.Workbench.DocumentOpened += OnDocumentOpened;
            }
            await RefreshAll();
            await base.OnInitialize(modelDescriptor, status);
        }

        void Surface_FocusedViewChanged(object sender, IView e)
        {
            var model = session.GetModel(e);
			if (model == null) {
                return;
			}

            if (outlinePad != null)  {
                outlinePad.Focus(model);
            }
            //var currentWrapper = GetWrapper (model);
			
            DesignerSupport.DesignerSupport.Service.PropertyPad?.SetCurrentObject (model, new object[] { model });
            //PropertyPad.Instance.Control.CurrentObject = GetWrapper(model);
        }

        void Session_ReloadFinished(object sender, EventArgs e)
        {
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

        async void OutlinePad_RaiseDeleteItem(object sender, FigmaNode e)
        {
            HasUnsavedChanges = true;
            session.DeleteView(e);
            await RefreshAll();
        }

        FigmaViewRendererServiceOptions fileOptions = new FigmaViewRendererServiceOptions();

        async Task RefreshAll()
        {
            await session.ReloadAsync (scrollview.ContentView, filePath, fileOptions);
            if (outlinePad != null) {
                outlinePad.GenerateTree(session.Response.document, figmaDelegate);
                outlinePad.Focus(GetCurrentSelectedNode ());
            }
        }

		FigmaNode GetCurrentSelectedNode ()
		{
            var selectedView = surface.SelectedView;
            var selectedModel = session.GetModel (selectedView);
            return selectedModel;
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

        //#region ICustomPropertyPadProvider

        //PropertyContentPad propertyPad;

        //public Widget GetCustomPropertyWidget()
        //{
        //    PropertyPad.Initialize(session);
        //    propertyPad = PropertyPad.Instance;
        //    return propertyPad.Control;
        //}

        //public void DisposeCustomPropertyWidget()
        //{
        //    //throw new NotImplementedException();
        //}

        //#endregion

        protected override Control OnGetViewControl(DocumentViewContent view)
        {
            return _content;
        }

        protected override void OnDispose()
        {
            surface.StopHover();
            base.OnDispose();
        }

		public object GetActiveComponent ()
		{
            return GetCurrentSelectedNode ();
        }

		public object GetProvider ()
		{
            return GetCurrentSelectedNode ();
        }

		public void OnEndEditing (object obj)
		{
			
		}

		public void OnChanged (object obj)
		{
			
		}
	}
}