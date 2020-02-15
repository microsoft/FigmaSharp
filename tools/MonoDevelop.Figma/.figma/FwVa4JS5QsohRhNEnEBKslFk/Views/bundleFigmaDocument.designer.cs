/*
This file was auto-generated using
FigmaSharp 1.0.0.0 and Figma API 1.0.1 on 2020-02-10 00:55 at 00:55

Document title:   macOS Components
Document version: 0.1
Document URL:     1234

Changes to this file may cause incorrect behavior
and will be lost if the code is regenerated.
* 
*/
using AppKit;
namespace MonoDevelop.Figma.FigmaBundles
{
	partial class bundleFigmaDocument
	{
		NSTextField figmaUrlTextField;
		NSPopUpButton versionComboBox;
		NSButton templateCodeOptionBox, templateNoneOptionBox, templateMarkUpOptionBox;
		NSProgressIndicator loadingProgressIndicator;
		NSButton bundleButton, cancelButton;

		private void InitializeComponent ()
		{
			this.StyleMask |= NSWindowStyle.Closable;
			this.Title = "Bundle Figma Document";
			var frame = Frame;
			frame.Size = new CoreGraphics.CGSize (481f, 334f);
			this.SetFrame (frame, true);

			// View: bundleButton NodeName: Button/Standard "bundleButton" a11y-label:"Start the bundling process" NodeType: INSTANCE NodeId: 15:393

			bundleButton = new AppKit.NSButton ();
			bundleButton.WantsLayer = true;
			bundleButton.BezelStyle = NSBezelStyle.Rounded;
			bundleButton.ControlSize = NSControlSize.Regular;
			bundleButton.Title = "Bundle";
			bundleButton.KeyEquivalent = "\r";

			bundleButton.AccessibilityTitle = "Start the bundling process";

			this.ContentView.AddSubview (bundleButton);
			bundleButton.Frame = bundleButton.GetFrameForAlignmentRect (new CoreGraphics.CGRect (379f, 21f, 82f, 19f)); ;

			// View: cancelButton NodeName: Button/Standard "cancelButton" NodeType: INSTANCE NodeId: 15:394

			cancelButton = new AppKit.NSButton ();
			cancelButton.WantsLayer = true;
			cancelButton.BezelStyle = NSBezelStyle.Rounded;
			cancelButton.ControlSize = NSControlSize.Regular;
			cancelButton.Title = "Cancel";

			this.ContentView.AddSubview (cancelButton);
			cancelButton.Frame = cancelButton.GetFrameForAlignmentRect (new CoreGraphics.CGRect (20f, 21f, 82f, 19f)); ;

			// View: nativeViewCodeServiceView NodeName: sep NodeType: VECTOR NodeId: 15:395

			var nativeViewCodeServiceView = new AppKit.NSBox ();
			nativeViewCodeServiceView.WantsLayer = true;
			nativeViewCodeServiceView.BoxType = NSBoxType.NSBoxSeparator;

			this.ContentView.AddSubview (nativeViewCodeServiceView);
			nativeViewCodeServiceView.Frame = nativeViewCodeServiceView.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f, 61.5f, 481f, 0f)); ;

			// View: templateNoneOptionBox NodeName: Radio/Standard "templateNoneOptionBox" a11y-label:"Don't generate anything, just include the document" NodeType: INSTANCE NodeId: 15:398

			templateNoneOptionBox = new AppKit.NSButton ();
			templateNoneOptionBox.WantsLayer = true;
			templateNoneOptionBox.BezelStyle = NSBezelStyle.Rounded;
			templateNoneOptionBox.SetButtonType (NSButtonType.Radio);
			templateNoneOptionBox.WantsLayer = true;
			templateNoneOptionBox.ControlSize = NSControlSize.Regular;
			templateNoneOptionBox.Title = "None";

			templateNoneOptionBox.AccessibilityTitle = "Don't generate anything, just include the document";

