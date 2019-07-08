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
	[Register ("OpenLocationViewController")]
	partial class OpenLocationViewController
	{
		[Outlet]
		AppKit.NSButton CancelButton { get; set; }

		[Outlet]
		AppKit.NSComboBox LinkComboBox { get; set; }

		[Outlet]
		AppKit.NSButton OpenButton { get; set; }

        [Outlet]
        AppKit.NSView OpenLocationWindow { get; set; }

		[Outlet]
		AppKit.NSTextField TokenStatusTextField { get; set; }

		[Outlet]
		AppKit.NSTextField TokenTextField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}

			if (LinkComboBox != null) {
				LinkComboBox.Dispose ();
				LinkComboBox = null;
			}

			if (OpenButton != null) {
				OpenButton.Dispose ();
				OpenButton = null;
			}

			if (OpenLocationWindow != null) {
				OpenLocationWindow.Dispose ();
				OpenLocationWindow = null;
			}

			if (TokenTextField != null) {
				TokenTextField.Dispose ();
				TokenTextField = null;
			}

			if (TokenStatusTextField != null) {
				TokenStatusTextField.Dispose ();
				TokenStatusTextField = null;
			}
		}
	}
}
