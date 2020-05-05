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
	partial class GenerateViewsWindow
	{
		private AppKit.NSButton bundleButton, cancelButton, translationsCheckbox;
		private AppKit.NSScrollView tableScrollView;
		private AppKit.NSTextField versionLabel;

		private void InitializeComponent ()
		{
			this.Title = "Update Figma Package";
			this.StyleMask |= NSWindowStyle.Resizable;
			this.StyleMask |= NSWindowStyle.Closable;
			this.StandardWindowButton (NSWindowButton.ZoomButton).Enabled = false;

			var frame = Frame;
			frame.Size = new CoreGraphics.CGSize (481f, 379f);
			this.SetFrame (frame, true);
			this.ContentMinSize = this.ContentView.Frame.Size;

			// View:     bundleButton
			// NodeName: "bundleButton"
			// NodeType: INSTANCE
			// NodeId:   38:717
			bundleButton = new AppKit.NSButton();
			bundleButton.WantsLayer = true;
			bundleButton.BezelStyle = NSBezelStyle.Rounded;
			bundleButton.ControlSize = NSControlSize.Regular;
			bundleButton.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			bundleButton.Title = "Update";
			bundleButton.KeyEquivalent = "\r";

			this.ContentView.AddSubview (bundleButton);
			bundleButton.Frame = bundleButton.GetFrameForAlignmentRect (new CoreGraphics.CGRect (373f, 20f, 89f, 21f));;

			// View:     cancelButton
			// NodeName: "cancelButton"
			// NodeType: INSTANCE
			// NodeId:   38:718
			cancelButton = new AppKit.NSButton();
			cancelButton.WantsLayer = true;
			cancelButton.BezelStyle = NSBezelStyle.Rounded;
			cancelButton.ControlSize = NSControlSize.Regular;
			cancelButton.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			cancelButton.Title = "Cancel";

			this.ContentView.AddSubview (cancelButton);
			cancelButton.Frame = cancelButton.GetFrameForAlignmentRect (new CoreGraphics.CGRect (19f, 20f, 84f, 21f));;

			// View:     tableScrollView
			// NodeName: customview "tableScrollView" type:"AppKit.NSScrollView"
			// NodeType: FRAME
			// NodeId:   38:719
			tableScrollView = new AppKit.NSScrollView();

			this.ContentView.AddSubview (tableScrollView);
			tableScrollView.Frame = tableScrollView.GetFrameForAlignmentRect (new CoreGraphics.CGRect (20f, 57f, 441f, 250f));;

			// View:     versionLabel
			// NodeName: "versionLabel"
			// NodeType: INSTANCE
			// NodeId:   38:726
			versionLabel = new AppKit.NSTextField();
			versionLabel.Editable = false;
			versionLabel.Bordered = false;
			versionLabel.Bezeled = false;
			versionLabel.DrawsBackground = false;
			versionLabel.StringValue = "Current - 8.6";
			versionLabel.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			versionLabel.WantsLayer = true;

			this.ContentView.AddSubview (versionLabel);
			versionLabel.Frame = versionLabel.GetFrameForAlignmentRect (new CoreGraphics.CGRect (93f, 321f, 162f, 20f));;

			// View:     labelView
			// NodeName: Generate:
			// NodeType: INSTANCE
			// NodeId:   44:0
			var labelView = new AppKit.NSTextField();
			labelView.Editable = false;
			labelView.Bordered = false;
			labelView.Bezeled = false;
			labelView.DrawsBackground = false;
			labelView.StringValue = "Update to:";
			labelView.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			labelView.WantsLayer = true;

			this.ContentView.AddSubview (labelView);
			labelView.Frame = labelView.GetFrameForAlignmentRect (new CoreGraphics.CGRect (21f, 321f, 68f, 20f));;

			// View:     translationsCheckbox
			// NodeName: "translationsCheckbox"
			// NodeType: INSTANCE
			// NodeId:   38:729
			translationsCheckbox = new AppKit.NSButton();
			translationsCheckbox.WantsLayer = true;
			translationsCheckbox.BezelStyle = NSBezelStyle.Rounded;
			translationsCheckbox.SetButtonType (NSButtonType.Switch);
			translationsCheckbox.ControlSize = NSControlSize.Regular;
			translationsCheckbox.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			translationsCheckbox.Title = "Make strings translatable";
			translationsCheckbox.State = NSCellStateValue.Off;

			this.ContentView.AddSubview (translationsCheckbox);
			translationsCheckbox.Frame = translationsCheckbox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (277f, 324f, 187f, 14f));;

		}
	}
}
