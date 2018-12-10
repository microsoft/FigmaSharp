// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace FigmaDocumentExporter
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTextField documentTextField { get; set; }

		[Outlet]
		AppKit.NSTextField outputPathTextField { get; set; }

		[Action ("DownloadAction:")]
		partial void DownloadAction (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (documentTextField != null) {
				documentTextField.Dispose ();
				documentTextField = null;
			}

			if (outputPathTextField != null) {
				outputPathTextField.Dispose ();
				outputPathTextField = null;
			}
		}
	}
}
