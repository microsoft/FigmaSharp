// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace FigmaSharp.Samples
{
    [Register ("MyWindowController")]
    partial class MyWindowController
    {
        [Outlet]
        AppKit.NSPopUpButton PagePopUpButton { get; set; }

        [Outlet]
        AppKit.NSButton RefreshButton { get; set; }

        [Outlet]
        AppKit.NSTextField TitleTextField { get; set; }
        
        void ReleaseDesignerOutlets ()
        {
            if (PagePopUpButton != null) {
                PagePopUpButton.Dispose ();
                PagePopUpButton = null;
            }

            if (RefreshButton != null) {
                RefreshButton.Dispose ();
                RefreshButton = null;
            }

            if (TitleTextField != null) {
                TitleTextField.Dispose ();
                TitleTextField = null;
            }
        }
    }
}
