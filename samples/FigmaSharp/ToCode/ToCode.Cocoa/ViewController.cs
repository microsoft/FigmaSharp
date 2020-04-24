using System;

using AppKit;
using FigmaSharp;
using FigmaSharp.Services;
using Foundation;
using System.Linq;
using FigmaSharp.NativeControls.Cocoa;
using FigmaSharp.Cocoa.Converters;
using System.Text;
using FigmaSharp.Designer;
using FigmaSharp.Models;
using FigmaSharp.NativeControls.Cocoa.Converters;

namespace ToCode.Cocoa
{
	public partial class ViewController : NSViewController
	{
		FigmaNodeView data;
		OutlinePanel outlinePanel;
		FigmaRemoteFileProvider fileProvider;

		FigmaDesignerDelegate figmaDelegate;
		FigmaCodeRendererService codeRenderer;

		const string fileId = "7E9P9fEYGJijRXigfSz0kP";

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			copyCSButton.Activated += CopyCSButton_Activated;
			copyDesignerCSButton.Activated += CopyDesignerCSButton_Activated;
			openUrlButton.Activated += OpenUrlButton_Activated;

			// Do any additional setup after loading the view.
			outlinePanel = new OutlinePanel ();

			var scrollView = outlinePanel.EnclosingScrollView;

			outlinePanel.RaiseFirstResponder += OutlinePanel_RaiseFirstResponder;

			treeHierarchyContainer.AddSubview (scrollView);

			treeHierarchyContainer.TranslatesAutoresizingMaskIntoConstraints = false;
			scrollView.TranslatesAutoresizingMaskIntoConstraints = false;

			scrollView.TopAnchor.ConstraintEqualToAnchor (treeHierarchyContainer.TopAnchor).Active = true;
			scrollView.BottomAnchor.ConstraintEqualToAnchor (treeHierarchyContainer.BottomAnchor).Active = true;
			scrollView.LeftAnchor.ConstraintEqualToAnchor (treeHierarchyContainer.LeftAnchor).Active = true;
			scrollView.RightAnchor.ConstraintEqualToAnchor (treeHierarchyContainer.RightAnchor).Active = true;

			figmaDelegate = new FigmaDesignerDelegate ();

			var converters = NativeControlsContext.Current.GetConverters ();
			fileProvider = new FigmaRemoteFileProvider ();
			fileProvider.Load (fileId);

			var codePropertyConverter = NativeControlsContext.Current.GetCodePropertyConverter ();
			codeRenderer = new NativeViewCodeService (fileProvider, converters, codePropertyConverter);

			data = new FigmaNodeView (fileProvider.Response.document);
			figmaDelegate.ConvertToNodes (fileProvider.Response.document, data);
			outlinePanel.GenerateTree (data);
		}

		private void OpenUrlButton_Activated (object sender, EventArgs e)
		{
			var url = new NSUrl (string.Format ("https://www.figma.com/file/{0}", fileId));
			NSWorkspace.SharedWorkspace.OpenUrl (url);
		}

		private void CopyDesignerCSButton_Activated (object sender, EventArgs e)
		{
			if (currentSelectedNode == null)
				return;

			var bundle = FigmaBundle.Empty ("1234", null, string.Empty);
			var className = currentSelectedNode.GetClassName ();
			var figmaBundleView = NativeControlsContext.Current.GetBundleView (bundle, className, currentSelectedNode);
			var publicPartialClass = figmaBundleView.GetPublicPartialClass ();
			var code = publicPartialClass.Generate ();
			CopyToLogView (code);
		}

		private void CopyCSButton_Activated (object sender, EventArgs e)
		{
			if (currentSelectedNode == null)
				return;
			var className = currentSelectedNode.GetClassName ();
			var bundle = FigmaBundle.Empty ("1234", null, string.Empty);
			var figmaBundleView = NativeControlsContext.Current.GetBundleView (bundle, className, currentSelectedNode);
			var publicPartialClass = figmaBundleView.GetFigmaPartialDesignerClass (codeRenderer, translateStrings: openUrlButton.State == NSCellStateValue.On);
			var code = publicPartialClass.Generate ();
			CopyToLogView (code);
		}

		FigmaNode currentSelectedNode = null;

		void OutlinePanel_RaiseFirstResponder (object sender, FigmaNode e)
		{
			codeRenderer.Clear();
			currentSelectedNode = e;
			var builder = new StringBuilder ();
			var options = new FigmaCodeRendererServiceOptions() { TranslateLabels = openUrlButton.State == NSCellStateValue.On };
			codeRenderer.GetCode (builder, new FigmaCodeNode (e, null), null, options);
			CopyToLogView (builder.ToString ());
		}

		void CopyToLogView (string text)
		{
			text = text ?? string.Empty;
			var textField = (NSTextView)logTextField.DocumentView;
			textField.Value = text;
			textField.SetFont (NSFont.FromFontName ("Monaco", 11), new NSRange (0, text.Length));
			CopyToClipBoard (text);
		}

		void CopyToClipBoard (string text)
		{
			NSPasteboard.GeneralPasteboard.DeclareTypes (new string[] { NSPasteboard.NSStringType }, null);
			NSPasteboard.GeneralPasteboard.SetStringForType (text, NSPasteboard.NSStringType);
		}

		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
	}
}
