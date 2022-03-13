    //using System;
    //using AppKit;
    //using System.Linq;
    //using System.Collections.Generic;
    //using Foundation;
    //using CoreGraphics;

    //namespace FigmaSharp.Designer
    //{
    //    public class DesignerDelegate : IDesignerDelegate
    //    {
    //        public DesignerDelegate()
    //        {
    //        }

    //        public void RemoveAllErrorWindows(IWindowWrapper windowWrapper)
    //        {
    //            var window = windowWrapper.NativeObject as NSWindow;
    //            var childWindro = window.ChildWindows.OfType<BorderedWindow>();
    //            foreach (var item in childWindro)
    //            {
    //                item.Close();
    //            }
    //        }


    //        #region Hover selection

    //        public void StartHoverSelection(IWindowWrapper currentWindow)
    //        {
    //            StopHoverSelection();

    //            var nativeWindow = currentWindow.NativeObject as NSWindow;

    //            endSelection = false;

    //            clickMonitor = NSEvent.AddLocalMonitorForEventsMatchingMask(NSEventMask.LeftMouseDown, (NSEvent theEvent) => {
    //                StopHoverSelection();
    //                var selected = GetHoverSelectedView();
    //                if (selected != null)
    //                {
    //                    HoverSelectionEnded?.Invoke(this, new ViewWrapper(selected));
    //                }
    //                else
    //                {
    //                    HoverSelectionEnded?.Invoke(this, null);
    //                }

    //                return null;
    //            });

    //            moveMonitor = NSEvent.AddLocalMonitorForEventsMatchingMask(NSEventMask.MouseMoved, (NSEvent theEvent) => {
    //                if (endSelection)
    //                {
    //                    return null;
    //                }
    //                var point = nativeWindow.ConvertBaseToScreen(theEvent.LocationInWindow);
    //                if (!nativeWindow.AccessibilityFrame.Contains(point))
    //                {
    //                    return null;
    //                }
    //                containerViews.Clear();
    //                AddContainerViews(nativeWindow.ContentView, point, containerViews);

    //                if (containerViews.Count > 0)
    //                {
    //                    index = containerViews.Count - 1;
    //                }
    //                else
    //                {
    //                    index = -1;
    //                }

    //                var selectedView = GetHoverSelectedView();
    //                if (selectedView != null)
    //                {
    //                    HoverSelecting?.Invoke(this, new ViewWrapper(selectedView));
    //                }
    //                return null;
    //            });
    //        }

    //        static void AddContainerViews(NSView view, CGPoint point, List<NSView> containerViews)
    //        {
    //            if (view.AccessibilityFrame.Contains(point))
    //            {
    //                containerViews.Add(view);
    //            }
    //            else
    //            {
    //                return;
    //            }

    //            if (view.Subviews == null)
    //            {
    //                return;
    //            }

    //            foreach (var item in view.Subviews)
    //            {
    //                try
    //                {
    //                    AddContainerViews(item, point, containerViews);
    //                }
    //                catch (Exception ex)
    //                {
    //                    Console.WriteLine(ex);
    //                }
    //            }
    //        }

    //        public void DeepHoverSelection()
    //        {
    //            if (endSelection)
    //            {
    //                return;
    //            }
    //            if (index == 0)
    //            {
    //                return;
    //            }
    //            index--;
    //            var selectedView = GetHoverSelectedView();
    //            if (selectedView != null)
    //            {
    //                HoverSelecting?.Invoke(this, new ViewWrapper(selectedView));
    //            }
    //        }

    //        public void PreviousHoverSelection()
    //        {
    //            if (endSelection)
    //            {
    //                return;
    //            }
    //            if (index >= containerViews.Count - 2)
    //            {
    //                return;
    //            }
    //            index++;
    //            var selectedView = GetHoverSelectedView();
    //            if (selectedView != null)
    //            {
    //                HoverSelecting?.Invoke(this, new MacViewWrapper(selectedView));
    //            }

    //        }

    //        public void StopHoverSelection()
    //        {
    //            endSelection = true;
    //            if (clickMonitor != null)
    //            {
    //                NSEvent.RemoveMonitor(clickMonitor);
    //                clickMonitor = null;
    //            }

    //            if (moveMonitor != null)
    //            {
    //                NSEvent.RemoveMonitor(moveMonitor);
    //                moveMonitor = null;
    //            }
    //        }

    //        int index;
    //        bool endSelection;
    //        NSObject clickMonitor, moveMonitor;
    //        List<NSView> containerViews = new List<NSView>();

    //        NSView GetHoverSelectedView() => index == -1 || index >= containerViews.Count ? null : containerViews[index];

    //        public event EventHandler<IViewWrapper> HoverSelecting;
    //        public event EventHandler<IViewWrapper> HoverSelectionEnded;

    //        #endregion

    //    }
    //}
