using System;

using AppKit;
using Foundation;

namespace FigmaSharp.Samples
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        public AppDelegate()
        {
        }

        public override bool OpenFile(NSApplication sender, string filename)
        {
            return true;
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}
