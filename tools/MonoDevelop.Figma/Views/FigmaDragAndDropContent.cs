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
using System.Linq;
using System.Text;
using AppKit;
using CoreGraphics;
using FigmaSharp;
using FigmaSharp.Controls.Services;
using FigmaSharp.Designer;
using FigmaSharp.Models;
using FigmaSharp.Services;
using MonoDevelop.Ide;

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
        NSButton openFileButton;

		public string GetCode (FigmaNode selectedNode)
		{
            StringBuilder builder = new StringBuilder ();
            codeRenderer.GetCode (builder, new CodeNode (selectedNode, null), null);
            return builder.ToString ();
        }

		NSScrollView scrollView;
        NSStackView toolbarBox;
        NSPopUpButton exportButton;
        List<(string, string)> Options = new List<(string, string)>()
        {
            ("Cocoa", ModuleService.Platform.MAC), 
            //("iOS" , ModuleService.Platform.iOS),
            //("Gtk" , ModuleService.Platform.Gtk),
            //("WinForms" , ModuleService.Platform.WinForms)
        };

        public FigmaDragAndDropContent()
        {
            toolbarBox = ViewHelpers.CreateHorizontalStackView();
            toolbarBox.EdgeInsets = new NSEdgeInsets(0, 10, 0, 10);
            AddSubview(toolbarBox);
            toolbarBox.AutoresizingMask = NSViewResizingMask.WidthSizable;
            toolbarBox.WantsLayer = true;
            fileTextField = new NSTextField();
            fileTextField.PlaceholderString = "Type the document id to load";
            fileTextField.StringValue = "Dq1CFm7IrDi3UJC7KJ8zVjOt";
            toolbarBox.AddArrangedSubview(fileTextField);

            openFileButton = new NSButton() { Title = "" };
            openFileButton.BezelStyle = NSBezelStyle.RoundRect;
            toolbarBox.AddArrangedSubview(openFileButton);

            openFileButton.Activated += openFileButton_Activated;

            exportButton = new NSPopUpButton();

            var icon = (NSImage) FigmaSharp.AppContext.Current.GetImageFromManifest(this.GetType().Assembly, "pe-path-reveal@2x").NativeObject;
            openFileButton.Image = icon;

            foreach (var item in Options)
            {
                exportButton.AddItem(item.Item1);
            }
           
            toolbarBox.AddArrangedSubview(exportButton);

            outlinePanel = new OutlinePanel();
            scrollView = outlinePanel.EnclosingScrollView;
            outlinePanel.RaiseFirstResponder += OutlinePanel_RaiseFirstResponder;

            AddSubview(scrollView);

            toolbarBox.SetFrameSize(new CGSize(Frame.Width, 30));
            scrollView.SetFrameOrigin(new CGPoint(0, 30));
            scrollView.SetFrameSize(new CGSize(Frame.Width, Frame.Height - 30));

            scrollView.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable; 

            figmaDelegate = new FigmaDesignerDelegate();
            fileProvider = new ControlsRemoteFileProvider();

            SetCocoaCodeRenderer ();
            //SetCodeRenderer(ModuleService.Platform.MAC);

            outlinePanel.DoubleClick += (sender, node) =>
            {
                var code = GetCode (node);
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

            exportButton.Activated += (sender, e) =>
            {
                var item = Options[(int)exportButton.IndexOfSelectedItem];
                SetCodeRenderer(item.Item2);
            };

            RefreshEnabledStatement ();

            FigmaRuntime.TokenChanged += FigmaRuntime_TokenChanged;
        }

		protected override void Dispose (bool disposing)
		{
            FigmaRuntime.TokenChanged -= FigmaRuntime_TokenChanged;
            base.Dispose (disposing);
		}

		private void FigmaRuntime_TokenChanged (object sender, EventArgs e)
		{
            RefreshEnabledStatement ();
        }

		internal void RefreshEnabledStatement () => exportButton.Enabled = fileTextField.Enabled = openFileButton.Enabled = FigmaSharp.AppContext.Current.IsApiConfigured;

        void SetCodeRenderer (string platform)
        {
            var converters = ModuleService.Converters.Where(s => s.Platform == platform)
              .Select(s => s.Converter)
              .ToArray();

            var codePropertyConverters = ModuleService.CodePropertyConverters.FirstOrDefault(s => s.Platform == platform)?.Converter;
            codeRenderer = new FigmaCodeRendererService (fileProvider, converters, codePropertyConverters);
        }

		void SetCocoaCodeRenderer ()
		{
            var converters = FigmaControlsContext.Current.GetConverters ();
            var codePropertyConverter = FigmaControlsContext.Current.GetCodePropertyConverter ();
            codeRenderer = new NativeViewCodeService (fileProvider, converters, codePropertyConverter);
        }

        public override void SetFrameSize(CGSize newSize)
        {
            base.SetFrameSize(newSize);
            toolbarBox?.SetFrameSize(new CoreGraphics.CGSize(newSize.Width, 30));
            scrollView?.SetFrameOrigin(new CoreGraphics.CGPoint(0, 30));
            scrollView?.SetFrameSize(new CoreGraphics.CGSize(newSize.Width, newSize.Height - 30));
        }

        FigmaRemoteFileProvider fileProvider;
        FigmaDesignerDelegate figmaDelegate;
        FigmaCodeRendererService codeRenderer;
        FigmaNodeView data;


        void openFileButton_Activated(object sender, EventArgs e)
        {
            if (FigmaSharp.AppContext.Current.IsApiConfigured)
            {
                MessageService.ShowError("Figma API is not configured");
                return;
            }

            fileProvider.Load(fileTextField.StringValue);
            data = new FigmaNodeView(fileProvider.Response.document);
            figmaDelegate.ConvertToNodes(fileProvider.Response.document, data);
            outlinePanel.GenerateTree(data);
        }

        private void OutlinePanel_RaiseFirstResponder(object sender, FigmaNode e)
        {

        }
    }
}