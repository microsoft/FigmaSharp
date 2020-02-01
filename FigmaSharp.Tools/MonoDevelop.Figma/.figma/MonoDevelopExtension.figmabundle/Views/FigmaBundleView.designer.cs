/*
This file was auto-generated using
FigmaSharp 1.0.0.0 and Figma API 1.0.1 on 2020-01-31 14:12 at 14:12

Document title:   macOS Components
Document version: 0.1
Document URL:     https://www.figma.com/file/fKugSkFGdwOF4vDsPGnJee/

Changes to this file may cause incorrect behavior
and will be lost if the code is regenerated.
* 
*/
using AppKit;
namespace FigmaSharp
{
	partial class FigmaBundleDialogContentView
	{
		public AppKit.NSPopUpButton versionPopupButton;
		public AppKit.NSButton codeOptionButton, markupOptionButton, noneOptionButton;

		private void InitializeComponent ()
		{
			this.WantsLayer = true;
			this.SetFrameSize(new CoreGraphics.CGSize(481f, 334f));
			
			var groupView = new NSView();
			groupView.WantsLayer = true;
			groupView.SetFrameSize(new CoreGraphics.CGSize(587f, 434f));
			
			AddSubview(groupView);
			groupView.SetFrameOrigin(new CoreGraphics.CGPoint(-53f, -74f));
			var view1 = new AppKit.NSImageView();
			view1.WantsLayer = true;
			view1.SetFrameSize(new CoreGraphics.CGSize(103f, 110f));
			view1.Layer.CornerRadius = 0f;
			
			groupView.AddSubview(view1);
			view1.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 324f));
			var view2 = new AppKit.NSImageView();
			view2.WantsLayer = true;
			view2.SetFrameSize(new CoreGraphics.CGSize(381f, 110f));
			view2.Layer.CornerRadius = 0f;
			
			groupView.AddSubview(view2);
			view2.SetFrameOrigin(new CoreGraphics.CGPoint(103f, 324f));
			var view3 = new AppKit.NSImageView();
			view3.WantsLayer = true;
			view3.SetFrameSize(new CoreGraphics.CGSize(103f, 110f));
			view3.Layer.CornerRadius = 0f;
			
			groupView.AddSubview(view3);
			view3.SetFrameOrigin(new CoreGraphics.CGPoint(484f, 324f));
			var view4 = new AppKit.NSImageView();
			view4.WantsLayer = true;
			view4.SetFrameSize(new CoreGraphics.CGSize(103f, 111f));
			view4.Layer.CornerRadius = 0f;
			
