using System;
using AppKit;

namespace BasicControls
{
    static class MainClass
    {
        static MainSample example;
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Regular;

            example = new MainSample();

            //mainWindow.Closing += delegate { NSRunningApplication.CurrentApplication.Terminate(); };
            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            NSApplication.SharedApplication.Run();
        }
    }
}
