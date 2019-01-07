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
using FigmaSharp;
using FigmaSharp.Designer;
using Xwt.GtkBackend;

namespace MonoDevelop.Figma
{
    class FigmaDesignerPropertyPad : Gtk.VBox
    {
        static FigmaDesignerPropertyPad instance;
        bool isInitialized;

        public static FigmaDesignerPropertyPad Instance
        {
            get
            {
                if (instance == null)
                    throw new InvalidOperationException("Needs to be initialized first");
                return instance;
            }
        }

        public static void Initialize(FigmaDesignerSession service)
        {
            if (instance == null)
            {
                instance = new FigmaDesignerPropertyPad(service);
            }
        }

        public void Select(object data)
        {
            propertyPanel.Select(data);
        }

        FigmaPropertyPanel propertyPanel;

        private FigmaDesignerPropertyPad(FigmaDesignerSession service)
        {
            propertyPanel = new FigmaPropertyPanel();
            var widget = GtkMacInterop.NSViewToGtkWidget(propertyPanel.View);
            PackStart(widget, true, true, 0);

            ShowAll();
        }

        public void Initialize()
        {
            if (isInitialized)
                return;
            isInitialized = true;
            propertyPanel.Initialize();
        }
    }
}