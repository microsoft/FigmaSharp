using System;

using AppKit;
using FigmaSharp.Designer;
using Foundation;

namespace StandaloneDesigner
{
    public partial class ViewController : NSViewController
    {
        FigmaDesignerSurface surface;
        FigmaDesignerSession session;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
            session = new FigmaDesignerSession();
            surface = new FigmaDesignerSurface
            {
                Session = session
            };
           // session.Reload ()
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
