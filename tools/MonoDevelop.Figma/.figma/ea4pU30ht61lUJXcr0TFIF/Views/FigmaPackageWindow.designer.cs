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
	partial class FigmaPackageWindow
	{
		private AppKit.NSButton bundleButton, cancelButton, translationsCheckbox, includeOriginalCheckbox, nothingRadio, templateRadio, codeRadio;
		private AppKit.NSView generateRadios;
		private AppKit.NSComboBox namespacePopUp;
		private AppKit.NSProgressIndicator versionSpinner;
		private AppKit.NSPopUpButton versionPopUp;
		private AppKit.NSTextField figmaUrlTextField;

		private void InitializeComponent ()
		{
			this.Title = "Add Figma Package";
			this.StyleMask |= NSWindowStyle.Resizable;
			this.StyleMask |= NSWindowStyle.Closable;
			this.StandardWindowButton (NSWindowButton.ZoomButton).Enabled = false;

			var frame = Frame;
			frame.Size = new CoreGraphics.CGSize (481f, 380f);
			this.SetFrame (frame, true);
			this.ContentMinSize = this.ContentView.Frame.Size;

			// View:     bundleButton
			// NodeName: "bundleButton"
			// NodeType: INSTANCE
			// NodeId:   38:696
			bundleButton = new AppKit.NSButton();
			bundleButton.WantsLayer = true;
			bundleButton.BezelStyle = NSBezelStyle.Rounded;
			bundleButton.ControlSize = NSControlSize.Regular;
			bundleButton.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			bundleButton.Title = "Add Package";
			bundleButton.KeyEquivalent = "\r";
			bundleButton.AccessibilityTitle = "Bundle";
			bundleButton.AccessibilityHelp = "Starts bundling the document";


			this.ContentView.AddSubview (bundleButton);
			bundleButton.Frame = bundleButton.GetFrameForAlignmentRect (new CoreGraphics.CGRect (354f, 20f, 108f, 21f));;

			// View:     cancelButton
			// NodeName: "cancelButton"
			// NodeType: INSTANCE
			// NodeId:   38:697
			cancelButton = new AppKit.NSButton();
			cancelButton.WantsLayer = true;
			cancelButton.BezelStyle = NSBezelStyle.Rounded;
			cancelButton.ControlSize = NSControlSize.Regular;
			cancelButton.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			cancelButton.Title = "Cancel";
			cancelButton.AccessibilityTitle = "Cancel";
			cancelButton.AccessibilityHelp = "Cancel bundling";


			this.ContentView.AddSubview (cancelButton);
			cancelButton.Frame = cancelButton.GetFrameForAlignmentRect (new CoreGraphics.CGRect (19f, 20f, 84f, 21f));;

			// View:     lineView
			// NodeName: sep
			// NodeType: VECTOR
			// NodeId:   38:698
			var lineView = new AppKit.NSBox();
			lineView.WantsLayer = true;
			lineView.BoxType = NSBoxType.NSBoxSeparator;

			this.ContentView.AddSubview (lineView);
			lineView.Frame = lineView.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f, 59.5f, 481f, 0f));;

			// View:     translationsCheckbox
			// NodeName: "translationsCheckbox"
			// NodeType: INSTANCE
			// NodeId:   38:699
			translationsCheckbox = new AppKit.NSButton();
			translationsCheckbox.WantsLayer = true;
			translationsCheckbox.BezelStyle = NSBezelStyle.Rounded;
			translationsCheckbox.SetButtonType (NSButtonType.Switch);
			translationsCheckbox.ControlSize = NSControlSize.Regular;
			translationsCheckbox.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			translationsCheckbox.Title = "Make strings translatable";
			translationsCheckbox.State = NSCellStateValue.Off;

			this.ContentView.AddSubview (translationsCheckbox);
			translationsCheckbox.Frame = translationsCheckbox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (169f, 104f, 238f, 14f));;

			// View:     includeOriginalCheckbox
			// NodeName: "includeOriginalCheckbox"
			// NodeType: INSTANCE
			// NodeId:   38:700
			includeOriginalCheckbox = new AppKit.NSButton();
			includeOriginalCheckbox.WantsLayer = true;
			includeOriginalCheckbox.BezelStyle = NSBezelStyle.Rounded;
			includeOriginalCheckbox.SetButtonType (NSButtonType.Switch);
			includeOriginalCheckbox.ControlSize = NSControlSize.Regular;
			includeOriginalCheckbox.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			includeOriginalCheckbox.Title = "Include original Figma document";
			includeOriginalCheckbox.State = NSCellStateValue.On;

			this.ContentView.AddSubview (includeOriginalCheckbox);
			includeOriginalCheckbox.Frame = includeOriginalCheckbox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (169f, 82f, 238f, 14f));;

			// View:     generateRadios
			// NodeName: "generateRadios"
			// NodeType: FRAME
			// NodeId:   38:701
			generateRadios = new AppKit.NSView();
			generateRadios.WantsLayer = true;

			this.ContentView.AddSubview (generateRadios);
			generateRadios.Frame = generateRadios.GetFrameForAlignmentRect (new CoreGraphics.CGRect (169f, 135f, 220f, 60f));;

			// View:     nothingRadio
			// NodeName: radio "nothingRadio"
			// NodeType: INSTANCE
			// NodeId:   38:702
			nothingRadio = new AppKit.NSButton();
			nothingRadio.WantsLayer = true;
			nothingRadio.BezelStyle = NSBezelStyle.Rounded;
			nothingRadio.SetButtonType (NSButtonType.Radio);
			nothingRadio.ControlSize = NSControlSize.Regular;
			nothingRadio.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			nothingRadio.Title = "Nothing";
			nothingRadio.AccessibilityTitle = "Nothing";
			nothingRadio.AccessibilityHelp = "Select to not output anything";


			generateRadios.AddSubview (nothingRadio);
			nothingRadio.Frame = nothingRadio.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f, 0f, 89f, 16f));;

			// View:     templateRadio
			// NodeName: radio "templateRadio"
			// NodeType: INSTANCE
			// NodeId:   38:703
			templateRadio = new AppKit.NSButton();
			templateRadio.WantsLayer = true;
			templateRadio.BezelStyle = NSBezelStyle.Rounded;
			templateRadio.SetButtonType (NSButtonType.Radio);
			templateRadio.ControlSize = NSControlSize.Regular;
			templateRadio.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			templateRadio.Title = "Template";
			templateRadio.AccessibilityTitle = "Template";
			templateRadio.AccessibilityHelp = "Select to output a template";


			generateRadios.AddSubview (templateRadio);
			templateRadio.Frame = templateRadio.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f, 22f, 89f, 16f));;

			// View:     codeRadio
			// NodeName: radio "codeRadio"
			// NodeType: INSTANCE
			// NodeId:   38:704
			codeRadio = new AppKit.NSButton();
			codeRadio.WantsLayer = true;
			codeRadio.BezelStyle = NSBezelStyle.Rounded;
			codeRadio.SetButtonType (NSButtonType.Radio);
			codeRadio.ControlSize = NSControlSize.Regular;
			codeRadio.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			codeRadio.Title = "Code";
			codeRadio.State = NSCellStateValue.On;

			generateRadios.AddSubview (codeRadio);
			codeRadio.Frame = codeRadio.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f, 44f, 54f, 16f));;

			// View:     labelView
			// NodeName: Generate:
			// NodeType: INSTANCE
			// NodeId:   38:705
			var labelView = new AppKit.NSTextField();
			labelView.Editable = false;
			labelView.Bordered = false;
			labelView.Bezeled = false;
			labelView.DrawsBackground = false;
			labelView.StringValue = "Generate:";
			labelView.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			labelView.WantsLayer = true;
			labelView.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (labelView);
			labelView.Frame = labelView.GetFrameForAlignmentRect (new CoreGraphics.CGRect (21f, 177f, 142f, 20f));;

			// View:     namespacePopUp
			// NodeName: "namespacePopUp"
			// NodeType: INSTANCE
			// NodeId:   38:706
			namespacePopUp = new AppKit.NSComboBox();
			namespacePopUp.WantsLayer = true;
			namespacePopUp.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			namespacePopUp.Add (new Foundation.NSString ("MyApp"));
			namespacePopUp.AccessibilityTitle = "Namespace";
			namespacePopUp.AccessibilityHelp = "The namespace to generate code in";


			this.ContentView.AddSubview (namespacePopUp);
			namespacePopUp.Frame = namespacePopUp.GetFrameForAlignmentRect (new CoreGraphics.CGRect (169f, 210f, 220f, 20f));;

			// View:     labelView1
			// NodeName: Namespace:
			// NodeType: INSTANCE
			// NodeId:   38:707
			var labelView1 = new AppKit.NSTextField();
			labelView1.Editable = false;
			labelView1.Bordered = false;
			labelView1.Bezeled = false;
			labelView1.DrawsBackground = false;
			labelView1.StringValue = "Namespace:";
			labelView1.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			labelView1.WantsLayer = true;
			labelView1.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (labelView1);
			labelView1.Frame = labelView1.GetFrameForAlignmentRect (new CoreGraphics.CGRect (21f, 211f, 142f, 20f));;

			// View:     lineView1
			// NodeName: sep
			// NodeType: VECTOR
			// NodeId:   38:708
			var lineView1 = new AppKit.NSBox();
			lineView1.WantsLayer = true;
			lineView1.BoxType = NSBoxType.NSBoxSeparator;

			this.ContentView.AddSubview (lineView1);
			lineView1.Frame = lineView1.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f, 255.5f, 481f, 0f));;

			// View:     versionSpinner
			// NodeName: "versionSpinner"
			// NodeType: INSTANCE
			// NodeId:   38:709
			versionSpinner = new AppKit.NSProgressIndicator();
			versionSpinner.WantsLayer = true;
			versionSpinner.Style = NSProgressIndicatorStyle.Spinning;
			versionSpinner.Hidden = true;
			versionSpinner.ControlSize = NSControlSize.Small;

			this.ContentView.AddSubview (versionSpinner);
			versionSpinner.Frame = versionSpinner.GetFrameForAlignmentRect (new CoreGraphics.CGRect (396f, 281f, 18f, 18f));;

			// View:     versionPopUp
			// NodeName: "versionPopUp"
			// NodeType: INSTANCE
			// NodeId:   38:710
			versionPopUp = new AppKit.NSPopUpButton();
			versionPopUp.WantsLayer = true;
			versionPopUp.BezelStyle = NSBezelStyle.Rounded;
			versionPopUp.ControlSize = NSControlSize.Regular;
			versionPopUp.AddItem ("Current");

			this.ContentView.AddSubview (versionPopUp);
			versionPopUp.Frame = versionPopUp.GetFrameForAlignmentRect (new CoreGraphics.CGRect (167f, 279f, 223f, 21f));;

			// View:     labelView2
			// NodeName: Version:
			// NodeType: INSTANCE
			// NodeId:   38:711
			var labelView2 = new AppKit.NSTextField();
			labelView2.Editable = false;
			labelView2.Bordered = false;
			labelView2.Bezeled = false;
			labelView2.DrawsBackground = false;
			labelView2.StringValue = "Version:";
			labelView2.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			labelView2.WantsLayer = true;
			labelView2.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (labelView2);
			labelView2.Frame = labelView2.GetFrameForAlignmentRect (new CoreGraphics.CGRect (21f, 280f, 142f, 20f));;

			// View:     figmaUrlTextField
			// NodeName: "figmaUrlTextField"
			// NodeType: INSTANCE
			// NodeId:   38:712
			figmaUrlTextField = new AppKit.NSTextField();
			figmaUrlTextField.WantsLayer = true;
			figmaUrlTextField.PlaceholderString = "https://www.figma.com/file/";

			this.ContentView.AddSubview (figmaUrlTextField);
			figmaUrlTextField.Frame = figmaUrlTextField.GetFrameForAlignmentRect (new CoreGraphics.CGRect (168f, 311f, 221f, 21f));;

			// View:     labelView3
			// NodeName: Figma URL:
			// NodeType: INSTANCE
			// NodeId:   38:713
			var labelView3 = new AppKit.NSTextField();
			labelView3.Editable = false;
			labelView3.Bordered = false;
			labelView3.Bezeled = false;
			labelView3.DrawsBackground = false;
			labelView3.StringValue = "Figma URL:";
			labelView3.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			labelView3.WantsLayer = true;
			labelView3.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (labelView3);
			labelView3.Frame = labelView3.GetFrameForAlignmentRect (new CoreGraphics.CGRect (21f, 311f, 142f, 20f));;

		}
	}
}
