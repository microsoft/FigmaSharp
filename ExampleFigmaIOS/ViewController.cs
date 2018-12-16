using System;

using UIKit;
using FigmaSharp.Services;

namespace ExampleFigmaIOS
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

            var fileName = Environment.GetEnvironmentVariable("FILE");

            var service = new FigmaRemoteFileService();
            var buttonConverter = new CustomButtonConverter();
            service.CustomConverters.Add(buttonConverter);
            service.Start(fileName);

            View = service.ContentView as UIView;
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
