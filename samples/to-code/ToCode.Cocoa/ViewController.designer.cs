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
		AppKit.NSScrollView logTextField { get; set; }

		[Outlet]
		AppKit.NSView treeHierarchyContainer { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (logTextField != null) {
				logTextField.Dispose ();
				logTextField = null;
			}

			if (treeHierarchyContainer != null) {
				treeHierarchyContainer.Dispose ();
				treeHierarchyContainer = null;
			}
		}
	}
}
