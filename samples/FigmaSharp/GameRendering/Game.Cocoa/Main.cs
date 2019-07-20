using System;
using AppKit;
using CoreGraphics;
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using System.Linq;

namespace Game.Cocoa
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            FigmaApplication.Init(Environment.GetEnvironmentVariable("TOKEN"));

            NSApplication.Init();
            NSApplication.SharedApplication.ActivationPolicy = NSApplicationActivationPolicy.Regular;

            var mainWindow = new GameWindow(new FigmaSharp.Rectangle(0, 0, 720, 450));

            mainWindow.Closing += delegate { NSRunningApplication.CurrentApplication.Terminate(); };
			//mainWindow.Center();

			mainWindow.Show();

            NSApplication.SharedApplication.ActivateIgnoringOtherApps(true);
            NSApplication.SharedApplication.Run();
        }
    }
}
