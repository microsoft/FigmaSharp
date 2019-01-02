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
using FigmaSharp.Converters;

namespace MonoDevelop.Figma
{
	public class FigmaViewContent : ViewContent
	{
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

        readonly FigmaLocalFileService fileService;
        readonly FigmaRendererService rendererService;
        readonly IScrollViewWrapper scrollViewWrapper;

        public FigmaViewContent (FilePath fileName)
		{
		
			this.fileName = fileName;
			ContentName = fileName;

            fileService = new FigmaLocalFileService();
            rendererService = new FigmaRendererService(fileService);

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

		void Reload ()
		{
            try {
                var resourcesDirectoryPath = Path.Combine(this.Project.BaseDirectory, "Resources");
                if (!Directory.Exists(resourcesDirectoryPath))
                {
                    throw new DirectoryNotFoundException(resourcesDirectoryPath);
                }

                fileService.Start(fileName);
                rendererService.Start();

                var mainNodes = fileService.NodesProcessed
                    .Where(s => s.ParentView == null)
                    .ToArray();

                foreach (var items in rendererService.MainViews)
                {
                    scrollViewWrapper.AddChild(items.View);
                }

                Reposition(mainNodes);

                //we need reload after set the content to ensure the scrollview
                scrollViewWrapper.AdjustToContent();

            } catch (DirectoryNotFoundException ex) {
                Console.WriteLine ("[FIGMA.RENDERER] Resource directory not found ({0}). Images will not load", ex.Message);
            } catch (System.Exception ex) {
				Console.WriteLine (ex);
			}
		}

        public void ReloadImages(string resourcesDirectory, string format = ".png")
        {
            Console.WriteLine($"Loading images..");

            var imageVectors = fileService.ImageVectors;
            if (imageVectors?.Count > 0)
            {
                foreach (var imageVector in imageVectors)
                {
                    try
                    {
                        var recoveredKey = FigmaResourceConverter.FromResource(imageVector.Key.id);
                        string filePath = Path.Combine(resourcesDirectory, string.Concat(recoveredKey, format));

                        if (!File.Exists(filePath))
                        {
                            throw new FileNotFoundException(filePath);
                        }

                        var processedNode = fileService.NodesProcessed.FirstOrDefault(s => s.FigmaNode == imageVector.Key);
                        var wrapper = processedNode.View as IImageViewWrapper;

                        var image = new ImageWrapper (new NSImage(filePath));
                        wrapper.SetImage(image);
                    }
                    catch (FileNotFoundException ex)
                    {
                        Console.WriteLine("[FIGMA.RENDERER] Resource '{0}' not found.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    //FigmaImages.Add(wrapper);
                }
            }
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

        void Editor_TextChanged (object sender, Core.Text.TextChangeEventArgs e)
		{

		}

        protected override void OnSetProject(Project project)
        {
            base.OnSetProject(project);
            Reload();
        }

        public override void Dispose ()
		{
			IdeApp.Workbench.ActiveDocument.Editor.TextChanged -= Editor_TextChanged;
			base.Dispose ();
		}

		public override Control Control => _content;
	}
}