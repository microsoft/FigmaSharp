/*
This file was auto-generated using
FigmaSharp 1.0.0.0 and Figma API 1.0.1 on 2020-02-02 12:35 at 12:35

Document title:   macOS Components
Document version: 0.1
Document URL:     https://www.figma.com/file/fKugSkFGdwOF4vDsPGnJee/

Changes to this file may cause incorrect behavior
and will be lost if the code is regenerated.
* 
*/
using AppKit;
using Foundation;

namespace MonoDevelop.Figma
{
	partial class FigmaBundleWindow
	{
		public NSTextField FigmaUrlTextField;
		public NSPopUpButton VersionComboBox;
		public NSButton TemplateCodeOptionBox, TemplateNoneOptionBox, TemplateMarkUpOptionBox;
		public NSProgressIndicator LoadingProgressIndicator;
		public NSButton BundleButton, CancelButton;

		private void InitializeComponent ()
		{
			var frame = Frame;
			frame.Size = new CoreGraphics.CGSize (481f, 334f);
			SetFrame (frame, true);

			this.StyleMask |= NSWindowStyle.Closable;

			this.Title = "Bundle Figma Document";

			var groupView = new NSView ();
			groupView.WantsLayer = true;
			groupView.SetFrameSize (new CoreGraphics.CGSize (587f, 434f));

			groupView.SetFrameOrigin (new CoreGraphics.CGPoint (-53f, -74f));
			var view1 = new AppKit.NSImageView ();
			view1.WantsLayer = true;
			view1.SetFrameSize (new CoreGraphics.CGSize (103f, 110f));
			view1.Layer.CornerRadius = 0f;

			groupView.AddSubview (view1);
			view1.SetFrameOrigin (new CoreGraphics.CGPoint (0f, 324f));
			var view2 = new AppKit.NSImageView ();
			view2.WantsLayer = true;
			view2.SetFrameSize (new CoreGraphics.CGSize (381f, 110f));
			view2.Layer.CornerRadius = 0f;

			groupView.AddSubview (view2);
			view2.SetFrameOrigin (new CoreGraphics.CGPoint (103f, 324f));
			var view3 = new AppKit.NSImageView ();
			view3.WantsLayer = true;
			view3.SetFrameSize (new CoreGraphics.CGSize (103f, 110f));
			view3.Layer.CornerRadius = 0f;

			groupView.AddSubview (view3);
			view3.SetFrameOrigin (new CoreGraphics.CGPoint (484f, 324f));
			var view4 = new AppKit.NSImageView ();
			view4.WantsLayer = true;
			view4.SetFrameSize (new CoreGraphics.CGSize (103f, 111f));
			view4.Layer.CornerRadius = 0f;

			groupView.AddSubview (view4);
			view4.SetFrameOrigin (new CoreGraphics.CGPoint (0f, 0f));
			var view5 = new AppKit.NSImageView ();
			view5.WantsLayer = true;
			view5.SetFrameSize (new CoreGraphics.CGSize (381f, 111f));
			view5.Layer.CornerRadius = 0f;

			groupView.AddSubview (view5);
			view5.SetFrameOrigin (new CoreGraphics.CGPoint (103f, 0f));
			var view6 = new AppKit.NSImageView ();
			view6.WantsLayer = true;
			view6.SetFrameSize (new CoreGraphics.CGSize (103f, 111f));
			view6.Layer.CornerRadius = 0f;

			groupView.AddSubview (view6);
			view6.SetFrameOrigin (new CoreGraphics.CGPoint (484f, 0f));
			var view7 = new AppKit.NSImageView ();
			view7.WantsLayer = true;
			view7.SetFrameSize (new CoreGraphics.CGSize (103f, 213f));
			view7.Layer.CornerRadius = 0f;

			groupView.AddSubview (view7);
			view7.SetFrameOrigin (new CoreGraphics.CGPoint (0f, 111f));
			var view8 = new AppKit.NSImageView ();
			view8.WantsLayer = true;
			view8.SetFrameSize (new CoreGraphics.CGSize (103f, 213f));
			view8.Layer.CornerRadius = 0f;

			groupView.AddSubview (view8);
			view8.SetFrameOrigin (new CoreGraphics.CGPoint (484f, 111f));
			var view9 = new AppKit.NSImageView ();
			view9.WantsLayer = true;
			view9.SetFrameSize (new CoreGraphics.CGSize (381f, 213f));
			view9.Layer.BackgroundColor = NSColor.FromRgba (0.9607843f, 0.9607843f, 0.9607843f, 1f).CGColor;
			view9.Layer.CornerRadius = 0f;

			groupView.AddSubview (view9);
			view9.SetFrameOrigin (new CoreGraphics.CGPoint (103f, 111f));
		
			var frameEntityView1 = new NSView ();
			frameEntityView1.WantsLayer = true;
			frameEntityView1.SetFrameSize (new CoreGraphics.CGSize (481f, 312f));

			ContentView.AddSubview (frameEntityView1);
			frameEntityView1.SetFrameOrigin (new CoreGraphics.CGPoint (0f, 0f));
			BundleButton = new AppKit.NSButton ();
			BundleButton.WantsLayer = true;
	
			BundleButton.BezelStyle = NSBezelStyle.Rounded;
			BundleButton.ControlSize = NSControlSize.Regular;
			BundleButton.Title = "Bundle";

			frameEntityView1.AddSubview (BundleButton);
			BundleButton.Frame = BundleButton.GetFrameForAlignmentRect (new CoreGraphics.CGRect (379f, 21f, 82f, 19f));

			CancelButton = new AppKit.NSButton ();
			CancelButton.WantsLayer = true;
			CancelButton.BezelStyle = NSBezelStyle.Rounded;
			CancelButton.ControlSize = NSControlSize.Regular;
			CancelButton.Title = "Cancel";
			
			frameEntityView1.AddSubview (CancelButton);
			CancelButton.Frame = CancelButton.GetFrameForAlignmentRect (new CoreGraphics.CGRect (20f, 21f, 82f, 19f));
		
			var vectorEntityView = new AppKit.NSBox ();
			vectorEntityView.WantsLayer = true;
			vectorEntityView.SetFrameSize (new CoreGraphics.CGSize (481f, 1f));
			vectorEntityView.BoxType = NSBoxType.NSBoxSeparator;

			frameEntityView1.AddSubview (vectorEntityView);
			vectorEntityView.SetFrameOrigin (new CoreGraphics.CGPoint (0f, 61.5f));
			var groupView1 = new NSView ();
			groupView1.WantsLayer = true;
			groupView1.SetFrameSize (new CoreGraphics.CGSize (330f, 59f));

			frameEntityView1.AddSubview (groupView1);
			groupView1.SetFrameOrigin (new CoreGraphics.CGPoint (58f, 95f));
			var groupView2 = new NSView ();
			groupView2.WantsLayer = true;
			groupView2.SetFrameSize (new CoreGraphics.CGSize (219f, 58f));

			groupView1.AddSubview (groupView2);
			groupView2.SetFrameOrigin (new CoreGraphics.CGPoint (111f, 0f));
			TemplateNoneOptionBox = new AppKit.NSButton ();
			TemplateNoneOptionBox.WantsLayer = true;
			TemplateNoneOptionBox.BezelStyle = NSBezelStyle.Rounded;
			TemplateNoneOptionBox.SetButtonType (NSButtonType.Radio);
			TemplateNoneOptionBox.WantsLayer = true;
			TemplateNoneOptionBox.Title = "None";
			groupView2.AddSubview (TemplateNoneOptionBox);
			TemplateNoneOptionBox.Frame = TemplateNoneOptionBox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0, 0, 219f, 14f));