			groupView.AddSubview(view4);
			view4.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 0f));
			var view5 = new AppKit.NSImageView();
			view5.WantsLayer = true;
			view5.SetFrameSize(new CoreGraphics.CGSize(381f, 111f));
			view5.Layer.CornerRadius = 0f;
			
			groupView.AddSubview(view5);
			view5.SetFrameOrigin(new CoreGraphics.CGPoint(103f, 0f));
			var view6 = new AppKit.NSImageView();
			view6.WantsLayer = true;
			view6.SetFrameSize(new CoreGraphics.CGSize(103f, 111f));
			view6.Layer.CornerRadius = 0f;
			
			groupView.AddSubview(view6);
			view6.SetFrameOrigin(new CoreGraphics.CGPoint(484f, 0f));
			var view7 = new AppKit.NSImageView();
			view7.WantsLayer = true;
			view7.SetFrameSize(new CoreGraphics.CGSize(103f, 213f));
			view7.Layer.CornerRadius = 0f;
			
			groupView.AddSubview(view7);
			view7.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 111f));
			var view8 = new AppKit.NSImageView();
			view8.WantsLayer = true;
			view8.SetFrameSize(new CoreGraphics.CGSize(103f, 213f));
			view8.Layer.CornerRadius = 0f;
			
			groupView.AddSubview(view8);
			view8.SetFrameOrigin(new CoreGraphics.CGPoint(484f, 111f));
			var view9 = new AppKit.NSImageView();
			view9.WantsLayer = true;
			view9.SetFrameSize(new CoreGraphics.CGSize(381f, 213f));
			view9.Layer.BackgroundColor = NSColor.FromRgba(0.9607843f, 0.9607843f, 0.9607843f, 1f).CGColor;
			view9.Layer.CornerRadius = 0f;
			
			groupView.AddSubview(view9);
			view9.SetFrameOrigin(new CoreGraphics.CGPoint(103f, 111f));
			var view10 = new AppKit.NSImageView();
			view10.Hidden = true;
			view10.WantsLayer = true;
			view10.SetFrameSize(new CoreGraphics.CGSize(441f, 272f));
			view10.Layer.BorderColor = NSColor.FromRgba(1f, 0f, 0f, 1f).CGColor;
			view10.Layer.BorderWidth = 20;
			view10.Layer.CornerRadius = 0f;
			
			AddSubview(view10);
			view10.SetFrameOrigin(new CoreGraphics.CGPoint(20f, 20f));
			var textView = new AppKit.NSTextField() {    StringValue = "Bundle Figma Document",
			Editable = false,
			Bordered = false,
			Bezeled = false,
			DrawsBackground = false,
			Alignment = NSTextAlignment.Left,
			};
			textView.WantsLayer = true;
			textView.SetFrameSize(new CoreGraphics.CGSize(481f, 22f));
			textView.Font = NSFontManager.SharedFontManager.FontWithFamily("SF Pro Text", default(NSFontTraitMask), 5, 13);
			textView.Alignment = NSTextAlignment.Center;
			textView.AlphaValue = 1f;
			textView.TextColor = NSColor.FromRgba(0.3137255f, 0.3137255f, 0.3137255f, 1f);
			
			AddSubview(textView);
			textView.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 312f));
			var frameEntityView1 = new NSView();
			frameEntityView1.WantsLayer = true;
			frameEntityView1.SetFrameSize(new CoreGraphics.CGSize(52f, 12f));
			
			AddSubview(frameEntityView1);
			frameEntityView1.SetFrameOrigin(new CoreGraphics.CGPoint(8f, 317f));
			var elipseView = new AppKit.NSView();
			elipseView.WantsLayer = true;
			elipseView.SetFrameSize(new CoreGraphics.CGSize(12f, 12f));
			
			frameEntityView1.AddSubview(elipseView);
			elipseView.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 0f));
			var elipseView1 = new AppKit.NSView();
			elipseView1.WantsLayer = true;
			elipseView1.SetFrameSize(new CoreGraphics.CGSize(12f, 12f));
			
			frameEntityView1.AddSubview(elipseView1);
			elipseView1.SetFrameOrigin(new CoreGraphics.CGPoint(20f, 0f));
			var elipseView2 = new AppKit.NSView();
			elipseView2.WantsLayer = true;
			elipseView2.SetFrameSize(new CoreGraphics.CGSize(12f, 12f));
			
			frameEntityView1.AddSubview(elipseView2);
			elipseView2.SetFrameOrigin(new CoreGraphics.CGPoint(40f, 0f));
			var elipseView3 = new AppKit.NSView();
			elipseView3.WantsLayer = true;
			elipseView3.SetFrameSize(new CoreGraphics.CGSize(12f, 12f));
			
			frameEntityView1.AddSubview(elipseView3);
			elipseView3.SetFrameOrigin(new CoreGraphics.CGPoint(20f, 0f));
			var elipseView4 = new AppKit.NSView();
			elipseView4.WantsLayer = true;
			elipseView4.SetFrameSize(new CoreGraphics.CGSize(12f, 12f));
			
			frameEntityView1.AddSubview(elipseView4);
			elipseView4.SetFrameOrigin(new CoreGraphics.CGPoint(40f, 0f));
			var frameEntityView2 = new NSView();
			frameEntityView2.WantsLayer = true;
			frameEntityView2.SetFrameSize(new CoreGraphics.CGSize(481f, 312f));

			this.AddSubview(frameEntityView2);
			frameEntityView2.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 0f));
			var view11 = new AppKit.NSButton();
			view11.BezelStyle = NSBezelStyle.Rounded;
			view11.WantsLayer = true;
			view11.SetFrameSize(new CoreGraphics.CGSize(82f, 19f));
			view11.ControlSize = NSControlSize.Regular;
			view11.Title = "";
			
			frameEntityView2.AddSubview(view11);
			view11.SetFrameOrigin(new CoreGraphics.CGPoint(379f, 21f));
			var view12 = new AppKit.NSButton();
			view12.BezelStyle = NSBezelStyle.Rounded;
			view12.WantsLayer = true;
			view12.SetFrameSize(new CoreGraphics.CGSize(82f, 19f));
			view12.ControlSize = NSControlSize.Regular;
			view12.Title = "";
			
			frameEntityView2.AddSubview(view12);
			view12.SetFrameOrigin(new CoreGraphics.CGPoint(20f, 21f));
			var vectorEntityView = new AppKit.NSImageView();
			vectorEntityView.WantsLayer = true;
			vectorEntityView.SetFrameSize(new CoreGraphics.CGSize(481f, 0f));
			vectorEntityView.Layer.BorderColor = NSColor.FromRgba(0.8f, 0.8f, 0.8f, 1f).CGColor;
			vectorEntityView.Layer.BorderWidth = 1;
			
			frameEntityView2.AddSubview(vectorEntityView);
			vectorEntityView.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 61.5f));
			var groupView1 = new NSView();
			groupView1.WantsLayer = true;
			groupView1.SetFrameSize(new CoreGraphics.CGSize(330f, 59f));
			
			frameEntityView2.AddSubview(groupView1);
			groupView1.SetFrameOrigin(new CoreGraphics.CGPoint(58f, 95f));
			var groupView2 = new NSView();
			groupView2.WantsLayer = true;
			groupView2.SetFrameSize(new CoreGraphics.CGSize(219f, 58f));
			
			groupView1.AddSubview(groupView2);
			groupView2.SetFrameOrigin(new CoreGraphics.CGPoint(111f, 0f));
			noneOptionButton = new AppKit.NSButton();
			noneOptionButton.BezelStyle = NSBezelStyle.Rounded;
			noneOptionButton.SetButtonType (NSButtonType.Radio);
			noneOptionButton.WantsLayer = true;
			noneOptionButton.SetFrameSize(new CoreGraphics.CGSize(219f, 14f));
			noneOptionButton.Title = "None";
			
			groupView2.AddSubview(noneOptionButton);
			noneOptionButton.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 0f));
			markupOptionButton = new AppKit.NSButton();
			markupOptionButton.BezelStyle = NSBezelStyle.Rounded;
			markupOptionButton.SetButtonType (NSButtonType.Radio);
			markupOptionButton.WantsLayer = true;
			markupOptionButton.SetFrameSize(new CoreGraphics.CGSize(219f, 14f));
			markupOptionButton.Title = "Markup";
			
			groupView2.AddSubview(markupOptionButton);
			markupOptionButton.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 22f));
			codeOptionButton = new AppKit.NSButton();
			codeOptionButton.BezelStyle = NSBezelStyle.Rounded;
			codeOptionButton.SetButtonType (NSButtonType.Radio);
			codeOptionButton.WantsLayer = true;
			codeOptionButton.SetFrameSize(new CoreGraphics.CGSize(219f, 14f));
			codeOptionButton.Title = "Code";
			
			groupView2.AddSubview(codeOptionButton);
			codeOptionButton.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 44f));
			var textView1 = new AppKit.NSTextField() {    StringValue = "Template:",
			Editable = false,
			Bordered = false,
			Bezeled = false,
			DrawsBackground = false,
			Alignment = NSTextAlignment.Left,
			};
			textView1.WantsLayer = true;
			textView1.SetFrameSize(new CoreGraphics.CGSize(105f, 18f));
			textView1.Font = NSFontManager.SharedFontManager.FontWithFamily("SF Pro Text", default(NSFontTraitMask), 5, 13);
			textView1.Alignment = NSTextAlignment.Right;
			textView1.AlphaValue = 1f;
			textView1.TextColor = NSColor.FromRgba(0f, 0f, 0f, 1f);
			
			groupView1.AddSubview(textView1);
			textView1.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 41f));
			var textView2 = new AppKit.NSTextField() {    StringValue = "Output",
			Editable = false,
			Bordered = false,
			Bezeled = false,
			DrawsBackground = false,
			Alignment = NSTextAlignment.Left,
			};
			textView2.WantsLayer = true;
			textView2.SetFrameSize(new CoreGraphics.CGSize(105f, 35f));
			textView2.Font = NSFontManager.SharedFontManager.FontWithFamily("SF Pro Text", default(NSFontTraitMask), 6, 13);
			textView2.Alignment = NSTextAlignment.Left;
			textView2.AlphaValue = 1f;
			textView2.TextColor = NSColor.FromRgba(0f, 0f, 0f, 1f);
			
			frameEntityView2.AddSubview(textView2);
			textView2.SetFrameOrigin(new CoreGraphics.CGPoint(20f, 147f));
			var vectorEntityView1 = new AppKit.NSImageView();
			vectorEntityView1.WantsLayer = true;
			vectorEntityView1.SetFrameSize(new CoreGraphics.CGSize(481f, 0f));
			vectorEntityView1.Layer.BorderColor = NSColor.FromRgba(0.8f, 0.8f, 0.8f, 1f).CGColor;
			vectorEntityView1.Layer.BorderWidth = 1;
			
			frameEntityView2.AddSubview(vectorEntityView1);
			vectorEntityView1.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 199.5f));
			var view16 = new NSView();
			view16.WantsLayer = true;
			view16.SetFrameSize(new CoreGraphics.CGSize(18f, 18f));
			
			frameEntityView2.AddSubview(view16);
			view16.SetFrameOrigin(new CoreGraphics.CGPoint(396f, 225f));
			var view17 = new AppKit.NSImageView();
			view17.WantsLayer = true;
			view17.SetFrameSize(new CoreGraphics.CGSize(1.384615f, 4.846154f));
			view17.Layer.BackgroundColor = NSColor.FromRgba(0.6784314f, 0.6784314f, 0.6784314f, 1f).CGColor;
			view17.Layer.CornerRadius = 4f;
			
			view16.AddSubview(view17);
			view17.SetFrameOrigin(new CoreGraphics.CGPoint(8.307678f, 13.15385f));
			var view18 = new AppKit.NSImageView();
			view18.WantsLayer = true;
			view18.SetFrameSize(new CoreGraphics.CGSize(1.384615f, 4.846154f));
			view18.Layer.BackgroundColor = NSColor.FromRgba(0.2588235f, 0.2588235f, 0.2588235f, 1f).CGColor;
			view18.Layer.CornerRadius = 4f;
			
			view16.AddSubview(view18);
			view18.SetFrameOrigin(new CoreGraphics.CGPoint(8.307678f, 7.629395E-06f));
			var view19 = new AppKit.NSImageView();
			view19.WantsLayer = true;
			view19.SetFrameSize(new CoreGraphics.CGSize(4.8892f, 3.622189f));
			view19.Layer.BackgroundColor = NSColor.FromRgba(0.7490196f, 0.7490196f, 0.7490196f, 1f).CGColor;
			view19.Layer.CornerRadius = 4f;
			
			view16.AddSubview(view19);
			view19.SetFrameOrigin(new CoreGraphics.CGPoint(0.8596191f, 10.47736f));
			var view20 = new AppKit.NSImageView();
			view20.WantsLayer = true;
			view20.SetFrameSize(new CoreGraphics.CGSize(4.8892f, 3.622189f));
			view20.Layer.BackgroundColor = NSColor.FromRgba(0.4431373f, 0.4431373f, 0.4431373f, 1f).CGColor;
			view20.Layer.CornerRadius = 4f;
			
			view16.AddSubview(view20);
			view20.SetFrameOrigin(new CoreGraphics.CGPoint(12.25116f, 3.900436f));
			var view21 = new AppKit.NSImageView();
			view21.WantsLayer = true;
			view21.SetFrameSize(new CoreGraphics.CGSize(3.622189f, 4.8892f));
			view21.Layer.BackgroundColor = NSColor.FromRgba(0.6313726f, 0.6313726f, 0.6313726f, 1f).CGColor;
			view21.Layer.CornerRadius = 4f;
			
			view16.AddSubview(view21);
			view21.SetFrameOrigin(new CoreGraphics.CGPoint(10.47736f, 12.25117f));
			var view22 = new AppKit.NSImageView();
			view22.WantsLayer = true;
			view22.SetFrameSize(new CoreGraphics.CGSize(3.622189f, 4.8892f));
			view22.Layer.BackgroundColor = NSColor.FromRgba(0.1372549f, 0.1372549f, 0.1372549f, 1f).CGColor;
			view22.Layer.CornerRadius = 4f;
			
			view16.AddSubview(view22);
			view22.SetFrameOrigin(new CoreGraphics.CGPoint(3.900452f, 0.8596115f));
			var view23 = new AppKit.NSImageView();
			view23.WantsLayer = true;
			view23.SetFrameSize(new CoreGraphics.CGSize(4.8892f, 3.622189f));
			view23.Layer.BackgroundColor = NSColor.FromRgba(0.5803922f, 0.5803922f, 0.5803922f, 1f).CGColor;
			view23.Layer.CornerRadius = 4f;
			
			view16.AddSubview(view23);
			view23.SetFrameOrigin(new CoreGraphics.CGPoint(12.25116f, 10.47733f));
			var view24 = new AppKit.NSImageView();
			view24.WantsLayer = true;
			view24.SetFrameSize(new CoreGraphics.CGSize(4.8892f, 3.622189f));
			view24.Layer.BackgroundColor = NSColor.FromRgba(0.04705882f, 0.04705882f, 0.04705882f, 1f).CGColor;
			view24.Layer.CornerRadius = 4f;
			
			view16.AddSubview(view24);
			view24.SetFrameOrigin(new CoreGraphics.CGPoint(0.8596191f, 3.900406f));
			var view25 = new AppKit.NSImageView();
			view25.WantsLayer = true;
			view25.SetFrameSize(new CoreGraphics.CGSize(3.622189f, 4.8892f));
			view25.Layer.BackgroundColor = NSColor.FromRgba(0.3568628f, 0.3568628f, 0.3568628f, 1f).CGColor;
			view25.Layer.CornerRadius = 4f;
			
			view16.AddSubview(view25);
			view25.SetFrameOrigin(new CoreGraphics.CGPoint(10.47736f, 0.8595581f));
			var view26 = new AppKit.NSImageView();
			view26.WantsLayer = true;
			view26.SetFrameSize(new CoreGraphics.CGSize(3.622189f, 4.8892f));
			view26.Layer.BackgroundColor = NSColor.FromRgba(0.7137255f, 0.7137255f, 0.7137255f, 1f).CGColor;
			view26.Layer.CornerRadius = 4f;
			
			view16.AddSubview(view26);
			view26.SetFrameOrigin(new CoreGraphics.CGPoint(3.900452f, 12.25111f));
			var view27 = new AppKit.NSImageView();
			view27.WantsLayer = true;
			view27.SetFrameSize(new CoreGraphics.CGSize(4.846154f, 1.384615f));
			view27.Layer.BackgroundColor = NSColor.FromRgba(0.772549f, 0.772549f, 0.772549f, 1f).CGColor;
			view27.Layer.CornerRadius = 4f;
			
			view16.AddSubview(view27);
			view27.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 8.307693f));
			var view28 = new AppKit.NSImageView();
			view28.WantsLayer = true;
			view28.SetFrameSize(new CoreGraphics.CGSize(4.846154f, 1.384615f));
			view28.Layer.BackgroundColor = NSColor.FromRgba(0.5176471f, 0.5176471f, 0.5176471f, 1f).CGColor;
			view28.Layer.CornerRadius = 4f;
			
			view16.AddSubview(view28);
			view28.SetFrameOrigin(new CoreGraphics.CGPoint(13.15387f, 8.307693f));
			var groupView3 = new NSView();
			groupView3.WantsLayer = true;
			groupView3.SetFrameSize(new CoreGraphics.CGSize(297f, 69f));
			
			frameEntityView2.AddSubview(groupView3);
			groupView3.SetFrameOrigin(new CoreGraphics.CGPoint(92f, 207f));
			versionPopupButton = new AppKit.NSPopUpButton();
			versionPopupButton.BezelStyle = NSBezelStyle.Rounded;
			versionPopupButton.WantsLayer = true;
			versionPopupButton.SetFrameSize(new CoreGraphics.CGSize(220f, 19f));
			versionPopupButton.ControlSize = NSControlSize.Regular;
			versionPopupButton.AddItem ("Current");
			
			groupView3.AddSubview(versionPopupButton);
			versionPopupButton.SetFrameOrigin(new CoreGraphics.CGPoint(76f, 17f));
			var textView3 = new AppKit.NSTextField() {    StringValue = "Version:",
			Editable = false,
			Bordered = false,
			Bezeled = false,
			DrawsBackground = false,
			Alignment = NSTextAlignment.Left,
			};
			textView3.WantsLayer = true;
			textView3.SetFrameSize(new CoreGraphics.CGSize(71f, 35f));
			textView3.Font = NSFontManager.SharedFontManager.FontWithFamily("SF Pro Text", default(NSFontTraitMask), 5, 13);
			textView3.Alignment = NSTextAlignment.Right;
			textView3.AlphaValue = 1f;
			textView3.TextColor = NSColor.FromRgba(0f, 0f, 0f, 1f);
			
			groupView3.AddSubview(textView3);
			textView3.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 0f));
			var view30 = new AppKit.NSTextField();
			view30.StringValue = "https://www.figma.com/file/";
			view30.WantsLayer = true;
			view30.SetFrameSize(new CoreGraphics.CGSize(221f, 21f));
			
			groupView3.AddSubview(view30);
			view30.SetFrameOrigin(new CoreGraphics.CGPoint(76f, 48f));
			var textView4 = new AppKit.NSTextField() {    StringValue = "Figma URL:",
			Editable = false,
			Bordered = false,
			Bezeled = false,
			DrawsBackground = false,
			Alignment = NSTextAlignment.Left,
			};
			textView4.WantsLayer = true;
			textView4.SetFrameSize(new CoreGraphics.CGSize(71f, 35f));
			textView4.Font = NSFontManager.SharedFontManager.FontWithFamily("SF Pro Text", default(NSFontTraitMask), 5, 13);
			textView4.Alignment = NSTextAlignment.Right;
			textView4.AlphaValue = 1f;
			textView4.TextColor = NSColor.FromRgba(0f, 0f, 0f, 1f);
			
			groupView3.AddSubview(textView4);
			textView4.SetFrameOrigin(new CoreGraphics.CGPoint(0f, 31f));
			var textView5 = new AppKit.NSTextField() {    StringValue = "Source",
			Editable = false,
			Bordered = false,
			Bezeled = false,
			DrawsBackground = false,
			Alignment = NSTextAlignment.Left,
			};
			textView5.WantsLayer = true;
			textView5.SetFrameSize(new CoreGraphics.CGSize(65f, 19f));
			textView5.Font = NSFontManager.SharedFontManager.FontWithFamily("SF Pro Text", default(NSFontTraitMask), 6, 13);
			textView5.Alignment = NSTextAlignment.Left;
			textView5.AlphaValue = 1f;
			textView5.TextColor = NSColor.FromRgba(0f, 0f, 0f, 1f);
			
			frameEntityView2.AddSubview(textView5);
			textView5.SetFrameOrigin(new CoreGraphics.CGPoint(20f, 279f));


			
		}
	}
}
