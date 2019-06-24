using System;
using CoreAnimation;
using CoreGraphics;
using FigmaSharp;
using FigmaSharp.iOS;
using FigmaSharp.NativeControls.iOS;
using FigmaSharp.Services;
using UIKit;
using System.Linq;
using LocalFile.Shared;

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

            var scrollViewWrapper = new ScrollViewWrapper(MainScrollView);

            var figmaConverters = FigmaSharp.AppContext.Current.GetFigmaConverters().Union (Resources.GetConverters()).ToArray ();

            manager = new ExampleViewManager(scrollViewWrapper, figmaConverters);
            manager.Initialize();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
