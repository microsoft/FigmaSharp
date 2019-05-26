using System;

using AppKit;
using FigmaSharp;
using FigmaSharp.Designer;
using FigmaSharp.Services;
using Foundation;
using MonoDevelop.Figma;
using System.Linq;
using FigmaSharp.NativeControls.Cocoa;

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
            fileProvider.Load("Dq1CFm7IrDi3UJC7KJ8zVjOt");
            codeRenderer = new FigmaCodeRendererService(fileProvider, converters);
           
            data = new FigmaNodeView(fileProvider.Response.document);
            figmaDelegate.ConvertToNodes(fileProvider.Response.document, data);
            outlinePanel.GenerateTree(data);
        }

        void OutlinePanel_RaiseFirstResponder(object sender, FigmaNode e)
        {
            var code = codeRenderer.GetCode(e, true);
            var textField = logTextField.DocumentView as NSTextView; // codeRenderer.GetCode()
            textField.Value = code;
            NSPasteboard.GeneralPasteboard.DeclareTypes(new string[] { NSPasteboard.NSStringType }, null);
            NSPasteboard.GeneralPasteboard.SetStringForType(code, NSPasteboard.NSStringType);
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
