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
using System.Linq;
using System.Collections.Generic;

using AppKit;
using Foundation;
using CoreGraphics;

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
