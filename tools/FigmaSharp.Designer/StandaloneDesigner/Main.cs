using System;
using AppKit;
using FigmaSharp;
using FigmaSharp.Cocoa;

namespace StandaloneDesigner
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
