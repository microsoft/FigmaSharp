// This file was auto-generated using
// FigmaSharp 1.0.0.0 and Figma API 1.0.1 on 2020-04-16 at 18:21
//
// Document title:   
// Document version: 0.1f
// Document URL:     FwVa4JS5QsohRhNEnEBKslFk
// Namespace:        FigmaSharp
//
// Changes to this file may cause incorrect behavior
// and will be lost if the code is regenerated.

using AppKit;

namespace MonoDevelop.Figma.Packages
{
	partial class PackageUpdateWindow
	{
		private AppKit.NSButton updateButton, cancelButton, translationsCheckbox;
		private AppKit.NSProgressIndicator versionSpinner;
		private AppKit.NSPopUpButton bundlePopUp, versionPopUp;

		private void InitializeComponent ()
		{
			this.Title = "Update Figma Package";
			this.StyleMask |= NSWindowStyle.Closable;
			this.StandardWindowButton (NSWindowButton.ZoomButton).Enabled = false;

			var frame = Frame;
			frame.Size = new CoreGraphics.CGSize (455f, 206f);
			this.SetFrame (frame, true);
			this.ContentMinSize = this.ContentView.Frame.Size;

			// View:     updateButton
			// NodeName: "updateButton"
			// NodeType: INSTANCE
			// NodeId:   775:30
			updateButton = new AppKit.NSButton();
			updateButton.WantsLayer = true;
			updateButton.BezelStyle = NSBezelStyle.Rounded;
			updateButton.ControlSize = NSControlSize.Regular;
			updateButton.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			updateButton.Title = "Update";
			updateButton.KeyEquivalent = "\r";

			this.ContentView.AddSubview (updateButton);
			updateButton.Frame = updateButton.GetFrameForAlignmentRect (new CoreGraphics.CGRect (352f, 20f, 84f, 21f));;

			// View:     cancelButton
			// NodeName: "cancelButton"
			// NodeType: INSTANCE
			// NodeId:   775:45
			cancelButton = new AppKit.NSButton();
			cancelButton.WantsLayer = true;
			cancelButton.BezelStyle = NSBezelStyle.Rounded;
			cancelButton.ControlSize = NSControlSize.Regular;
			cancelButton.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			cancelButton.Title = "Cancel";

			this.ContentView.AddSubview (cancelButton);
			cancelButton.Frame = cancelButton.GetFrameForAlignmentRect (new CoreGraphics.CGRect (257f, 20f, 84f, 21f));;

			// View:     versionSpinner
			// NodeName: "versionSpinner"
			// NodeType: INSTANCE
			// NodeId:   772:40
			versionSpinner = new AppKit.NSProgressIndicator();
			versionSpinner.WantsLayer = true;
			versionSpinner.Style = NSProgressIndicatorStyle.Spinning;
			versionSpinner.Hidden = true;
			versionSpinner.ControlSize = NSControlSize.Small;

			this.ContentView.AddSubview (versionSpinner);
			versionSpinner.Frame = versionSpinner.GetFrameForAlignmentRect (new CoreGraphics.CGRect (383f, 99f, 18f, 18f));;

			// View:     nativeViewCodeServiceView
			// NodeName: Generate:
			// NodeType: INSTANCE
			// NodeId:   772:120
			var nativeViewCodeServiceView = new AppKit.NSTextField();
			nativeViewCodeServiceView.Editable = false;
			nativeViewCodeServiceView.Bordered = false;
			nativeViewCodeServiceView.Bezeled = false;
			nativeViewCodeServiceView.DrawsBackground = false;
			nativeViewCodeServiceView.StringValue = "Figma Package:";
			nativeViewCodeServiceView.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			nativeViewCodeServiceView.WantsLayer = true;
			nativeViewCodeServiceView.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (nativeViewCodeServiceView);
			nativeViewCodeServiceView.Frame = nativeViewCodeServiceView.GetFrameForAlignmentRect (new CoreGraphics.CGRect (8f, 131f, 142f, 20f));;

			// View:     nativeViewCodeServiceView1
			// NodeName: Generate:
			// NodeType: INSTANCE
			// NodeId:   772:124
			var nativeViewCodeServiceView1 = new AppKit.NSTextField();
			nativeViewCodeServiceView1.Editable = false;
			nativeViewCodeServiceView1.Bordered = false;
			nativeViewCodeServiceView1.Bezeled = false;
			nativeViewCodeServiceView1.DrawsBackground = false;
			nativeViewCodeServiceView1.StringValue = "Update to:";
			nativeViewCodeServiceView1.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			nativeViewCodeServiceView1.WantsLayer = true;
			nativeViewCodeServiceView1.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (nativeViewCodeServiceView1);
			nativeViewCodeServiceView1.Frame = nativeViewCodeServiceView1.GetFrameForAlignmentRect (new CoreGraphics.CGRect (8f, 98f, 142f, 20f));;

			// View:     bundlePopUp
			// NodeName: "bundlePopUp"
			// NodeType: INSTANCE
			// NodeId:   765:0
			bundlePopUp = new AppKit.NSPopUpButton();
			bundlePopUp.WantsLayer = true;
			bundlePopUp.BezelStyle = NSBezelStyle.Rounded;
			bundlePopUp.ControlSize = NSControlSize.Regular;
			bundlePopUp.AddItem ("Breakpoints Dialog");

			this.ContentView.AddSubview (bundlePopUp);
			bundlePopUp.Frame = bundlePopUp.GetFrameForAlignmentRect (new CoreGraphics.CGRect (154f, 130f, 223f, 21f));;

			// View:     versionPopUp
			// NodeName: "versionPopUp"
			// NodeType: INSTANCE
			// NodeId:   765:58
			versionPopUp = new AppKit.NSPopUpButton();
			versionPopUp.WantsLayer = true;
			versionPopUp.BezelStyle = NSBezelStyle.Rounded;
			versionPopUp.ControlSize = NSControlSize.Regular;
			versionPopUp.AddItem ("Current – 8.6");

			this.ContentView.AddSubview (versionPopUp);
			versionPopUp.Frame = versionPopUp.GetFrameForAlignmentRect (new CoreGraphics.CGRect (154f, 97f, 223f, 21f));;

			// View:     translationsCheckbox
			// NodeName: "translationsCheckbox"
			// NodeType: INSTANCE
			// NodeId:   1011:0
			translationsCheckbox = new AppKit.NSButton();
			translationsCheckbox.WantsLayer = true;
			translationsCheckbox.BezelStyle = NSBezelStyle.Rounded;
			translationsCheckbox.SetButtonType (NSButtonType.Switch);
			translationsCheckbox.ControlSize = NSControlSize.Regular;
			translationsCheckbox.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			translationsCheckbox.Title = "Make strings translatable";
			translationsCheckbox.State = NSCellStateValue.Off;

			this.ContentView.AddSubview (translationsCheckbox);
			translationsCheckbox.Frame = translationsCheckbox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (154f, 65f, 223f, 14f));;

		}
	}
}
