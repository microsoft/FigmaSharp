using System;
using AppKit;
using FigmaSharp;

namespace ToCode.Cocoa
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            FigmaApplication.Init(Environment.GetEnvironmentVariable("TOKEN"));
            NSApplication.Init();
            NSApplication.Main(args);
        }
    }
}
