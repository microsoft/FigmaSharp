using System;
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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.

            var fileName = Environment.GetEnvironmentVariable("FILE");

            var service = new FigmaRemoteFileService();
            var buttonConverter = new CustomButtonConverter();
            service.CustomViewConverters.Add(buttonConverter);
            service.Start(fileName);

            var scrollViewWrapper = new ScrollViewWrapper(MainScrollView);

            var builderService = new ScrollViewRendererService();
            builderService.Start(scrollViewWrapper, service.NodesProcessed);
            MainScrollView.ContentSize = new CoreGraphics.CGSize(1000, 300);


            //View = service.ContentView as UIView;
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
