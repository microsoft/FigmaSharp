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
using MonoDevelop.Ide.Gui.Dialogs;
using System.IO;
using System.Text;
using FigmaSharp.Models;

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
        NSScrollView scrollView;
        NSStackView toolbarBox;

        List<(string, string)> Options = new List<(string, string)>()
        {
            ("Cocoa", ModuleService.Platform.MAC), 
            ("iOS" , ModuleService.Platform.iOS),
            ("Gtk" , ModuleService.Platform.Gtk),
            ("WinForms" , ModuleService.Platform.WinForms)
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

            NSPopUpButton exportButton = new NSPopUpButton();

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
            fileProvider = new FigmaRemoteFileProvider();

            SetCodeRenderer(ModuleService.Platform.MAC);

            outlinePanel.DoubleClick += (sender, node) =>
            {
                StringBuilder builder = new StringBuilder();
                codeRenderer.GetCode(builder, node, null, null);
                SelectCode?.Invoke(this, builder.ToString ());
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

            RefreshUIStates(); 
        }

        void SetCodeRenderer (string platform)
        {
            var converters = ModuleService.Converters.Where(s => s.Platform == platform)
              .Select(s => s.Converter)
              .ToArray();

            var addChildConverter = ModuleService.AddChildConverters.FirstOrDefault(s => s.Platform == platform)?.Converter;
            var codePositionConverter = ModuleService.CodePositionConverters.FirstOrDefault(s => s.Platform == platform)?.Converter;
            codeRenderer = new FigmaCodeRendererService(fileProvider, converters, codePositionConverter, addChildConverter);
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

        public void RefreshUIStates ()
        {
            fileTextField.Enabled = openFileButton.Enabled = FigmaSharp.AppContext.Current.IsConfigured;
        }

        void openFileButton_Activated(object sender, EventArgs e)
        {
            if (!FigmaSharp.AppContext.Current.IsConfigured)
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