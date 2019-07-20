using FigmaSharp.iOS;
using Foundation;
using LocalFile.Shared;
using System;
using UIKit;

namespace LocalFile.IOS
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        DocumentExample documentExample;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.


            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();
            var storyboard = new FigmaStoryboard(converters);

            var scrollViewWrapper = new ScrollViewWrapper(ScrollView);
            documentExample = new DocumentExample(scrollViewWrapper, storyboard);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}