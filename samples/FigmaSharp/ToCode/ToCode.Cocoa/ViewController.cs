// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Text;

using AppKit;
using Foundation;

using FigmaSharp;
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Designer;
using FigmaSharp.Models;
using FigmaSharp.Services;

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

		//const string fileIds = "Rg3acHLy7Y0pkBiSWgu0ps";

		public ViewController (ObjCRuntime.NativeHandle handle) : base (handle)
		{
		}

		public override async void ViewDidLoad ()
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

			scrollView.TopAnchor.ConstraintEqualTo (treeHierarchyContainer.TopAnchor).Active = true;
			scrollView.BottomAnchor.ConstraintEqualTo(treeHierarchyContainer.BottomAnchor).Active = true;
			scrollView.LeadingAnchor.ConstraintEqualTo(treeHierarchyContainer.LeadingAnchor).Active = true;
			scrollView.TrailingAnchor.ConstraintEqualTo(treeHierarchyContainer.TrailingAnchor).Active = true;

			translateButton.State = NSCellStateValue.Off;

			figmaDelegate = new FigmaDesignerDelegate ();

			if (File.Exists (FilePath))
            {
				urlTextField.StringValue = File.ReadAllText(FilePath) ?? string.Empty;
				await RefreshTreeAsync(urlTextField.StringValue);
            }
		}

        public string FilePath => Path.Combine(Path.GetDirectoryName(GetType ().Assembly.Location), "config.cfg");

		const string UrlRegistryKey = "keyUrl";

		async Task RefreshTreeAsync(string docId)
		{
			var converters = FigmaControlsContext.Current.GetConverters();
			fileProvider = new ControlRemoteNodeProvider();
			await fileProvider.LoadAsync(docId);

			if (fileProvider.Response == null)
            {
				return;
            }

				var codePropertyConverter = FigmaControlsContext.Current.GetCodePropertyConverter();
			codeRenderer = new NativeViewCodeService(fileProvider, converters, codePropertyConverter, translationService);

			data = new FigmaNodeView(fileProvider.Response.document);
			figmaDelegate.ConvertToNodes(fileProvider.Response.document, data);
			outlinePanel.GenerateTree(data);

			((NSTextView)logTextField.DocumentView).Value = string.Empty;
		}

        private	async void UrlTextField_Activated(object sender, EventArgs e)
        {
			if (string.IsNullOrEmpty(urlTextField.StringValue))
				return;
			System.IO.File.WriteAllText(FilePath, urlTextField.StringValue);
			await RefreshTreeAsync(urlTextField.StringValue);
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
