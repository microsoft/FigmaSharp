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

namespace ToCode.Cocoa
{
    public partial class ViewController : NSViewController
    {
        FigmaNodeView data;
        OutlinePanel outlinePanel;
        FigmaRemoteFileProvider fileProvider;

        FigmaDesignerDelegate figmaDelegate;
        FigmaCodeRendererService codeRenderer;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            outlinePanel = new OutlinePanel();

            var scrollView = outlinePanel.EnclosingScrollView;

            outlinePanel.RaiseFirstResponder += OutlinePanel_RaiseFirstResponder;

            treeHierarchyContainer.AddSubview(scrollView);

            treeHierarchyContainer.TranslatesAutoresizingMaskIntoConstraints = false;
            scrollView.TranslatesAutoresizingMaskIntoConstraints = false;

            scrollView.TopAnchor.ConstraintEqualToAnchor(treeHierarchyContainer.TopAnchor).Active = true;
            scrollView.BottomAnchor.ConstraintEqualToAnchor(treeHierarchyContainer.BottomAnchor).Active = true;
            scrollView.LeftAnchor.ConstraintEqualToAnchor(treeHierarchyContainer.LeftAnchor).Active = true;
            scrollView.RightAnchor.ConstraintEqualToAnchor(treeHierarchyContainer.RightAnchor).Active = true;

            figmaDelegate = new FigmaDesignerDelegate();

            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters()
                .Union (Resources.GetConverters())
                .ToArray ();

            fileProvider = new FigmaRemoteFileProvider();
            fileProvider.Load("CobaSo7LmEYsuGZB0ED0ewSs");

            var addChildConverter = new FigmaCodeAddChildConverter();
            var positionConverter = new FigmaCodePositionConverter();
            codeRenderer = new FigmaCodeRendererService(fileProvider, converters, positionConverter, addChildConverter);
           
            data = new FigmaNodeView(fileProvider.Response.document);
            figmaDelegate.ConvertToNodes(fileProvider.Response.document, data);
            outlinePanel.GenerateTree(data);
        }

        void OutlinePanel_RaiseFirstResponder(object sender, FigmaNode e)
        {
            var builder = new StringBuilder();
            codeRenderer.GetCode(builder, e, null, null);
            var textField = logTextField.DocumentView as NSTextView; // codeRenderer.GetCode()
            textField.Value = builder.ToString();
            NSFont monospaceFont = NSFont.FromFontName("Monaco", 11);
            textField.SetFont(monospaceFont, new NSRange(0, textField.Value.Length));

            NSPasteboard.GeneralPasteboard.DeclareTypes(new string[] { NSPasteboard.NSStringType }, null);
            NSPasteboard.GeneralPasteboard.SetStringForType(textField.Value, NSPasteboard.NSStringType);
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
