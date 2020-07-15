using System;
using System.Text;
using AppKit;
using FigmaSharp;
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Designer;
using FigmaSharp.Models;
using FigmaSharp.Services;
using Foundation;

namespace ToCode.Cocoa
{
	public class MonoDevelopTranslationService : ITranslationService
	{
		public string GetTranslatedStringText (string text)
		{
			return string.Format("Core.GettextCatalog.GetString (\"{0}\")", text);
		}
	}

	public partial class ViewController : NSViewController
	{
		FigmaNodeView data;
		OutlinePanel outlinePanel;
		RemoteNodeProvider fileProvider;

		FigmaDesignerDelegate figmaDelegate;
		CodeRenderService codeRenderer;

		MonoDevelopTranslationService translationService = new MonoDevelopTranslationService();

		const string fileIds = "Rg3acHLy7Y0pkBiSWgu0ps";

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

            urlTextField.Activated += UrlTextField_Activated;

			treeHierarchyContainer.AddSubview (scrollView);

			treeHierarchyContainer.TranslatesAutoresizingMaskIntoConstraints = false;
			scrollView.TranslatesAutoresizingMaskIntoConstraints = false;

			scrollView.TopAnchor.ConstraintEqualToAnchor (treeHierarchyContainer.TopAnchor).Active = true;
			scrollView.BottomAnchor.ConstraintEqualToAnchor (treeHierarchyContainer.BottomAnchor).Active = true;
			scrollView.LeftAnchor.ConstraintEqualToAnchor (treeHierarchyContainer.LeftAnchor).Active = true;
			scrollView.RightAnchor.ConstraintEqualToAnchor (treeHierarchyContainer.RightAnchor).Active = true;

			figmaDelegate = new FigmaDesignerDelegate ();

			urlTextField.StringValue = fileIds;

			RefreshTree(fileIds);
		}

		void RefreshTree(string docId)
		{
			var converters = FigmaControlsContext.Current.GetConverters();
			fileProvider = new ControlRemoteNodeProvider();
			fileProvider.Load(docId);

			var codePropertyConverter = FigmaControlsContext.Current.GetCodePropertyConverter();
			codeRenderer = new NativeViewCodeService(fileProvider, converters, codePropertyConverter, translationService);

			data = new FigmaNodeView(fileProvider.Response.document);
			figmaDelegate.ConvertToNodes(fileProvider.Response.document, data);
			outlinePanel.GenerateTree(data);

			((NSTextView)logTextField.DocumentView).Value = string.Empty;
		}

        private void UrlTextField_Activated(object sender, EventArgs e)
        {
			RefreshTree(urlTextField.StringValue);
		}

        private void OpenUrlButton_Activated (object sender, EventArgs e)
		{
			var url = new NSUrl (string.Format ("https://www.figma.com/file/{0}", urlTextField.StringValue));
			NSWorkspace.SharedWorkspace.OpenUrl (url);
		}

		private void CopyDesignerCSButton_Activated (object sender, EventArgs e)
		{
			if (currentSelectedNode == null)
				return;

			var bundle = FigmaBundle.Empty ("1234", null, string.Empty);
			var className = currentSelectedNode.GetClassName ();
			var figmaBundleView = FigmaControlsContext.Current.GetBundleView (bundle, className, currentSelectedNode);
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
			var figmaBundleView = FigmaControlsContext.Current.GetBundleView (bundle, className, currentSelectedNode);
			var publicPartialClass = figmaBundleView.GetFigmaPartialDesignerClass (codeRenderer, translateStrings: translateButton.State == NSCellStateValue.On);
			var code = publicPartialClass.Generate ();
			CopyToLogView (code);
		}

		FigmaNode currentSelectedNode = null;

		void OutlinePanel_RaiseFirstResponder (object sender, FigmaNode e)
		{
			codeRenderer.Clear();
			currentSelectedNode = e;
			var builder = new StringBuilder ();
			var options = new CodeRenderServiceOptions() { TranslateLabels = openUrlButton.State == NSCellStateValue.On };
			codeRenderer.GetCode (builder, new CodeNode (e, null), null, options);
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
