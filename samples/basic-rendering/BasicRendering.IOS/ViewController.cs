using ExampleFigma;
using FigmaSharp.iOS;
using Foundation;
using System;
using UIKit;

namespace BasicRendering.IOS
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        ExampleViewManager manager;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            View.Layer.BackgroundColor = UIColor.Black.CGColor;

            var scrollViewWrapper = new ScrollViewWrapper(MainScrollView);
            var contentView = new ViewWrapper(new UIView());

            manager = new ExampleViewManager(scrollViewWrapper, contentView);
            manager.Initialize();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}