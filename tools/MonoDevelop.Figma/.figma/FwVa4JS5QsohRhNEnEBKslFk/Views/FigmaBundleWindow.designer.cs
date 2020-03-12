/*
This file was auto-generated using
FigmaSharp 1.0.0.0 and Figma API 1.0.1 on 2020-03-12 at 03:52

Document title:   
Document version: 0.1f
Document Namespace:   FigmaSharp
Document URL:     FwVa4JS5QsohRhNEnEBKslFk

Changes to this file may cause incorrect behavior
and will be lost if the code is regenerated.
* 
*/
using AppKit;
namespace MonoDevelop.Figma.FigmaBundles
{
	partial class FigmaBundleWindow
	{
		NSTextField figmaUrlTextField;
		NSPopUpButton versionComboBox;
		NSButton templateNoneOptionBox, templateMarkUpOptionBox, templateCodeOptionBox;
		NSProgressIndicator loadingProgressIndicator;
		NSButton includeOriginalCheckbox;
		NSButton bundleButton, cancelButton;
		
		NSComboBox namespacePopUp;

		private void InitializeComponent ()
		{
			this.StyleMask |= NSWindowStyle.Closable;
			this.StyleMask |= NSWindowStyle.Resizable;
			this.Title = "Bundle Figma Document";
			var frame = Frame;
			frame.Size = new CoreGraphics.CGSize (481f,362f);
			this.SetFrame (frame,true);
			
			// View:     bundleButton
			// NodeName: "bundleButton"
			// NodeType: INSTANCE
			// NodeId:   680:549
			
			bundleButton = new AppKit.NSButton();
			bundleButton.WantsLayer = true;
			bundleButton.BezelStyle = NSBezelStyle.Rounded;
			bundleButton.ControlSize = NSControlSize.Regular;
			bundleButton.Title = "Bundle";
			bundleButton.KeyEquivalent = "\r";
			bundleButton.AccessibilityTitle = "Bundle";
			bundleButton.AccessibilityHelp = "Starts bundling the document";
			
			
			this.ContentView.AddSubview (bundleButton);
			bundleButton.Frame = bundleButton.GetFrameForAlignmentRect (new CoreGraphics.CGRect (378f,20f,84f,21f));;
			
			
			// View:     cancelButton
			// NodeName: "cancelButton"
			// NodeType: INSTANCE
			// NodeId:   680:564
			
			cancelButton = new AppKit.NSButton();
			cancelButton.WantsLayer = true;
			cancelButton.BezelStyle = NSBezelStyle.Rounded;
			cancelButton.ControlSize = NSControlSize.Regular;
			cancelButton.Title = "Cancel";
			cancelButton.AccessibilityTitle = "Cancel";
			cancelButton.AccessibilityHelp = "Cancel bundling";
			
			
			this.ContentView.AddSubview (cancelButton);
			cancelButton.Frame = cancelButton.GetFrameForAlignmentRect (new CoreGraphics.CGRect (19f,20f,84f,21f));;
			
			
			// View:     nativeViewCodeServiceView
			// NodeName: sep
			// NodeType: VECTOR
			// NodeId:   622:0
			
			var nativeViewCodeServiceView = new AppKit.NSBox();
			nativeViewCodeServiceView.WantsLayer = true;
			nativeViewCodeServiceView.BoxType = NSBoxType.NSBoxSeparator;
			
			this.ContentView.AddSubview (nativeViewCodeServiceView);
			nativeViewCodeServiceView.Frame = nativeViewCodeServiceView.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f,59.5f,481f,0f));;
			
			
			// View:     includeOriginalCheckbox
			// NodeName: "includeOriginalCheckbox"
			// NodeType: INSTANCE
			// NodeId:   527:46
			
			includeOriginalCheckbox = new AppKit.NSButton();
			includeOriginalCheckbox.WantsLayer = true;
			includeOriginalCheckbox.BezelStyle = NSBezelStyle.Rounded;
			includeOriginalCheckbox.SetButtonType (NSButtonType.Switch);
			includeOriginalCheckbox.ControlSize = NSControlSize.Regular;
			includeOriginalCheckbox.Title = "Include original Figma document";
			includeOriginalCheckbox.State = NSCellStateValue.On;
			
