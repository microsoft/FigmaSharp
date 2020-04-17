// This file was auto-generated using
// FigmaSharp 1.0.0.0 and Figma API 1.0.1 on 2020-04-12 at 06:54
//
// Document title:   
// Document version: 0.1f
// Document URL:     ea4pU30ht61lUJXcr0TFIF
// Namespace:        FigmaSharp
//
// Changes to this file may cause incorrect behavior
// and will be lost if the code is regenerated.

using System;
using AppKit;
using CoreGraphics;
using Foundation;

namespace MonoDevelop.Figma.Packages
{
	partial class GenerateViewsWindow
	{
		private AppKit.NSButton createButton, cancelButton, translationsCheckbox;
		private AppKit.NSTableView fileTableView;
		private AppKit.NSProgressIndicator versionSpinner;

		private void InitializeComponent ()
		{
			this.Title = "Create Views";
			this.StyleMask |= NSWindowStyle.Resizable;
			this.StyleMask |= NSWindowStyle.Closable;
			this.StandardWindowButton (NSWindowButton.ZoomButton).Enabled = false;

			var frame = Frame;
			frame.Size = new CoreGraphics.CGSize (481f, 345f);
			this.SetFrame (frame, true);
			this.ContentMinSize = this.ContentView.Frame.Size;

			// View:     bundleButton
			// NodeName: "bundleButton"
			// NodeType: INSTANCE
			// NodeId:   0:697
			createButton = new AppKit.NSButton();
			createButton.WantsLayer = true;
			createButton.BezelStyle = NSBezelStyle.Rounded;
			createButton.ControlSize = NSControlSize.Regular;
			createButton.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			createButton.Title = "Create";
			createButton.KeyEquivalent = "\r";

			this.ContentView.AddSubview (createButton);
			createButton.Frame = createButton.GetFrameForAlignmentRect (new CoreGraphics.CGRect (373f, 20f, 89f, 21f));;

			// View:     cancelButton
			// NodeName: "cancelButton"
			// NodeType: INSTANCE
			// NodeId:   0:698
			cancelButton = new AppKit.NSButton();
			cancelButton.WantsLayer = true;
			cancelButton.BezelStyle = NSBezelStyle.Rounded;
			cancelButton.ControlSize = NSControlSize.Regular;
			cancelButton.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			cancelButton.Title = "Cancel";

			this.ContentView.AddSubview (cancelButton);
			cancelButton.Frame = cancelButton.GetFrameForAlignmentRect (new CoreGraphics.CGRect (19f, 20f, 84f, 21f));

			// View:     MyTable
			// NodeName: customview "MyTable" type:"AppKit.NSTableView"
			// NodeType: FRAME
			// NodeId:   0:699
			var fileTableScrollView = new NSScrollView();
			fileTableView = new NSTableView ();
			fileTableView.UsesAlternatingRowBackgroundColors = true;
			fileTableScrollView.DocumentView = fileTableView;
			this.ContentView.AddSubview (fileTableScrollView);
			fileTableScrollView.Frame = fileTableScrollView.GetFrameForAlignmentRect(new CoreGraphics.CGRect(20f, 60f, 441f, 172));
			fileTableView.Frame = fileTableView.GetFrameForAlignmentRect(new CoreGraphics.CGRect(0, 0, 400f, 172));

			// View:     versionSpinner
			// NodeName: "versionSpinner"
			// NodeType: INSTANCE
			// NodeId:   0:704
			versionSpinner = new AppKit.NSProgressIndicator();
			versionSpinner.WantsLayer = true;
			versionSpinner.Style = NSProgressIndicatorStyle.Spinning;
			versionSpinner.Hidden = true;
			versionSpinner.ControlSize = NSControlSize.Small;

			this.ContentView.AddSubview (versionSpinner);
			versionSpinner.Frame = versionSpinner.GetFrameForAlignmentRect (new CoreGraphics.CGRect (383f, 284f, 18f, 18f));

			// View:     nativeViewCodeServiceView1
			// NodeName: Generate:
			// NodeType: INSTANCE
			// NodeId:   0:706
			var nativeViewCodeServiceView1 = new AppKit.NSTextField();
			nativeViewCodeServiceView1.Editable = false;
			nativeViewCodeServiceView1.Bordered = false;
			nativeViewCodeServiceView1.Bezeled = false;
			nativeViewCodeServiceView1.DrawsBackground = false;
			nativeViewCodeServiceView1.StringValue = "Version:";
			nativeViewCodeServiceView1.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			nativeViewCodeServiceView1.WantsLayer = true;
			nativeViewCodeServiceView1.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (nativeViewCodeServiceView1);
			nativeViewCodeServiceView1.Frame = nativeViewCodeServiceView1.GetFrameForAlignmentRect (new CoreGraphics.CGRect (8f, 283f, 142f, 20f));;

			// View:     versionTextField
			// NodeName: "versionPopUp"
			// NodeType: INSTANCE
			// NodeId:   0:708
			var versionTextField = new AppKit.NSTextField();
			versionTextField.Editable = false;
			versionTextField.Bordered = false;
			versionTextField.Bezeled = false;
			versionTextField.DrawsBackground = false;
			versionTextField.StringValue = "Current – 8.6";
			versionTextField.Font = AppKit.NSFont.SystemFontOfSize(AppKit.NSFont.SystemFontSize);
			versionTextField.WantsLayer = true;

			this.ContentView.AddSubview (versionTextField);
			versionTextField.Frame = versionTextField.GetFrameForAlignmentRect(new CoreGraphics.CGRect(154f, 282f, 223f, 21f));

			// View:     translationsCheckbox
			// NodeName: "translationsCheckbox"
			// NodeType: INSTANCE
			// NodeId:   0:709
			translationsCheckbox = new AppKit.NSButton();
			translationsCheckbox.WantsLayer = true;
			translationsCheckbox.BezelStyle = NSBezelStyle.Rounded;
			translationsCheckbox.SetButtonType (NSButtonType.Switch);
			translationsCheckbox.ControlSize = NSControlSize.Regular;
			translationsCheckbox.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			translationsCheckbox.Title = "Make strings translatable";
			translationsCheckbox.State = NSCellStateValue.Off;

			this.ContentView.AddSubview (translationsCheckbox);
			translationsCheckbox.Frame = translationsCheckbox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (154f, 250f, 223f, 14f));;
		}
	}
}
