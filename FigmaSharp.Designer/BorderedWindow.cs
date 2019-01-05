using System;
using AppKit;
using CoreGraphics;

namespace FigmaSharp.Designer
{
    internal class BorderedWindow : WindowWrapper, IBorderedWindow
    {
        readonly NSBox box;
        IViewWrapper ObjContent { get; set; }

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

        public BorderedWindow(IViewWrapper content, NSColor borderColor, NSBorderType borderType = NSBorderType.LineBorder, float borderWidth = 3) : this((content.NativeObject as NSView).Frame, borderColor, NSColor.Clear, borderType, borderWidth)
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
            ContentView = box;
            FillColor = fillColor;
            BorderWidth = borderWidth;
            BorderColor = borderColor;
            BorderType = borderType;
            Level = NSWindowLevel.Floating;
            Visible = false;
        }

        public void SetParentWindow(IWindowWrapper selectedWindow)
        {
            this.ParentWindow = selectedWindow.NativeObject as NSWindow;
        }

        public void AlignWith(IViewWrapper view)
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