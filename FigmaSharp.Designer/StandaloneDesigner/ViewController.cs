using System;
using System.IO;
using AppKit;
using FigmaSharp;
using FigmaSharp.Designer;
using Foundation;
using System.Linq;

namespace StandaloneDesigner
{
    public partial class ViewController : NSViewController
    {
        FigmaDesignerSurface surface;
        FigmaDesignerSession session;
        IDesignerDelegate figmaDelegate;

        ScrollViewWrapper scrollViewWrapper;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            scrollViewWrapper = new ScrollViewWrapper(scrollview);

            figmaDelegate = new DesignerDelegate();

            // Do any additional setup after loading the view.
            session = new FigmaDesignerSession();
            surface = new FigmaDesignerSurface (figmaDelegate)
            {
                Session = session
            };

            session.ReloadFinished += Session_ReloadFinished;
         
            var directory = Environment.GetEnvironmentVariable("DIRECTORY");

            var file = Path.Combine (directory, Environment.GetEnvironmentVariable("FILE"));
            session.Reload(file, directory);

            var window = NSApplication.SharedApplication.Windows.FirstOrDefault();

            surface.SetWindow(window as WindowWrapper);

            surface.StartHoverSelection();
        }

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
