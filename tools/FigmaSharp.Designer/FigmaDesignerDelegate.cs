using System;
using AppKit;
using System.Linq;
using System.Collections.Generic;
using Foundation;
using CoreGraphics;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Designer
{
    public class FigmaDesignerDelegate : IFigmaDesignerDelegate
    {
        public void RemoveAllErrorWindows(IWindowWrapper windowWrapper)
        {
            var window = windowWrapper.NativeObject as NSWindow;
            var childWindro = window.ChildWindows.OfType<BorderedWindow>();
            foreach (var item in childWindro)
            {
                item.Close();
            }
        }

        #region Hover selection

        public void StartHoverSelection(IWindowWrapper currentWindow)
        {
            StopHoverSelection();

            clickMonitor = NSEvent.AddLocalMonitorForEventsMatchingMask(NSEventMask.LeftMouseDown, (NSEvent theEvent) => {

				if (currentWindow == null) {
                    return theEvent;
				}

                var nativeWindow = currentWindow.NativeObject as NSWindow;

                if (theEvent.Window != nativeWindow)
                {
                    return theEvent;
                }

                var point = nativeWindow.ConvertPointToScreen(theEvent.LocationInWindow);//nativeWindow.ConvertBaseToScreen();

                if (!nativeWindow.AccessibilityFrame.Contains(point))
                {
                    return theEvent;
                }
                containerViews.Clear();
                AddContainerViews(nativeWindow.ContentView, point, containerViews);

                if (containerViews.Count > 0)
                {
                    index = containerViews.Count - 1;
                }
                else
                {
                    index = -1;
                }
                //StopHoverSelection();
                var selected = GetHoverSelectedView();
                if (selected != null)
                {
                    HoverSelectionEnded?.Invoke(this, new View(selected));
                }
                else
                {
                    HoverSelectionEnded?.Invoke(this, null);
                }

                return theEvent;
            });
        }

        static void AddContainerViews(NSView view, CGPoint point, List<NSView> containerViews)
        {
            if (view.AccessibilityFrame.Contains(point))
            {
                containerViews.Add(view);
            }
            else
            {
                return;
            }

            if (view.Subviews == null)
            {
                return;
            }

            foreach (var item in view.Subviews)
            {
                try
                {
                    AddContainerViews(item, point, containerViews);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public void DeepHoverSelection()
        {
            if (index == 0)
            {
                return;
            }
            index--;
            var selectedView = GetHoverSelectedView();
            if (selectedView != null)
            {
                HoverSelecting?.Invoke(this, new View(selectedView));
            }
        }

        public void PreviousHoverSelection()
        {
            if (index >= containerViews.Count - 2)
            {
                return;
            }
            index++;
            var selectedView = GetHoverSelectedView();
            if (selectedView != null)
            {
                HoverSelecting?.Invoke(this, new View(selectedView));
            }
        }

        public void StopHoverSelection()
        {
            if (clickMonitor != null)
            {
                NSEvent.RemoveMonitor(clickMonitor);
                clickMonitor = null;
            }

            //if (moveMonitor != null)
            //{
            //    NSEvent.RemoveMonitor(moveMonitor);
            //    moveMonitor = null;
            //}
        }

        int index;
        NSObject clickMonitor;
        List<NSView> containerViews = new List<NSView>();

        NSView GetHoverSelectedView() => index == -1 || index >= containerViews.Count ? null : containerViews[index];

        public IBorderedWindow CreateOverlayWindow()
        {
            return new BorderedWindow(CGRect.Empty, NSColor.Blue);
        }

        public void ConvertToNodes(FigmaNode figmaNode, FigmaNodeView node)
        {

            var current = new FigmaNodeView(figmaNode);
            node.AddChild(current);

            if (figmaNode is FigmaDocument document)
            {
                if (document.children != null)
                {
                    foreach (var item in document.children)
                    {
                        try
                        {
                            ConvertToNodes(item, current);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                return;
            }

            if (figmaNode is IFigmaNodeContainer container)
            {
                foreach (var item in container.children)
                {
                    try
                    {
                        ConvertToNodes(item, current);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        public event EventHandler<IView> HoverSelecting;
        public event EventHandler<IView> HoverSelectionEnded;

        #endregion

    }
}
