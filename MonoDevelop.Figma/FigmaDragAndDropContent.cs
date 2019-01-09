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

using CoreGraphics;
using FigmaSharp;
using FigmaSharp.Designer;
using FigmaSharp.Services;

namespace MonoDevelop.Figma
{
    class FigmaDragAndDropContent : NSView
    {
        public event EventHandler<string> SelectCode;
        public event EventHandler DragBegin;
        public event EventHandler DragSourceUnset;
        public event EventHandler<Gtk.TargetEntry[]> DragSourceSet;

        public FigmaNode SelectedNode => (outlinePanel.View.SelectedNode as FigmaNodeView)?.Wrapper;

        public static NSStackView CreateHorizontalStackView(int spacing = 10) => new NSStackView()
        {
            Orientation = NSUserInterfaceLayoutOrientation.Horizontal,
            Alignment = NSLayoutAttribute.CenterY,
            Spacing = spacing,
            Distribution = NSStackViewDistribution.Fill,
        };

        public override bool IsFlipped => true;
        public const int TextType = 1;
        OutlinePanel outlinePanel;
        NSTextField fileTextField;
        NSButton button;
        NSScrollView scrollView;
        NSStackView toolbarBox;

        public FigmaDragAndDropContent()
        {
            toolbarBox = CreateHorizontalStackView();
            toolbarBox.EdgeInsets = new NSEdgeInsets(0, 10, 0, 10);
            AddSubview(toolbarBox);
            toolbarBox.AutoresizingMask = NSViewResizingMask.WidthSizable;
            toolbarBox.WantsLayer = true;
            fileTextField = new NSTextField();
            fileTextField.StringValue = "UeIJu6C1IQwPkdOut2IWRgGd";
            toolbarBox.AddArrangedSubview(fileTextField);

            button = new NSButton() { Title = "Open" };
            button.BezelStyle = NSBezelStyle.RoundRect;
            toolbarBox.AddArrangedSubview(button);

            button.Activated += Button_Activated;

            outlinePanel = new OutlinePanel();
            scrollView = outlinePanel.EnclosingScrollView;
            outlinePanel.RaiseFirstResponder += OutlinePanel_RaiseFirstResponder;

            AddSubview(scrollView);

            toolbarBox.SetFrameSize(new CoreGraphics.CGSize(Frame.Width, 30));
            scrollView.SetFrameOrigin(new CoreGraphics.CGPoint(0, 30));
            scrollView.SetFrameSize(new CoreGraphics.CGSize(Frame.Width, Frame.Height - 30));

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
            // scrollView.SetContentHuggingPriorityForOrientation((int)NSLayoutPriority.DefaultLow, NSLayoutConstraintOrientation.Vertical);
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

        void Button_Activated(object sender, EventArgs e)
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