			this.ContentView.AddSubview (includeOriginalCheckbox);
			includeOriginalCheckbox.Frame = includeOriginalCheckbox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (169f,86f,238f,14f));;
			
			
			// View:     generateRadios
			// NodeName: "generateRadios"
			// NodeType: FRAME
			// NodeId:   577:0
			
			var generateRadios = new NSView();
			generateRadios.WantsLayer = true;
			
			this.ContentView.AddSubview (generateRadios);
			generateRadios.Frame = generateRadios.GetFrameForAlignmentRect (new CoreGraphics.CGRect (169f,117f,220f,60f));;
			
			
			// View:     nothingRadio
			// NodeName: radio "nothingRadio"
			// NodeType: INSTANCE
			// NodeId:   576:0
			
			templateNoneOptionBox = new AppKit.NSButton();
			templateNoneOptionBox.WantsLayer = true;
			templateNoneOptionBox.BezelStyle = NSBezelStyle.Rounded;
			templateNoneOptionBox.SetButtonType (NSButtonType.Radio);
			templateNoneOptionBox.ControlSize = NSControlSize.Regular;
			templateNoneOptionBox.Title = "Nothing";
			
			generateRadios.AddSubview (templateNoneOptionBox);
			templateNoneOptionBox.Frame = templateNoneOptionBox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f,0f,89f,16f));;
			
			
			// View:     templateRadio
			// NodeName: radio "templateRadio"
			// NodeType: INSTANCE
			// NodeId:   557:20
			
			templateMarkUpOptionBox = new AppKit.NSButton();
			templateMarkUpOptionBox.WantsLayer = true;
			templateMarkUpOptionBox.BezelStyle = NSBezelStyle.Rounded;
			templateMarkUpOptionBox.SetButtonType (NSButtonType.Radio);
			templateMarkUpOptionBox.ControlSize = NSControlSize.Regular;
			templateMarkUpOptionBox.Title = "Template";
			
			generateRadios.AddSubview (templateMarkUpOptionBox);
			templateMarkUpOptionBox.Frame = templateMarkUpOptionBox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f,22f,89f,16f));;
			
			
			// View:     codeRadio
			// NodeName: radio "codeRadio"
			// NodeType: INSTANCE
			// NodeId:   557:11
			
			templateCodeOptionBox = new AppKit.NSButton();
			templateCodeOptionBox.WantsLayer = true;
			templateCodeOptionBox.BezelStyle = NSBezelStyle.Rounded;
			templateCodeOptionBox.SetButtonType (NSButtonType.Radio);
			templateCodeOptionBox.ControlSize = NSControlSize.Regular;
			templateCodeOptionBox.Title = "Code";
			templateCodeOptionBox.State = NSCellStateValue.On;
			
			generateRadios.AddSubview (templateCodeOptionBox);
			templateCodeOptionBox.Frame = templateCodeOptionBox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f,44f,54f,16f));;
			
			
			// View:     nativeViewCodeServiceView1
			// NodeName: Generate:
			// NodeType: INSTANCE
			// NodeId:   754:19
			
			var nativeViewCodeServiceView1 = new AppKit.NSTextField() {    StringValue = "Generate:",
			Editable = false,
			Bordered = false,
			Bezeled = false,
			DrawsBackground = false,
			Alignment = NSTextAlignment.Left,
			};
			nativeViewCodeServiceView1.WantsLayer = true;
			nativeViewCodeServiceView1.Alignment = NSTextAlignment.Right;
			
			this.ContentView.AddSubview (nativeViewCodeServiceView1);
			nativeViewCodeServiceView1.Frame = nativeViewCodeServiceView1.GetFrameForAlignmentRect (new CoreGraphics.CGRect (21f,159f,142f,20f));;
			
			
			// View:     namespacePopUp
			// NodeName: "namespacePopUp"
			// NodeType: INSTANCE
			// NodeId:   576:78
			
			namespacePopUp = new AppKit.NSComboBox();
			namespacePopUp.WantsLayer = true;
			
			this.ContentView.AddSubview (namespacePopUp);
			namespacePopUp.Frame = namespacePopUp.GetFrameForAlignmentRect (new CoreGraphics.CGRect (169f,192f,220f,20f));;
			
			
			// View:     nativeViewCodeServiceView2
			// NodeName: Namespace:
			// NodeType: INSTANCE
			// NodeId:   754:17
			
			var nativeViewCodeServiceView2 = new AppKit.NSTextField() {    StringValue = "Namespace:",
			Editable = false,
			Bordered = false,
			Bezeled = false,
			DrawsBackground = false,
			Alignment = NSTextAlignment.Left,
			};
			nativeViewCodeServiceView2.WantsLayer = true;
			nativeViewCodeServiceView2.Alignment = NSTextAlignment.Right;
			
			this.ContentView.AddSubview (nativeViewCodeServiceView2);
			nativeViewCodeServiceView2.Frame = nativeViewCodeServiceView2.GetFrameForAlignmentRect (new CoreGraphics.CGRect (21f,193f,142f,20f));;
			
			
			// View:     nativeViewCodeServiceView3
			// NodeName: sep
			// NodeType: VECTOR
			// NodeId:   209:132
			
			var nativeViewCodeServiceView3 = new AppKit.NSBox();
			nativeViewCodeServiceView3.WantsLayer = true;
			nativeViewCodeServiceView3.BoxType = NSBoxType.NSBoxSeparator;
			
			this.ContentView.AddSubview (nativeViewCodeServiceView3);
			nativeViewCodeServiceView3.Frame = nativeViewCodeServiceView3.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f,237.5f,481f,0f));;
			
			
			// View:     versionSpinner
			// NodeName: "versionSpinner"
			// NodeType: INSTANCE
			// NodeId:   772:27
			
			loadingProgressIndicator = new AppKit.NSProgressIndicator();
			loadingProgressIndicator.WantsLayer = true;
			loadingProgressIndicator.Style = NSProgressIndicatorStyle.Spinning;
			loadingProgressIndicator.Hidden = true;
			loadingProgressIndicator.ControlSize = NSControlSize.Small;
			
			this.ContentView.AddSubview (loadingProgressIndicator);
			loadingProgressIndicator.Frame = loadingProgressIndicator.GetFrameForAlignmentRect (new CoreGraphics.CGRect (396f,263f,18f,18f));;
			
			
			// View:     versionPopUp
			// NodeName: "versionPopUp"
			// NodeType: INSTANCE
			// NodeId:   754:103
			
			versionComboBox = new AppKit.NSPopUpButton();
			versionComboBox.WantsLayer = true;
			versionComboBox.BezelStyle = NSBezelStyle.Rounded;
			versionComboBox.ControlSize = NSControlSize.Regular;
			versionComboBox.AddItem ("Current");
			
			this.ContentView.AddSubview (versionComboBox);
			versionComboBox.Frame = versionComboBox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (167f,261f,223f,21f));;
			
			
			// View:     nativeViewCodeServiceView4
			// NodeName: Version:
			// NodeType: INSTANCE
			// NodeId:   754:15
			
			var nativeViewCodeServiceView4 = new AppKit.NSTextField() {    StringValue = "Version:",
			Editable = false,
			Bordered = false,
			Bezeled = false,
			DrawsBackground = false,
			Alignment = NSTextAlignment.Left,
			};
			nativeViewCodeServiceView4.WantsLayer = true;
			nativeViewCodeServiceView4.Alignment = NSTextAlignment.Right;
			
			this.ContentView.AddSubview (nativeViewCodeServiceView4);
			nativeViewCodeServiceView4.Frame = nativeViewCodeServiceView4.GetFrameForAlignmentRect (new CoreGraphics.CGRect (21f,262f,142f,20f));;
			
			
			// View:     figmaUrlTextField
			// NodeName: "figmaUrlTextField"
			// NodeType: INSTANCE
			// NodeId:   754:192
			
			figmaUrlTextField = new AppKit.NSTextField();
			figmaUrlTextField.WantsLayer = true;
			figmaUrlTextField.PlaceholderString = "https://www.figma.com/file/";
			
			this.ContentView.AddSubview (figmaUrlTextField);
			figmaUrlTextField.Frame = figmaUrlTextField.GetFrameForAlignmentRect (new CoreGraphics.CGRect (168f,293f,221f,21f));;
			
			
			// View:     nativeViewCodeServiceView5
			// NodeName: Figma URL:
			// NodeType: INSTANCE
			// NodeId:   754:13
			
			var nativeViewCodeServiceView5 = new AppKit.NSTextField() {    StringValue = "Figma URL:",
			Editable = false,
			Bordered = false,
			Bezeled = false,
			DrawsBackground = false,
			Alignment = NSTextAlignment.Left,
			};
			nativeViewCodeServiceView5.WantsLayer = true;
			nativeViewCodeServiceView5.Alignment = NSTextAlignment.Right;
			
			this.ContentView.AddSubview (nativeViewCodeServiceView5);
			nativeViewCodeServiceView5.Frame = nativeViewCodeServiceView5.GetFrameForAlignmentRect (new CoreGraphics.CGRect (21f,293f,142f,20f));;
			
		}
	}
}
