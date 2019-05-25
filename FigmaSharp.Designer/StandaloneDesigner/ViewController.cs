using System;
using System.IO;
using AppKit;
using FigmaSharp;
using FigmaSharp.Designer;
using Foundation;
using System.Linq;
using MonoDevelop.Figma;
using CoreGraphics;
using FigmaSharp.Services;
using FigmaSharp.Cocoa;

namespace StandaloneDesigner
{
    public partial class ViewController : NSViewController
    {
        FigmaDesignerSurface surface;
        FigmaDesignerSession session;
        IFigmaDesignerDelegate figmaDelegate;

        ScrollViewWrapper scrollViewWrapper;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        FigmaRemoteFileService fileService;
        OutlinePanel outlinePanel;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            scrollViewWrapper = new ScrollViewWrapper(scrollview);
            outlinePanel = new OutlinePanel();

            figmaDelegate = new FigmaDesignerDelegate();

            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();

            fileService = new FigmaRemoteFileService(converters);
            surface = new FigmaDesignerSurface(figmaDelegate);
            // Do any additional setup after loading the view.

            var directory = Environment.GetEnvironmentVariable("DIRECTORY");
            var file = Path.Combine (directory, Environment.GetEnvironmentVariable("FILE"));
            session = new FigmaDesignerSession(converters);
            session.Reload(file, directory);

            var window = NSApplication.SharedApplication.Windows.FirstOrDefault();

            surface.SetWindow(window as WindowWrapper);

            surface.StartHoverSelection();
           
            var second = new NSWindow(new CGRect(0, 0, 300, 600), NSWindowStyle.Titled | NSWindowStyle.Resizable | NSWindowStyle.Closable, NSBackingStore.Buffered, false);
            window.AddChildWindow(second, NSWindowOrderingMode.Above);

            //propertyPanel = new FigmaPropertyPanel();
            //second.ContentView = propertyPanel.View;
            //propertyPanel.Initialize();

            //surface.FocusedViewChanged += (sender, e) =>
            //{
            //    var model = session.GetModel(e);
            //    propertyPanel.Select(model);
            //};

            //propertyPanel.Select(session.MainViews[0].FigmaNode);
        }

        //FigmaPropertyPanel propertyPanel;

        void Session_ReloadFinished(object sender, EventArgs e)
        {
            foreach (var items in session.MainViews)
            {
                scrollViewWrapper.AddChild(items.View);
            }

            var mainNodes = session.ProcessedNodes
               .Where(s => s.ParentView == null)
               .ToArray();

            Reposition(mainNodes);

            //we need reload after set the content to ensure the scrollview
            scrollViewWrapper.AdjustToContent();
        }
        
        public void Reposition(ProcessedNode[] mainNodes)
        {
            //Alignment 
            const int Margin = 20;
            float currentX = Margin;
            foreach (var processedNode in mainNodes)
            {
                var view = processedNode.View;
                scrollViewWrapper.AddChild(view);

                view.X = currentX;
                view.Y = 0; //currentView.Height + currentHeight;
                currentX += view.Width + Margin;
            }
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
