using System;
using System.IO;
using AppKit;
using FigmaSharp;
using FigmaSharp.Designer;
using Foundation;
using System.Linq;
using CoreGraphics;
using FigmaSharp.Services;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using System.Reflection;

namespace StandaloneDesigner
{
    public partial class ViewController : NSViewController
    {
        FigmaDesignerSurface designerSurface;
        FigmaDesignerSession session;
        IFigmaDesignerDelegate designerDelegate;

        ScrollViewWrapper scrollViewWrapper;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        FigmaViewRendererDistributionService distributionService;
        FigmaManifestFileProvider fileProvider;
        FigmaFileRendererService rendererService;
        OutlinePanel outlinePanel;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            scrollViewWrapper = new ScrollViewWrapper(scrollview);
            outlinePanel = new OutlinePanel();

            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();

            //we load all the services
            fileProvider = new FigmaManifestFileProvider(this.GetType ().Assembly);
            rendererService = new FigmaFileRendererService(fileProvider, converters);
            distributionService = new FigmaViewRendererDistributionService(rendererService);

            designerDelegate = new FigmaDesignerDelegate();

            //figma session handles
            session = new FigmaDesignerSession(fileProvider, rendererService, distributionService);
            designerSurface = new FigmaDesignerSurface(designerDelegate, session);

            // we set to our surface current window to listen changes on it
            var window = NSApplication.SharedApplication.DangerousWindows
                .FirstOrDefault (s => s is WindowWrapper) as WindowWrapper;
            designerSurface.SetWindow(window);

            //time to reload a figma document in the current session
            session.Reload(scrollViewWrapper, "FigmaStoryboard.figma", new FigmaViewRendererServiceOptions());

            //
            designerSurface.StartHoverSelection();
           
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
                view.SetPosition(currentX, 0);
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
