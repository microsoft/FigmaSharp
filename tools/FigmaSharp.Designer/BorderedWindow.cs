// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

using AppKit;
using CoreGraphics;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Designer
{
    internal class BorderedWindow : WindowWrapper, IBorderedWindow
    {
        readonly NSBox box;
        IView ObjContent { get; set; }

        public NSColor BorderColor
        {
            get => box.BorderColor;
            set => box.BorderColor = value;
        }

        public NSColor FillColor
        {
            get => box.FillColor;
            set
            {
                BackgroundColor = value;
                box.FillColor = value;
            }
        }

        public float BorderWidth
        {
            get => (float)box.BorderWidth;
            set => box.BorderWidth = value;
        }

        public NSBorderType BorderType
        {
            get => box.BorderType;
            set => box.BorderType = value;
        }

        public bool Visible
        {
            get => !box.Transparent;
            set
            {
                box.Transparent = !value;
            }
        }

        public string ContentViewIdentifier => ObjContent?.Identifier ?? "";

        public BorderedWindow(IntPtr handle) : base(handle)
        {

        }

        public BorderedWindow(IView content, NSColor borderColor, NSBorderType borderType = NSBorderType.LineBorder, float borderWidth = 3) : this((content.NativeObject as NSView).Frame, borderColor, NSColor.Clear, borderType, borderWidth)
        {
            ObjContent = content;
        }

        public BorderedWindow(CGRect frame, NSColor borderColor, NSBorderType borderType = NSBorderType.LineBorder, float borderWidth = 3) : this(frame, borderColor, NSColor.Clear, borderType, borderWidth)
        {

        }

        public BorderedWindow(CGRect frame, NSColor borderColor, NSColor fillColor, NSBorderType borderType = NSBorderType.LineBorder, float borderWidth = 3) : base(frame, NSWindowStyle.Borderless, NSBackingStore.Buffered, false)
        {
            IsOpaque = false;
            ShowsToolbarButton = false;
            IgnoresMouseEvents = true;
            box = new NSBox { BoxType = NSBoxType.NSBoxCustom };
            Content = new View (box);
            FillColor = fillColor;
            BorderWidth = borderWidth;
            BorderColor = borderColor;
            BorderType = borderType;
            Level = NSWindowLevel.Floating;
            Visible = false;
        }

        public void SetParentWindow(IWindowWrapper selectedWindow)
        {
            try
            {
                this.ParentWindow = selectedWindow.NativeObject as NSWindow;
            }
            catch (Exception ex)
            {
                LoggingService.LogError("BorderedWindow.SetParentWindow error", ex);
            }
        }

        public void AlignWith(IView view)
        {
            var frame = (view.NativeObject as NSView).AccessibilityFrame;
            SetFrame(frame, true);
        }

        public void AlignWindowWithContentView()
        {
            if (ObjContent != null)
            {
                AlignWith(ObjContent);
            }
        }

        public void OrderFront()
        {
            base.OrderFront(null);
        }
    }
}