			TemplateMarkUpOptionBox = new AppKit.NSButton ();
			TemplateMarkUpOptionBox.WantsLayer = true;
			TemplateMarkUpOptionBox.BezelStyle = NSBezelStyle.Rounded;
			TemplateMarkUpOptionBox.SetButtonType (NSButtonType.Radio);
			TemplateMarkUpOptionBox.WantsLayer = true;
			TemplateMarkUpOptionBox.Title = "Markup";
			groupView2.AddSubview (TemplateMarkUpOptionBox);
			TemplateMarkUpOptionBox.Frame = TemplateMarkUpOptionBox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f, 22f, 219f, 14f));

			TemplateCodeOptionBox = new AppKit.NSButton ();
			TemplateCodeOptionBox.WantsLayer = true;
			TemplateCodeOptionBox.BezelStyle = NSBezelStyle.Rounded;
			TemplateCodeOptionBox.SetButtonType (NSButtonType.Radio);
			TemplateCodeOptionBox.WantsLayer = true;
			TemplateCodeOptionBox.Title = "Code";
			groupView2.AddSubview (TemplateCodeOptionBox);
			TemplateCodeOptionBox.Frame = TemplateCodeOptionBox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f, 44f, 219f, 14f));

			var textView1 = new AppKit.NSTextField () {
				StringValue = "Template:",
				Editable = false,
				Bordered = false,
				Bezeled = false,
				DrawsBackground = false,
				Alignment = NSTextAlignment.Left,
			};
			textView1.Font = NSFontManager.SharedFontManager.FontWithFamily ("SF Pro Text", default (NSFontTraitMask), 5, 13);
			textView1.Alignment = NSTextAlignment.Right;
			textView1.AlphaValue = 1f;
			textView1.TextColor = NSColor.FromRgba (0f, 0f, 0f, 1f);
			textView1.WantsLayer = true;
			textView1.SetFrameSize (new CoreGraphics.CGSize (105f, 18f));

			groupView1.AddSubview (textView1);
			textView1.SetFrameOrigin (new CoreGraphics.CGPoint (0f, 41f));
			var textView2 = new AppKit.NSTextField () {
				StringValue = "Output",
				Editable = false,
				Bordered = false,
				Bezeled = false,
				DrawsBackground = false,
				Alignment = NSTextAlignment.Left,
			};
			textView2.Font = NSFontManager.SharedFontManager.FontWithFamily ("SF Pro Text", default (NSFontTraitMask), 6, 13);
			textView2.Alignment = NSTextAlignment.Left;
			textView2.AlphaValue = 1f;
			textView2.TextColor = NSColor.FromRgba (0f, 0f, 0f, 1f);
			textView2.WantsLayer = true;
			textView2.SetFrameSize (new CoreGraphics.CGSize (105f, 35f));

			frameEntityView1.AddSubview (textView2);
			textView2.SetFrameOrigin (new CoreGraphics.CGPoint (20f, 147f));
			var vectorEntityView1 = new AppKit.NSBox ();
			vectorEntityView1.WantsLayer = true;
			vectorEntityView1.SetFrameSize (new CoreGraphics.CGSize (481f, 1f));
			vectorEntityView1.BoxType = NSBoxType.NSBoxSeparator;

			frameEntityView1.AddSubview (vectorEntityView1);
			vectorEntityView1.SetFrameOrigin (new CoreGraphics.CGPoint (0f, 199.5f));


			LoadingProgressIndicator = new AppKit.NSProgressIndicator ();
			LoadingProgressIndicator.WantsLayer = true;
			LoadingProgressIndicator.Style = NSProgressIndicatorStyle.Spinning;
			LoadingProgressIndicator.Hidden = true;

			frameEntityView1.AddSubview (LoadingProgressIndicator);
			LoadingProgressIndicator.Frame = LoadingProgressIndicator.GetFrameForAlignmentRect (new CoreGraphics.CGRect (396f, 225f, 18f, 18f));

			var groupView3 = new NSView ();
			groupView3.WantsLayer = true;
			groupView3.SetFrameSize (new CoreGraphics.CGSize (297f, 69f));

			frameEntityView1.AddSubview (groupView3);
			groupView3.SetFrameOrigin (new CoreGraphics.CGPoint (92f, 207f));

			VersionComboBox = new AppKit.NSPopUpButton ();
			VersionComboBox.WantsLayer = true;
			VersionComboBox.BezelStyle = NSBezelStyle.Rounded;
			VersionComboBox.ControlSize = NSControlSize.Regular;
			groupView3.AddSubview (VersionComboBox);
			VersionComboBox.Frame = VersionComboBox.GetFrameForAlignmentRect (new CoreGraphics.CGRect (76f, 17f, 220f, 19f));

			var textView3 = new AppKit.NSTextField () {
				StringValue = "Version:",
				Editable = false,
				Bordered = false,
				Bezeled = false,
				DrawsBackground = false,
				Alignment = NSTextAlignment.Left,
			};
			textView3.Font = NSFontManager.SharedFontManager.FontWithFamily ("SF Pro Text", default (NSFontTraitMask), 5, 13);
			textView3.Alignment = NSTextAlignment.Right;
			textView3.AlphaValue = 1f;
			textView3.WantsLayer = true;

			groupView3.AddSubview (textView3);
			textView3.SetFrameSize (new CoreGraphics.CGSize (71f, 35f));

			FigmaUrlTextField = new AppKit.NSTextField ();
			FigmaUrlTextField.WantsLayer = true;
			FigmaUrlTextField.Font = NSFontManager.SharedFontManager.FontWithFamily ("SF Pro Text", default (NSFontTraitMask), 5, 13);
			FigmaUrlTextField.Alignment = NSTextAlignment.Left;
			FigmaUrlTextField.AlphaValue = 1f;
			FigmaUrlTextField.PlaceholderString = "https://www.figma.com/file/";
			groupView3.AddSubview (FigmaUrlTextField);
			FigmaUrlTextField.Frame = FigmaUrlTextField.GetFrameForAlignmentRect (new CoreGraphics.CGRect (76f, 48f, 221f, 21f));

			var textView4 = new AppKit.NSTextField () {
				StringValue = "Figma URL:",
				Editable = false,
				Bordered = false,
				Bezeled = false,
				DrawsBackground = false,
				Alignment = NSTextAlignment.Left,
			};
			textView4.Font = NSFontManager.SharedFontManager.FontWithFamily ("SF Pro Text", default (NSFontTraitMask), 5, 13);
			textView4.Alignment = NSTextAlignment.Right;
			textView4.AlphaValue = 1f;
			textView4.TextColor = NSColor.FromRgba (0f, 0f, 0f, 1f);
			textView4.WantsLayer = true;
			groupView3.AddSubview (textView4);
			textView4.Frame = textView4.GetFrameForAlignmentRect (new CoreGraphics.CGRect (0f, 31f, 71f, 35f));

			var textView5 = new AppKit.NSTextField () {
				StringValue = "Source",
				Editable = false,
				Bordered = false,
				Bezeled = false,
				DrawsBackground = false,
				Alignment = NSTextAlignment.Left,
			};
			textView5.Font = NSFontManager.SharedFontManager.FontWithFamily ("SF Pro Text", default (NSFontTraitMask), 6, 13);
			textView5.Alignment = NSTextAlignment.Left;
			textView5.AlphaValue = 1f;
			textView5.TextColor = NSColor.FromRgba (0f, 0f, 0f, 1f);
			textView5.WantsLayer = true;
			textView5.SetFrameSize (new CoreGraphics.CGSize (65f, 19f));

			frameEntityView1.AddSubview (textView5);
			textView5.SetFrameOrigin (new CoreGraphics.CGPoint (20f, 279f));

			//initial states
			BundleButton.KeyEquivalent = "\r";

			TemplateCodeOptionBox.State = NSCellStateValue.On;
			TemplateMarkUpOptionBox.Enabled =
			TemplateNoneOptionBox.Enabled = false;

			InitialFirstResponder = FigmaUrlTextField;
		}
	}
}
