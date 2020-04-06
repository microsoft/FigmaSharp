// This file was auto-generated using
// FigmaSharp 1.0.0.0 and Figma API 1.0.1 on 2020-04-06 at 02:59
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
	partial class FigmaPackageWindow
	{
		private AppKit.NSButton bundleButton, cancelButton, translationsCheckbox, includeOriginalCheckbox, nothingRadio, templateRadio, codeRadio;
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
			frame.Size = new CoreGraphics.CGSize (481f, 379f);
			this.SetFrame (frame, true);
			this.ContentMinSize = this.ContentView.Frame.Size;

			// View:     bundleButton
			// NodeName: "bundleButton"
			// NodeType: INSTANCE
			// NodeId:   680:549
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
			// NodeId:   680:564
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

			// View:     nativeViewCodeServiceView
			// NodeName: sep
			// NodeType: VECTOR
			// NodeId:   622:0
			var nativeViewCodeServiceView = new AppKit.NSBox();
			nativeViewCodeServiceView.WantsLayer = true;
			nativeViewCodeServiceView.BoxType = NSBoxType.NSBoxSeparator;

			this.ContentView.AddSubview (nativeViewCodeServiceView);
			nativeViewCodeServiceView.Frame = nativeViewCodeServiceView.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f, 59.5f, 481f, 0f));;

			// View:     translationsCheckbox
			// NodeName: "translationsCheckbox"
			// NodeType: INSTANCE
			// NodeId:   974:0
			translationsCheckbox = new AppKit.NSButton();
			translationsCheckbox.WantsLayer = true;
			translationsCheckbox.BezelStyle = NSBezelStyle.Rounded;
			translationsCheckbox.SetButtonType (NSButtonType.Switch);
			translationsCheckbox.ControlSize = NSControlSize.Regular;
			translationsCheckbox.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			translationsCheckbox.Title = "Make strings translatable";
			translationsCheckbox.State = NSCellStateValue.On;

			this.ContentView.AddSubview (translationsCheckbox);
			translationsCheckbox.Frame = translationsCheckbox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (169f, 103f, 238f, 14f));;

			// View:     includeOriginalCheckbox
			// NodeName: "includeOriginalCheckbox"
			// NodeType: INSTANCE
			// NodeId:   527:46
			includeOriginalCheckbox = new AppKit.NSButton();
			includeOriginalCheckbox.WantsLayer = true;
			includeOriginalCheckbox.BezelStyle = NSBezelStyle.Rounded;
			includeOriginalCheckbox.SetButtonType (NSButtonType.Switch);
			includeOriginalCheckbox.ControlSize = NSControlSize.Regular;
			includeOriginalCheckbox.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			includeOriginalCheckbox.Title = "Include original Figma document";
			includeOriginalCheckbox.State = NSCellStateValue.On;

			this.ContentView.AddSubview (includeOriginalCheckbox);
			includeOriginalCheckbox.Frame = includeOriginalCheckbox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (169f, 81f, 238f, 14f));;

			// View:     generateRadios
			// NodeName: "generateRadios"
			// NodeType: FRAME
			// NodeId:   577:0
			var generateRadios = new NSView();
			generateRadios.WantsLayer = true;

			this.ContentView.AddSubview (generateRadios);
			generateRadios.Frame = generateRadios.GetFrameForAlignmentRect (new CoreGraphics.CGRect (169f, 134f, 220f, 60f));;

			// View:     nothingRadio
			// NodeName: radio "nothingRadio"
			// NodeType: INSTANCE
			// NodeId:   576:0
			nothingRadio = new AppKit.NSButton();
			nothingRadio.WantsLayer = true;
			nothingRadio.BezelStyle = NSBezelStyle.Rounded;
			nothingRadio.SetButtonType (NSButtonType.Radio);
			nothingRadio.ControlSize = NSControlSize.Regular;
			nothingRadio.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			nothingRadio.Title = "Nothing";

			generateRadios.AddSubview (nothingRadio);
			nothingRadio.Frame = nothingRadio.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f, 0f, 89f, 16f));;

			// View:     templateRadio
			// NodeName: radio "templateRadio"
			// NodeType: INSTANCE
			// NodeId:   557:20
			templateRadio = new AppKit.NSButton();
			templateRadio.WantsLayer = true;
			templateRadio.BezelStyle = NSBezelStyle.Rounded;
			templateRadio.SetButtonType (NSButtonType.Radio);
			templateRadio.ControlSize = NSControlSize.Regular;
			templateRadio.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			templateRadio.Title = "Template";

			generateRadios.AddSubview (templateRadio);
			templateRadio.Frame = templateRadio.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f, 22f, 89f, 16f));;

			// View:     codeRadio
			// NodeName: radio "codeRadio"
			// NodeType: INSTANCE
			// NodeId:   557:11
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
			nativeViewCodeServiceView1.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			nativeViewCodeServiceView1.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (nativeViewCodeServiceView1);
			nativeViewCodeServiceView1.Frame = nativeViewCodeServiceView1.GetFrameForAlignmentRect (new CoreGraphics.CGRect (21f, 176f, 142f, 20f));;

			// View:     namespacePopUp
			// NodeName: "namespacePopUp"
			// NodeType: INSTANCE
			// NodeId:   576:78
			namespacePopUp = new AppKit.NSComboBox();
			namespacePopUp.WantsLayer = true;
			namespacePopUp.Add (new Foundation.NSString ("MyApp"));

			this.ContentView.AddSubview (namespacePopUp);
			namespacePopUp.Frame = namespacePopUp.GetFrameForAlignmentRect (new CoreGraphics.CGRect (169f, 209f, 220f, 20f));;

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
			nativeViewCodeServiceView2.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			nativeViewCodeServiceView2.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (nativeViewCodeServiceView2);
			nativeViewCodeServiceView2.Frame = nativeViewCodeServiceView2.GetFrameForAlignmentRect (new CoreGraphics.CGRect (21f, 210f, 142f, 20f));;

			// View:     nativeViewCodeServiceView3
			// NodeName: sep
			// NodeType: VECTOR
			// NodeId:   209:132
			var nativeViewCodeServiceView3 = new AppKit.NSBox();
			nativeViewCodeServiceView3.WantsLayer = true;
			nativeViewCodeServiceView3.BoxType = NSBoxType.NSBoxSeparator;

			this.ContentView.AddSubview (nativeViewCodeServiceView3);
			nativeViewCodeServiceView3.Frame = nativeViewCodeServiceView3.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f, 254.5f, 481f, 0f));;

			// View:     versionSpinner
			// NodeName: "versionSpinner"
			// NodeType: INSTANCE
			// NodeId:   772:27
			versionSpinner = new AppKit.NSProgressIndicator();
			versionSpinner.WantsLayer = true;
			versionSpinner.Style = NSProgressIndicatorStyle.Spinning;
			versionSpinner.Hidden = true;
			versionSpinner.ControlSize = NSControlSize.Small;

			this.ContentView.AddSubview (versionSpinner);
			versionSpinner.Frame = versionSpinner.GetFrameForAlignmentRect (new CoreGraphics.CGRect (396f, 280f, 18f, 18f));;

			// View:     versionPopUp
			// NodeName: "versionPopUp"
			// NodeType: INSTANCE
			// NodeId:   754:103
			versionPopUp = new AppKit.NSPopUpButton();
			versionPopUp.WantsLayer = true;
			versionPopUp.BezelStyle = NSBezelStyle.Rounded;
			versionPopUp.ControlSize = NSControlSize.Regular;
			versionPopUp.AddItem ("Current");

			this.ContentView.AddSubview (versionPopUp);
			versionPopUp.Frame = versionPopUp.GetFrameForAlignmentRect (new CoreGraphics.CGRect (167f, 278f, 223f, 21f));;

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
			nativeViewCodeServiceView4.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			nativeViewCodeServiceView4.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (nativeViewCodeServiceView4);
			nativeViewCodeServiceView4.Frame = nativeViewCodeServiceView4.GetFrameForAlignmentRect (new CoreGraphics.CGRect (21f, 279f, 142f, 20f));;

			// View:     figmaUrlTextField
			// NodeName: "figmaUrlTextField"
			// NodeType: INSTANCE
			// NodeId:   754:192
			figmaUrlTextField = new AppKit.NSTextField();
			figmaUrlTextField.WantsLayer = true;
			figmaUrlTextField.PlaceholderString = "https://www.figma.com/file/";

			this.ContentView.AddSubview (figmaUrlTextField);
			figmaUrlTextField.Frame = figmaUrlTextField.GetFrameForAlignmentRect (new CoreGraphics.CGRect (168f, 310f, 221f, 21f));;

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
			nativeViewCodeServiceView5.Font = AppKit.NSFont.SystemFontOfSize (AppKit.NSFont.SystemFontSize);;
			nativeViewCodeServiceView5.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (nativeViewCodeServiceView5);
			nativeViewCodeServiceView5.Frame = nativeViewCodeServiceView5.GetFrameForAlignmentRect (new CoreGraphics.CGRect (21f, 310f, 142f, 20f));;

		}
	}
}
