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
using FigmaSharp;
using System;
using System.Linq;

namespace FigmaSharp.Designer
{
    public class FigmaDesignerSurface
    {
        internal IDesignerDelegate Delegate;

        IScrollViewWrapper scrollViewWrapper;

        FigmaDesignerSession session;
        public FigmaDesignerSession Session
        {
            get
            {
                return session;
            }
            set
            {
                if (session == value)
                    return;

                if (session != null)
                {
                    session.ReloadFinished -= Session_ReloadFinished;
                }

                session = value;
                session.ReloadFinished += Session_ReloadFinished;
            }
        }

        IObjectWrapper nativeObject;
        internal IViewWrapper SelectedView => nativeObject as IViewWrapper;
        IMainWindowWrapper selectedWindow;

        readonly IBorderedWindow viewSelectedOverlayWindow;

        bool IsViewSelected;
        bool IsFirstResponderOverlayVisible
        {
            get => IsViewSelected;
            set
            {
                IsViewSelected = value;

                if (viewSelectedOverlayWindow != null)
                {
                    viewSelectedOverlayWindow.SetParentWindow(selectedWindow);
                    viewSelectedOverlayWindow.Visible = value;
                    if (SelectedView != null)
                    {
                        viewSelectedOverlayWindow.AlignWith(SelectedView);
                    }
                    viewSelectedOverlayWindow.OrderFront();
                }
            }
        }

        public FigmaDesignerSurface()
        {

        }

        public void SetWindow(IMainWindowWrapper selectedWindow)
        {
            if (this.selectedWindow?.NativeObject == selectedWindow?.NativeObject)
            {
                return;
            }

            var needsReattach = selectedWindow != this.selectedWindow;

            if (this.selectedWindow != null)
            {
                Delegate.RemoveAllErrorWindows(this.selectedWindow);
            }

            if (this.selectedWindow != null)
            {
                this.selectedWindow.ResizeRequested -= OnRespositionViews;
                this.selectedWindow.MovedRequested -= OnRespositionViews;
            }

            this.selectedWindow = selectedWindow;
            if (this.selectedWindow == null)
            {
                return;
            }

            RefreshOverlaysVisibility();

            RefreshNeeded();

            this.selectedWindow.ResizeRequested += OnRespositionViews;
            this.selectedWindow.MovedRequested += OnRespositionViews;
            this.selectedWindow.LostFocus += OnRespositionViews;
        }

        void RefreshNeeded()
        {
            //AccessibilityService.Current.ScanErrors(Delegate, selectedWindow, ViewMode);
        }

        void OnRespositionViews(object sender, EventArgs e)
        {
            //var currentWidth = selectedWindow.FrameWidth;
            RefreshOverlaysVisibility();
        }

        void RefreshOverlaysVisibility()
        {
            IsFirstResponderOverlayVisible = IsFirstResponderOverlayVisible;
        }

        void Session_ReloadFinished(object sender, EventArgs e)
        {

            foreach (var items in session.MainViews)
            {
                scrollViewWrapper.AddChild(items.View);
            }

            var mainNodes = session.ProcessedNodes
               .Where(s => s.ParentView == null)
               .ToArray();

            Reposition(mainNodes);

            //we need reload after set the content to ensure the scrollview
            scrollViewWrapper.AdjustToContent();
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

        public void ChangeFocusedView(IObjectWrapper nextView)
        {
            if (selectedWindow == null || nextView == null || SelectedView == nextView)
            {
                //FocusedViewChanged?.Invoke(this, nextView);
                return;
            }

            nativeObject = nextView;

            RefreshWindows();
            RefreshOverlaysVisibility();

            if (SelectedView != null)
            {
                //toolbarWindow.ChangeView(this, SelectedView);
                //FocusedViewChanged?.Invoke(this, SelectedView);
            }
        }

        void RefreshWindows()
        {
            //toolbarWindow.AlignTop(selectedWindow, WindowMargin);
            //inspectorWindow.AlignRight(selectedWindow, WindowMargin);
            //accessibilityWindow.AlignLeft(selectedWindow, WindowMargin);
            //inspectorWindow.GenerateStatusView(SelectedView, Delegate, ViewMode);
        }


    }
}