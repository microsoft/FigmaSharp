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
using AppKit;
using System.Linq;
using MonoDevelop;

using CoreGraphics;
using FigmaSharp;
using FigmaSharp.Designer;
using FigmaSharp.Services;
using MonoDevelop.Ide;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace MonoDevelop.Figma
{
    class FigmaDragAndDropContent : NSView
    {
        public event EventHandler<string> SelectCode;
        public event EventHandler DragBegin;
        public event EventHandler DragSourceUnset;
        public event EventHandler<Gtk.TargetEntry[]> DragSourceSet;

        public FigmaNode SelectedNode => (outlinePanel.View.SelectedNode as FigmaNodeView)?.Wrapper;

        public override bool IsFlipped => true;
        public const int TextType = 1;
        OutlinePanel outlinePanel;
        NSTextField fileTextField;
        NSButton openFileButton, loadAssemblyButton;
        NSScrollView scrollView;
        NSStackView toolbarBox;

        public FigmaDragAndDropContent()
        {
            toolbarBox = ViewHelpers.CreateHorizontalStackView();
            toolbarBox.EdgeInsets = new NSEdgeInsets(0, 10, 0, 10);
            AddSubview(toolbarBox);
            toolbarBox.AutoresizingMask = NSViewResizingMask.WidthSizable;
            toolbarBox.WantsLayer = true;
            fileTextField = new NSTextField();
            fileTextField.StringValue = "UeIJu6C1IQwPkdOut2IWRgGd";
            toolbarBox.AddArrangedSubview(fileTextField);

            openFileButton = new NSButton() { Title = "Open" };
            openFileButton.BezelStyle = NSBezelStyle.RoundRect;
            toolbarBox.AddArrangedSubview(openFileButton);

            openFileButton.Activated += openFileButton_Activated;

            loadAssemblyButton = new NSButton() { Title = "Load" };
            loadAssemblyButton.BezelStyle = NSBezelStyle.RoundRect;
            toolbarBox.AddArrangedSubview(loadAssemblyButton);

            loadAssemblyButton.Activated += loadAssemblyButton_Activated;

            outlinePanel = new OutlinePanel();
            scrollView = outlinePanel.EnclosingScrollView;
            outlinePanel.RaiseFirstResponder += OutlinePanel_RaiseFirstResponder;

            AddSubview(scrollView);

            toolbarBox.SetFrameSize(new CGSize(Frame.Width, 30));
            scrollView.SetFrameOrigin(new CGPoint(0, 30));
            scrollView.SetFrameSize(new CGSize(Frame.Width, Frame.Height - 30));

            scrollView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable; 

            figmaDelegate = new FigmaDesignerDelegate();
            fileService = new FigmaRemoteFileService();
            codeRenderer = new FigmaCodeRendererService(fileService);

            outlinePanel.DoubleClick += (sender, e) =>
            {
                var node = fileService.NodesProcessed.FirstOrDefault(s => s.FigmaNode == e);
                var code = codeRenderer.GetCode(node.FigmaNode, true);
                SelectCode?.Invoke(this, code);
            };

            outlinePanel.StartDrag += (sender, e) =>
            {
                var entries = new Gtk.TargetEntry[]
                {
                     new Gtk.TargetEntry(e.id, Gtk.TargetFlags.App, TextType)
                };
                DragSourceSet?.Invoke(this, entries);
                DragBegin?.Invoke(this, EventArgs.Empty);
            };

            RefreshUIStates(); 
            // scrollView.SetContentHuggingPriorityForOrientation((int)NSLayoutPriority.DefaultLow, NSLayoutConstraintOrientation.Vertical);
        }

        private void loadAssemblyButton_Activated(object sender, EventArgs e)
        {
            var fullpath = "/Users/jmedrano/FigmaSharp/FigmaSharp.NativeControls/FigmaSharp.NativeControls.Cocoa/bin/Debug";
            List<string> modules = new List<string>();
            modules.Add(fullpath + "/FigmaSharp.NativeControls.dll");
            modules.Add (fullpath + "/FigmaSharp.NativeControls.Cocoa.dll");
            ModulesService.LoadModule(modules.ToArray ());
        }

        public static class ModulesService
        {
            public static List<CustomViewConverter> Converters = new List<CustomViewConverter>();

            public static void LoadModule(params string[] filePath)
            {
                //var path = Path.GetDirectoryName(filePath);

                Dictionary<Assembly, string> instanciableTypes = new Dictionary<Assembly, string>();

                Console.WriteLine("Loading {0}...", string.Join (",",filePath));

                //var enumeratedFiles = Directory.EnumerateFiles(path, "*.dll");
                var enumeratedFiles = filePath;

                foreach (var file in enumeratedFiles)
                {
                    var fileName = Path.GetFileName(file);
                    Console.WriteLine("[{0}] Found.", fileName);
                    try
                    {
                        var assembly = Assembly.LoadFile(file);
                        instanciableTypes.Add(assembly, file);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[{0}] Error loading.", fileName);
                        //Console.WriteLine(ex);
                    }
                }

                foreach (var assemblyTypes in instanciableTypes)
                {
                    try
                    {
                        var interfaceType = typeof(CustomViewConverter);
                        var types = assemblyTypes.Key.GetTypes()
                            .Where(interfaceType.IsAssignableFrom);

                        Console.WriteLine("[{0}] Loaded.", assemblyTypes.Value);
                        foreach (var type in types)
                        {
                            Console.WriteLine("[{0}] Creating instance {1}", assemblyTypes.Key, type);
                            try
                            {
                                if (Activator.CreateInstance(type) is CustomViewConverter element)
                                    Converters.Add(element);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            Console.WriteLine("[{0}] Loaded", type);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                Console.WriteLine("[{0}] Load finished.");
            }
        }

        public ProcessedNode GetProcessedNode (FigmaNode node)
        {
            return fileService.NodesProcessed.FirstOrDefault(s => s.FigmaNode == node);
        }

        public override void SetFrameSize(CGSize newSize)
        {
            base.SetFrameSize(newSize);
            toolbarBox?.SetFrameSize(new CoreGraphics.CGSize(newSize.Width, 30));
            scrollView?.SetFrameOrigin(new CoreGraphics.CGPoint(0, 30));
            scrollView?.SetFrameSize(new CoreGraphics.CGSize(newSize.Width, newSize.Height - 30));
        }

        FigmaRemoteFileService fileService;
        FigmaDesignerDelegate figmaDelegate;
        FigmaCodeRendererService codeRenderer;
        FigmaNodeView data;

        public void RefreshUIStates ()
        {
            fileTextField.Enabled = openFileButton.Enabled = loadAssemblyButton.Enabled = !FigmaSharp.AppContext.Current.IsConfigured;
        }

        void openFileButton_Activated(object sender, EventArgs e)
        {
            fileService.Start(fileTextField.StringValue, processImages: false);
            data = new FigmaNodeView(fileService.Response.document);
            figmaDelegate.ConvertToNodes(fileService.Response.document, data);
            outlinePanel.GenerateTree(data);
        }

        private void OutlinePanel_RaiseFirstResponder(object sender, FigmaNode e)
        {

        }
    }
}