using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Linq;
using System.Globalization;
using System.Threading;
using FigmaSharp.Cocoa;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Designer
{
    [Register("WindowWrapper")]
    public class WindowInternalWrapper : IWindowWrapper
    {
        NSWindow window;
        public WindowInternalWrapper (NSWindow window)
        {
            this.window = window;
            Initialize();
        }

        public bool HasParentWindow => window.ParentWindow != null;


        void Initialize()
        {
            window.DidResize += (s, e) => {
                ResizeRequested?.Invoke(this, EventArgs.Empty);
            };

            window.DidMove += (s, e) => {
                MovedRequested?.Invoke(this, EventArgs.Empty);
            };

            window.DidResignKey += (s, e) => {
                LostFocus?.Invoke(this, EventArgs.Empty);
            };
        }

        public object NativeObject => window;

        public void AddChildWindow(IWindowWrapper borderer)
        {
            window.AddChildWindow(borderer.NativeObject as NSWindow, NSWindowOrderingMode.Above);
        }

        public bool ContainsChildWindow(IWindowWrapper debugOverlayWindow)
        {
            return window.ChildWindows.Contains(debugOverlayWindow.NativeObject as NSWindow);
        }

        public IView ContentView
        {
            get
            {
                if (window.ContentView is NSView view)
                {
                    return new View(view);
                }
                return null;
            }
            set
            {
                window.ContentView = value.NativeObject as NSView;
            }
        }

        public IView FirstResponder
        {
            get
            {
                if (window.FirstResponder is NSView view)
                {
                    return new View(view);
                }
                return null;
            }
        }

        public float FrameX => (float)window.Frame.X;
        public float FrameY => (float)window.Frame.Y;
        public float FrameWidth => (float)window.Frame.Width;
        public float FrameHeight => (float)window.Frame.Height;

        public string Title { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Color BackgroundColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Borderless { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Resizable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsFullSizeContentView { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsClosable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Size Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsOpaque { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool MovableByWindowBackground { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ShowCloseButton { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ShowZoomButton { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ShowMiniaturizeButton { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ShowTitle { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsDark { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IView Content { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AlignRight(IWindowWrapper toView, int pixels)
        {
            var toViewWindow = toView.NativeObject as NSWindow;
            var frame = window.Frame;
            frame.Location = new CGPoint(toViewWindow.Frame.Right + pixels, toViewWindow.Frame.Bottom - frame.Height);
            window.SetFrame(frame, true);
        }

        public void AlignLeft(IWindowWrapper toView, int pixels)
        {
            var toViewWindow = toView.NativeObject as NSWindow;
            var frame = window.Frame;
            frame.Location = new CGPoint(toViewWindow.Frame.Left - window.Frame.Width - pixels, toViewWindow.Frame.Bottom - frame.Height);
            window.SetFrame(frame, true);
        }

        public void AlignTop(IWindowWrapper toView, int pixels)
        {
            var toViewWindow = toView.NativeObject as NSWindow;
            var frame = window.Frame;
            frame.Location = new CGPoint(toViewWindow.Frame.Left, toViewWindow.AccessibilityFrame.Y + toViewWindow.Frame.Height + pixels);
            window.SetFrame(frame, true);
        }

        public void SetTitle(string v)
        {
            window.Title = v;
        }

        public void SetContentSize(int toolbarWindowWidth, int toolbarWindowHeight)
        {
            window.SetContentSize(new CGSize(toolbarWindowWidth, toolbarWindowHeight));
        }

        public void SetAppareance(bool isDark)
        {
            window.Appearance = NSAppearance.GetAppearance(isDark ? NSAppearance.NameVibrantDark : NSAppearance.NameVibrantLight);
        }

        public void RecalculateKeyViewLoop()
        {
            window.RecalculateKeyViewLoop();
        }

        public void Close()
        {
            window.Close();
        }

        public void AddChild(IWindow window)
        {
            throw new NotImplementedException();
        }

        public void RemoveChild(IWindow window)
        {
            throw new NotImplementedException();
        }

        public void ShowDialog()
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            throw new NotImplementedException();
        }

        public void Center()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public event EventHandler ResizeRequested;
        public event EventHandler MovedRequested;
        public event EventHandler LostFocus;
    }

    [Register("WindowWrapper")]
    public class WindowWrapper : NSWindow, IWindowWrapper
    {
        public WindowWrapper()
        {
            Initialize();
        }

        public bool HasParentWindow => base.ParentWindow != null;

        public WindowWrapper(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        public WindowWrapper(CGRect contentRect, NSWindowStyle aStyle, NSBackingStore bufferingType, bool deferCreation) : base(contentRect, aStyle, bufferingType, deferCreation)
        {
            Initialize();
        }

        public WindowWrapper(CGRect contentRect, NSWindowStyle aStyle, NSBackingStore bufferingType, bool deferCreation, NSScreen screen) : base(contentRect, aStyle, bufferingType, deferCreation, screen)
        {
            Initialize();
        }

        protected WindowWrapper(NSObjectFlag t) : base(t)
        {
            Initialize();
        }

        protected internal WindowWrapper(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        void Initialize()
        {
            DidResize += (s, e) => {
                ResizeRequested?.Invoke(this, EventArgs.Empty);
            };

            DidMove += (s, e) => {
                MovedRequested?.Invoke(this, EventArgs.Empty);
            };

            base.DidResignKey += (s, e) => {
                LostFocus?.Invoke(this, EventArgs.Empty);
            };
        }

        public object NativeObject => this;

        public void AddChildWindow(IWindowWrapper borderer)
        {
            base.AddChildWindow(borderer.NativeObject as NSWindow, NSWindowOrderingMode.Above);
        }

        public bool ContainsChildWindow(IWindowWrapper debugOverlayWindow)
        {
            return this.ChildWindows.Contains(debugOverlayWindow.NativeObject as NSWindow);
        }

        public IView ContentView
        {
            get
            {
                if (ContentView is NSView view)
                {
                    return new View(view);
                }
                return null;
            }
            set
            {
                ContentView = value;
            }
        }

        public IView FirstResponder
        {
            get
            {
                if (FirstResponder is NSView view)
                {
                    return new View(view);
                }
                return null;
            }
        }

        public float FrameX => (float)Frame.X;
        public float FrameY => (float)Frame.Y;
        public float FrameWidth => (float)Frame.Width;
        public float FrameHeight => (float)Frame.Height;

        public void AlignRight(IWindowWrapper toView, int pixels)
        {
            var toViewWindow = toView.NativeObject as NSWindow;
            var frame = Frame;
            frame.Location = new CGPoint(toViewWindow.Frame.Right + pixels, toViewWindow.Frame.Bottom - frame.Height);
            SetFrame(frame, true);
        }

        public void AlignLeft(IWindowWrapper toView, int pixels)
        {
            var toViewWindow = toView.NativeObject as NSWindow;
            var frame = Frame;
            frame.Location = new CGPoint(toViewWindow.Frame.Left - Frame.Width - pixels, toViewWindow.Frame.Bottom - frame.Height);
            SetFrame(frame, true);
        }

        public void AlignTop(IWindowWrapper toView, int pixels)
        {
            var toViewWindow = toView.NativeObject as NSWindow;
            var frame = Frame;
            frame.Location = new CGPoint(toViewWindow.Frame.Left, toViewWindow.AccessibilityFrame.Y + toViewWindow.Frame.Height + pixels);
            SetFrame(frame, true);
        }

        public void SetTitle(string v)
        {
            Title = v;
        }

        public void SetContentSize(int toolbarWindowWidth, int toolbarWindowHeight)
        {
            base.SetContentSize(new CGSize(toolbarWindowWidth, toolbarWindowHeight));
        }

        public void SetAppareance(bool isDark)
        {
            base.Appearance = NSAppearance.GetAppearance(isDark ? NSAppearance.NameVibrantDark : NSAppearance.NameVibrantLight);
        }

        public event EventHandler ResizeRequested;
        public event EventHandler MovedRequested;
        public event EventHandler LostFocus;
    }
}