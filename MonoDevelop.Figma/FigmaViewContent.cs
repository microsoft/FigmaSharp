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

namespace MonoDevelop.Figma
{
    public class FigmaViewContent : ViewContent
    {
        XamarinStudioIdeService ideService;
        FigmaDesignerSession session;

        private FilePath fileName;
		NSStackView container;

		public override bool IsReadOnly {
			get {
				return true;
			}
		}

		public override bool IsFile {
			get {
				return true;
			}
		}

		public override string TabPageLabel {
			get {
				return fileName.FileName;
			}
		}

		public override bool IsViewOnly {
			get {
				return true;
			}
		}

		Gtk.Widget _content;

        readonly IScrollViewWrapper scrollViewWrapper;

        public FigmaViewContent (FilePath fileName)
		{
		
			this.fileName = fileName;
			ContentName = fileName;

            container = new NSStackView ();
           
            container.Spacing = 10;
			container.WantsLayer = true;
			container.Layer.BackgroundColor = NSColor.DarkGray.CGColor;

			container.Distribution = NSStackViewDistribution.Fill;
			container.Orientation = NSUserInterfaceLayoutOrientation.Vertical;

			_content = GtkMacInterop.NSViewToGtkWidget (container);
			_content.CanFocus = true;
			_content.Sensitive = true;

			var scrollView = new FlippedScrollView()
            {
				HasVerticalScroller = true,
				HasHorizontalScroller = true,
			};

			scrollView.AutohidesScrollers = false;
			scrollView.BackgroundColor = NSColor.DarkGray;
			scrollView.ScrollerStyle = NSScrollerStyle.Legacy;

            scrollViewWrapper = new ScrollViewWrapper(scrollView);

            var contentView = new FlippedView ();
            contentView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
            scrollView.DocumentView = contentView;

            container.AddArrangedSubview (scrollView);

			//IdeApp.Workbench.ActiveDocument.Editor.TextChanged += Editor_TextChanged;

			_content.ShowAll ();
		}

        void Editor_TextChanged (object sender, Core.Text.TextChangeEventArgs e)
		{

		}

        public override Task Load(FileOpenInformation fileOpenInformation)
        {
            fileName = fileOpenInformation.FileName;

            if (session == null)
            {
                CreateSession();
                IdeApp.Workbench.ActiveDocumentChanged += OnActiveDocumentChanged;
                IdeApp.Workbench.DocumentOpened += OnDocumentOpened;
            }

            session.Reload(fileName, Project.BaseDirectory, scrollViewWrapper);

            ContentName = fileName;
            return Task.FromResult(true);
        }

        private void CreateSession()
        {
            ideService = new XamarinStudioIdeService(Project);

            session = new FigmaDesignerSession();
            session.ModifiedChanged += HandleModifiedChanged;
        }

        private void HandleModifiedChanged(object sender, EventArgs e)
        {
            if (session == null)
                return;

            IsDirty = session.IsModified;
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

        protected override void OnSetProject(Project project)
        {
            base.OnSetProject(project);
           
        }

        public override void Dispose ()
		{
			IdeApp.Workbench.ActiveDocument.Editor.TextChanged -= Editor_TextChanged;
			base.Dispose ();
		}

		public override Control Control => _content;
    }

    class XamarinStudioIdeService
    {
        private Project project;

        public XamarinStudioIdeService(Project project)
        {
            this.project = project;
        }
    }

}