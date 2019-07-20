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
	[Register ("DocumentViewController")]
	partial class DocumentViewController
	{
		[Outlet]
		AppKit.NSScrollView MainScrollView { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator Spinner { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Spinner != null) {
				Spinner.Dispose ();
				Spinner = null;
			}

			if (MainScrollView != null) {
				MainScrollView.Dispose ();
				MainScrollView = null;
			}
		}
	}
}
