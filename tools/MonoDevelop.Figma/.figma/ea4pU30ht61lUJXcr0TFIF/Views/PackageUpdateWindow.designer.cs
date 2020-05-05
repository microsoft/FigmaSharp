// This file was auto-generated using
// FigmaSharp 1.0.0.0 and Figma API 1.0.1 on 2020-05-05 at 18:13
//
// Document title:   
// Document version: 0.1f
// Document URL:     ea4pU30ht61lUJXcr0TFIF
// Namespace:        MonoDevelop.Figma
//
// Changes to this file may cause incorrect behavior
// and will be lost if the code is regenerated.

using AppKit;

namespace MonoDevelop.Figma
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
			frame.Size = new CoreGraphics.CGSize (455f, 202f);
			this.SetFrame (frame, true);
			this.ContentMinSize = this.ContentView.Frame.Size;

			// View:     updateButton
			// NodeName: "updateButton"
			// NodeType: INSTANCE
			// NodeId:   38:685
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
			// NodeId:   38:686
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
			// NodeId:   38:687
			versionSpinner = new AppKit.NSProgressIndicator();
			versionSpinner.WantsLayer = true;
			versionSpinner.Style = NSProgressIndicatorStyle.Spinning;
			versionSpinner.Hidden = true;
			versionSpinner.ControlSize = NSControlSize.Small;

			this.ContentView.AddSubview (versionSpinner);
			versionSpinner.Frame = versionSpinner.GetFrameForAlignmentRect (new CoreGraphics.CGRect (383f, 95f, 18f, 18f));;

			// View:     labelView
			// NodeName: Generate:
			// NodeType: INSTANCE
			// NodeId:   38:688
			var labelView = new AppKit.NSTextField();
			labelView.Editable = false;
			labelView.Bordered = false;
			labelView.Bezeled = false;
			labelView.DrawsBackground = false;
			labelView.StringValue = "Figma Package:";
			labelView.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			labelView.WantsLayer = true;
			labelView.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (labelView);
			labelView.Frame = labelView.GetFrameForAlignmentRect (new CoreGraphics.CGRect (8f, 127f, 142f, 20f));;

			// View:     labelView1
			// NodeName: Generate:
			// NodeType: INSTANCE
			// NodeId:   38:689
			var labelView1 = new AppKit.NSTextField();
			labelView1.Editable = false;
			labelView1.Bordered = false;
			labelView1.Bezeled = false;
			labelView1.DrawsBackground = false;
			labelView1.StringValue = "Update to:";
			labelView1.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			labelView1.WantsLayer = true;
			labelView1.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (labelView1);
			labelView1.Frame = labelView1.GetFrameForAlignmentRect (new CoreGraphics.CGRect (8f, 94f, 142f, 20f));;

			// View:     bundlePopUp
			// NodeName: "bundlePopUp"
			// NodeType: INSTANCE
			// NodeId:   38:690
			bundlePopUp = new AppKit.NSPopUpButton();
			bundlePopUp.WantsLayer = true;
			bundlePopUp.BezelStyle = NSBezelStyle.Rounded;
			bundlePopUp.ControlSize = NSControlSize.Regular;
			bundlePopUp.AddItem ("Breakpoints Dialog");

			this.ContentView.AddSubview (bundlePopUp);
			bundlePopUp.Frame = bundlePopUp.GetFrameForAlignmentRect (new CoreGraphics.CGRect (154f, 126f, 223f, 21f));;

			// View:     versionPopUp
			// NodeName: "versionPopUp"
			// NodeType: INSTANCE
			// NodeId:   38:691
			versionPopUp = new AppKit.NSPopUpButton();
			versionPopUp.WantsLayer = true;
			versionPopUp.BezelStyle = NSBezelStyle.Rounded;
			versionPopUp.ControlSize = NSControlSize.Regular;
			versionPopUp.AddItem ("Current – 8.6");

			this.ContentView.AddSubview (versionPopUp);
			versionPopUp.Frame = versionPopUp.GetFrameForAlignmentRect (new CoreGraphics.CGRect (154f, 93f, 223f, 21f));;

			// View:     translationsCheckbox
			// NodeName: "translationsCheckbox"
			// NodeType: INSTANCE
			// NodeId:   38:692
			translationsCheckbox = new AppKit.NSButton();
			translationsCheckbox.WantsLayer = true;
			translationsCheckbox.BezelStyle = NSBezelStyle.Rounded;
			translationsCheckbox.SetButtonType (NSButtonType.Switch);
			translationsCheckbox.ControlSize = NSControlSize.Regular;
			translationsCheckbox.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			translationsCheckbox.Title = "Make strings translatable";
			translationsCheckbox.State = NSCellStateValue.Off;

			this.ContentView.AddSubview (translationsCheckbox);
			translationsCheckbox.Frame = translationsCheckbox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (154f, 61f, 223f, 14f));;

		}
	}
}
