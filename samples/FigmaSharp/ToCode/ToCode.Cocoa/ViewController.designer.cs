// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ToCode.Cocoa
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton translateButton { get; set; }

		[Outlet]
		AppKit.NSButton copyDesignerCSButton { get; set; }

		[Outlet]
		AppKit.NSButton copyCSButton { get; set; }

		[Outlet]
		AppKit.NSButton openUrlButton { get; set; }

		[Outlet]
		AppKit.NSScrollView logTextField { get; set; }

		[Outlet]
		AppKit.NSView treeHierarchyContainer { get; set; }

		[Outlet]
		AppKit.NSTextField urlTextField { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (translateButton != null)
			{
				translateButton.Dispose();
				translateButton = null;
			}

			if (openUrlButton != null) {
				openUrlButton.Dispose ();
				openUrlButton = null;
			}

			if (copyDesignerCSButton != null) {
				copyDesignerCSButton.Dispose ();
				copyDesignerCSButton = null;
			}

			if (copyCSButton != null) {
				copyCSButton.Dispose ();
				copyCSButton = null;
			}

			if (logTextField != null) {
				logTextField.Dispose ();
				logTextField = null;
			}

			if (treeHierarchyContainer != null) {
				treeHierarchyContainer.Dispose ();
				treeHierarchyContainer = null;
			}

			if (urlTextField != null)
			{
				urlTextField.Dispose();
				urlTextField = null;
			}
		}
	}
}
