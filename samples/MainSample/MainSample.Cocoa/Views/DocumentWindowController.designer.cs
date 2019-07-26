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
    [Register ("DocumentWindowController")]
    partial class DocumentWindowController
    {
        [Outlet]
        AppKit.NSButton CodeButton { get; set; }

        [Outlet]
        AppKit.NSToolbar MainToolbar { get; set; }

        [Outlet]
        AppKit.NSPopUpButton PagePopUpButton { get; set; }

        [Outlet]
        AppKit.NSButton RefreshButton { get; set; }

        [Outlet]
        AppKit.NSToolbarItem Spinner { get; set; }

        [Outlet]
        AppKit.NSTextField TitleTextField { get; set; }

        [Action ("CodeClicked:")]
        partial void CodeClicked (Foundation.NSObject sender);

        [Action ("PageClicked:")]
        partial void PageClicked (Foundation.NSObject sender);

        [Action ("RefreshClicked:")]
        partial void RefreshClicked (Foundation.NSObject sender);
        
        void ReleaseDesignerOutlets ()
        {
            if (MainToolbar != null) {
                MainToolbar.Dispose ();
                MainToolbar = null;
            }

            if (PagePopUpButton != null) {
                PagePopUpButton.Dispose ();
                PagePopUpButton = null;
            }

            if (RefreshButton != null) {
                RefreshButton.Dispose ();
                RefreshButton = null;
            }

            if (CodeButton != null) {
                CodeButton.Dispose ();
                CodeButton = null;
            }

            if (Spinner != null) {
                Spinner.Dispose ();
                Spinner = null;
            }

            if (TitleTextField != null) {
                TitleTextField.Dispose ();
                TitleTextField = null;
            }
        }
    }
}
