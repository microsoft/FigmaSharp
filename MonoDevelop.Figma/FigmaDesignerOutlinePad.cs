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
using FigmaSharp.Designer;
using Xwt.GtkBackend;

namespace MonoDevelop.Figma
{
    public class FigmaDesignerOutlinePad : Gtk.VBox
    {
        static FigmaDesignerOutlinePad instance;

        public static FigmaDesignerOutlinePad Instance
        {
            get
            {
                if (instance == null)
                    instance = new FigmaDesignerOutlinePad();
                return instance;
            }
        }

        OutlinePanel outlinePanel;
        public FigmaDesignerOutlinePad()
        {
            outlinePanel = new OutlinePanel();
      
            var widget = GtkMacInterop.NSViewToGtkWidget(outlinePanel.EnclosingScrollView);
            CanFocus = widget.CanFocus = true;
            Sensitive = widget.Sensitive = true;

            widget.Focused += FigmaDesignerOutlinePad_Focused;

            PackStart(widget, true, true, 0);
            ShowAll();

            Focused += FigmaDesignerOutlinePad_Focused;
        }

        void FigmaDesignerOutlinePad_Focused(object o, Gtk.FocusedArgs args)
        {
            outlinePanel.FocusSelectedView();
        }


        public void GenerateTree(Node node)
        {
            outlinePanel.GenerateTree(node);
        }
    }

}