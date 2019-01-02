using System;
using CoreAnimation;
using CoreGraphics;
using ExampleFigmaMac;
using FigmaSharp;
using FigmaSharp.Services;
using UIKit;

namespace ExampleFigma.IOS
{
    public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        ExampleViewManager manager;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            View.Layer.BackgroundColor = UIColor.Black.CGColor;

            var fileName = "Dq1CFm7IrDi3UJC7KJ8zVjOt";
            var scrollViewWrapper = new ScrollViewWrapper(MainScrollView);

            manager = new ExampleViewManager(scrollViewWrapper, fileName);
            manager.Initialize();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
