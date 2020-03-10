/*
This file was auto-generated using
FigmaSharp 1.0.0.0 and Figma API 1.0.1 on 2020-03-08 at 21:20

Document title:   
Document version: 0.1f
Document URL:     FwVa4JS5QsohRhNEnEBKslFk

Changes to this file may cause incorrect behavior
and will be lost if the code is regenerated.
* 
*/
using AppKit;
namespace MonoDevelop.Figma.FigmaBundles
{
	partial class BundleUpdateWindow
	{
		NSButton UpdateButton, CancelButton;
		NSProgressIndicator loadingProgressIndicator;
		NSPopUpButton versionComboBox, BundlePopUp;

		private void InitializeComponent()
		{
			this.StyleMask |= NSWindowStyle.Closable;
			this.StyleMask |= NSWindowStyle.Resizable;
			this.Title = "Update Figma Bundle";

			var frame = Frame;
			frame.Size = new CoreGraphics.CGSize(455f, 171f);
			this.SetFrame(frame, true);

			// View:     updateButton
			// NodeName: "updateButton"
			// NodeType: INSTANCE
			// NodeId:   539:207

			UpdateButton = new AppKit.NSButton();
			UpdateButton.WantsLayer = true;
			UpdateButton.BezelStyle = NSBezelStyle.Rounded;
			UpdateButton.ControlSize = NSControlSize.Regular;
			UpdateButton.Title = "Update";
			UpdateButton.KeyEquivalent = "\r";

			this.ContentView.AddSubview(UpdateButton);
			UpdateButton.Frame = UpdateButton.GetFrameForAlignmentRect(new CoreGraphics.CGRect(353f, 21f, 82f, 19f)); ;


			// View:     cancelButton
			// NodeName: "cancelButton"
			// NodeType: INSTANCE
			// NodeId:   539:208

			CancelButton = new AppKit.NSButton();
			CancelButton.WantsLayer = true;
			CancelButton.BezelStyle = NSBezelStyle.Rounded;
			CancelButton.ControlSize = NSControlSize.Regular;
			CancelButton.Title = "Cancel";

			this.ContentView.AddSubview(CancelButton);
			CancelButton.Frame = CancelButton.GetFrameForAlignmentRect(new CoreGraphics.CGRect(258f, 21f, 82f, 19f));


			// View:     versionSpinner
			// NodeName: "versionSpinner"
			// NodeType: INSTANCE
			// NodeId:   539:217

			loadingProgressIndicator = new AppKit.NSProgressIndicator();
			loadingProgressIndicator.WantsLayer = true;
			loadingProgressIndicator.Style = NSProgressIndicatorStyle.Spinning;
			loadingProgressIndicator.Hidden = true;
			loadingProgressIndicator.ControlSize = NSControlSize.Small;

			this.ContentView.AddSubview(loadingProgressIndicator);
			loadingProgressIndicator.Frame = loadingProgressIndicator.GetFrameForAlignmentRect(new CoreGraphics.CGRect(383f, 72f, 18f, 18f));

			// View:     versionPopUp
			// NodeName: "versionPopUp"
			// NodeType: INSTANCE
			// NodeId:   539:219

			versionComboBox = new AppKit.NSPopUpButton();
			versionComboBox.WantsLayer = true;
			versionComboBox.BezelStyle = NSBezelStyle.Rounded;
			versionComboBox.ControlSize = NSControlSize.Regular;
			versionComboBox.AddItem("Current – 8.6");

			this.ContentView.AddSubview(versionComboBox);
			versionComboBox.Frame = versionComboBox.GetFrameForAlignmentRect(new CoreGraphics.CGRect(155f, 71f, 220f, 19f)); ;

			// View:     nativeViewCodeServiceView
			// NodeName: Update to:
			// NodeType: INSTANCE
			// NodeId:   539:226

			var nativeViewCodeServiceView = new NSView();
			nativeViewCodeServiceView.WantsLayer = true;

			this.ContentView.AddSubview(nativeViewCodeServiceView);
			nativeViewCodeServiceView.Frame = nativeViewCodeServiceView.GetFrameForAlignmentRect(new CoreGraphics.CGRect(8f, 74f, 142f, 16f));

			// View:     nativeViewCodeServiceView1
			// NodeName: lbl
			// NodeType: TEXT
			// NodeId:   I539:226;527:57

			var nativeViewCodeServiceView1 = new AppKit.NSTextField()
			{
				StringValue = "Update to:",
				Editable = false,
				Bordered = false,
				Bezeled = false,
				DrawsBackground = false,
				Alignment = NSTextAlignment.Left,
			};
			nativeViewCodeServiceView1.WantsLayer = true;
			nativeViewCodeServiceView1.Alignment = NSTextAlignment.Right;

			nativeViewCodeServiceView.AddSubview(nativeViewCodeServiceView1);
			nativeViewCodeServiceView1.Frame = nativeViewCodeServiceView1.GetFrameForAlignmentRect(new CoreGraphics.CGRect(0f, 0f, 142f, 16f));

			// View:     bundlePopUp
			// NodeName: "bundlePopUp"
			// NodeType: INSTANCE
			// NodeId:   539:345

			BundlePopUp = new AppKit.NSPopUpButton();
			BundlePopUp.WantsLayer = true;
			BundlePopUp.BezelStyle = NSBezelStyle.Rounded;
			BundlePopUp.ControlSize = NSControlSize.Regular;
			BundlePopUp.AddItem("Breakpoints Dialog");

			this.ContentView.AddSubview(BundlePopUp);
			BundlePopUp.Frame = BundlePopUp.GetFrameForAlignmentRect(new CoreGraphics.CGRect(155f, 104f, 220f, 19f)); ;

			// View:     nativeViewCodeServiceView2
			// NodeName: Figma Bundle:
			// NodeType: INSTANCE
			// NodeId:   539:225

			var nativeViewCodeServiceView2 = new NSView();
			nativeViewCodeServiceView2.WantsLayer = true;

			this.ContentView.AddSubview(nativeViewCodeServiceView2);
			nativeViewCodeServiceView2.Frame = nativeViewCodeServiceView2.GetFrameForAlignmentRect(new CoreGraphics.CGRect(8f, 105f, 142f, 16f));

			// View:     nativeViewCodeServiceView3
			// NodeName: lbl
			// NodeType: TEXT
			// NodeId:   I539:225;527:57

			var nativeViewCodeServiceView3 = new AppKit.NSTextField()
			{
				StringValue = "Figma Bundle:",
				Editable = false,
				Bordered = false,
				Bezeled = false,
				DrawsBackground = false,
				Alignment = NSTextAlignment.Left,
			};
			nativeViewCodeServiceView3.WantsLayer = true;
			nativeViewCodeServiceView3.Alignment = NSTextAlignment.Right;

			nativeViewCodeServiceView2.AddSubview(nativeViewCodeServiceView3);
			nativeViewCodeServiceView3.Frame = nativeViewCodeServiceView3.GetFrameForAlignmentRect(new CoreGraphics.CGRect(0f, 0f, 142f, 16f));
		}
	}
}
