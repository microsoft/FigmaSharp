using UIKit;
using FigmaSharp;
using System;

namespace ExampleFigmaIOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            FigmaApplication.Init(Environment.GetEnvironmentVariable("TOKEN"));
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