			this.ContentView.AddSubview (templateNoneOptionBox);
			templateNoneOptionBox.Frame = templateNoneOptionBox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (166f, 91.99997f, 56.93478f, 18.13334f)); ;

			// View: templateMarkUpOptionBox NodeName: Radio/Standard "templateMarkUpOptionBox" a11y-label:"Generate a template file" NodeType: INSTANCE NodeId: 15:399

			templateMarkUpOptionBox = new AppKit.NSButton ();
			templateMarkUpOptionBox.WantsLayer = true;
			templateMarkUpOptionBox.BezelStyle = NSBezelStyle.Rounded;
			templateMarkUpOptionBox.SetButtonType (NSButtonType.Radio);
			templateMarkUpOptionBox.WantsLayer = true;
			templateMarkUpOptionBox.ControlSize = NSControlSize.Regular;
			templateMarkUpOptionBox.Title = "Template";

			templateMarkUpOptionBox.AccessibilityTitle = "Generate a template file";

			this.ContentView.AddSubview (templateMarkUpOptionBox);
			templateMarkUpOptionBox.Frame = templateMarkUpOptionBox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (166f, 114.9333f, 97f, 18.13334f)); ;

			// View: templateCodeOptionBox NodeName: Radio/Standard "templateCodeOptionBox" a11y-label:"Generate code" NodeType: INSTANCE NodeId: 15:400

			templateCodeOptionBox = new AppKit.NSButton ();
			templateCodeOptionBox.WantsLayer = true;
			templateCodeOptionBox.BezelStyle = NSBezelStyle.Rounded;
			templateCodeOptionBox.SetButtonType (NSButtonType.Radio);
			templateCodeOptionBox.WantsLayer = true;
			templateCodeOptionBox.ControlSize = NSControlSize.Regular;
			templateCodeOptionBox.Title = "Code";
			templateCodeOptionBox.State = NSCellStateValue.On;

			templateCodeOptionBox.AccessibilityTitle = "Generate code";

			this.ContentView.AddSubview (templateCodeOptionBox);
			templateCodeOptionBox.Frame = templateCodeOptionBox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (166f, 137.8667f, 56.93478f, 18.13334f)); ;

			// View: nativeViewCodeServiceView1 NodeName: Template: NodeType: TEXT NodeId: 15:401

			var nativeViewCodeServiceView1 = new AppKit.NSTextField () {
				StringValue = "Template:",
				Editable = false,
				Bordered = false,
				Bezeled = false,
				DrawsBackground = false,
				Alignment = NSTextAlignment.Left,
			};
			nativeViewCodeServiceView1.WantsLayer = true;
			nativeViewCodeServiceView1.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (nativeViewCodeServiceView1);
			nativeViewCodeServiceView1.Frame = nativeViewCodeServiceView1.GetFrameForAlignmentRect (new CoreGraphics.CGRect (58f, 136f, 105f, 18f)); ;

			// View: nativeViewCodeServiceView2 NodeName: Output NodeType: TEXT NodeId: 15:402

			var nativeViewCodeServiceView2 = new AppKit.NSTextField () {
				StringValue = "Output",
				Editable = false,
				Bordered = false,
				Bezeled = false,
				DrawsBackground = false,
				Alignment = NSTextAlignment.Left,
			};
			nativeViewCodeServiceView2.WantsLayer = true;

			this.ContentView.AddSubview (nativeViewCodeServiceView2);
			nativeViewCodeServiceView2.Frame = nativeViewCodeServiceView2.GetFrameForAlignmentRect (new CoreGraphics.CGRect (20f, 147f, 105f, 35f)); ;

			// View: nativeViewCodeServiceView3 NodeName: sep NodeType: VECTOR NodeId: 15:403

			var nativeViewCodeServiceView3 = new AppKit.NSBox ();
			nativeViewCodeServiceView3.WantsLayer = true;
			nativeViewCodeServiceView3.BoxType = NSBoxType.NSBoxSeparator;

			this.ContentView.AddSubview (nativeViewCodeServiceView3);
			nativeViewCodeServiceView3.Frame = nativeViewCodeServiceView3.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f, 199.5f, 481f, 0f)); ;

			// View: loadingProgressIndicator NodeName: Progress Spinner/Small "loadingProgressIndicator" NodeType: INSTANCE NodeId: 15:404

			loadingProgressIndicator = new AppKit.NSProgressIndicator ();
			loadingProgressIndicator.WantsLayer = true;
			loadingProgressIndicator.Style = NSProgressIndicatorStyle.Spinning;
			loadingProgressIndicator.Hidden = true;
			loadingProgressIndicator.ControlSize = NSControlSize.Small;

			this.ContentView.AddSubview (loadingProgressIndicator);
			loadingProgressIndicator.Frame = loadingProgressIndicator.GetFrameForAlignmentRect (new CoreGraphics.CGRect (396f, 225f, 18f, 18f)); ;

			// View: versionComboBox NodeName: popup "versionComboBox" a11y-label:"The version of the document" NodeType: INSTANCE NodeId: 15:406

			versionComboBox = new AppKit.NSPopUpButton ();
			versionComboBox.WantsLayer = true;
			versionComboBox.BezelStyle = NSBezelStyle.Rounded;
			versionComboBox.ControlSize = NSControlSize.Regular;

			versionComboBox.AccessibilityTitle = "The version of the document";

			this.ContentView.AddSubview (versionComboBox);
			versionComboBox.Frame = versionComboBox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (168f, 224f, 220f, 19f)); ;

			// View: nativeViewCodeServiceView4 NodeName: Version: NodeType: TEXT NodeId: 15:407

			var nativeViewCodeServiceView4 = new AppKit.NSTextField () {
				StringValue = "Version:",
				Editable = false,
				Bordered = false,
				Bezeled = false,
				DrawsBackground = false,
				Alignment = NSTextAlignment.Left,
			};
			nativeViewCodeServiceView4.WantsLayer = true;
			nativeViewCodeServiceView4.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (nativeViewCodeServiceView4);
			nativeViewCodeServiceView4.Frame = nativeViewCodeServiceView4.GetFrameForAlignmentRect (new CoreGraphics.CGRect (92f, 207f, 71f, 35f)); ;

			// View: figmaUrlTextField NodeName: textfield "figmaUrlTextField" a11y-label:"The URL from figma.com" NodeType: INSTANCE NodeId: 15:408

			figmaUrlTextField = new AppKit.NSTextField ();
			figmaUrlTextField.WantsLayer = true;
			figmaUrlTextField.PlaceholderString = "https://www.figma.com/file/";

			figmaUrlTextField.AccessibilityTitle = "The URL from figma.com";

			this.ContentView.AddSubview (figmaUrlTextField);
			figmaUrlTextField.Frame = figmaUrlTextField.GetFrameForAlignmentRect (new CoreGraphics.CGRect (168f, 255f, 221f, 21f)); ;

			// View: nativeViewCodeServiceView5 NodeName: Figma URL: NodeType: TEXT NodeId: 15:409

			var nativeViewCodeServiceView5 = new AppKit.NSTextField () {
				StringValue = "Figma URL:",
				Editable = false,
				Bordered = false,
				Bezeled = false,
				DrawsBackground = false,
				Alignment = NSTextAlignment.Left,
			};
			nativeViewCodeServiceView5.WantsLayer = true;
			nativeViewCodeServiceView5.Alignment = NSTextAlignment.Right;

			this.ContentView.AddSubview (nativeViewCodeServiceView5);
			nativeViewCodeServiceView5.Frame = nativeViewCodeServiceView5.GetFrameForAlignmentRect (new CoreGraphics.CGRect (92f, 238f, 71f, 35f)); ;

			// View: nativeViewCodeServiceView6 NodeName: Source NodeType: TEXT NodeId: 15:410

			var nativeViewCodeServiceView6 = new AppKit.NSTextField () {
				StringValue = "Source",
				Editable = false,
				Bordered = false,
				Bezeled = false,
				DrawsBackground = false,
				Alignment = NSTextAlignment.Left,
			};
			nativeViewCodeServiceView6.WantsLayer = true;

			this.ContentView.AddSubview (nativeViewCodeServiceView6);
			nativeViewCodeServiceView6.Frame = nativeViewCodeServiceView6.GetFrameForAlignmentRect (new CoreGraphics.CGRect (20f, 279f, 65f, 19f)); ;

		}
	}
}
