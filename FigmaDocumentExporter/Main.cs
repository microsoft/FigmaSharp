using System;
using AppKit;
using FigmaSharp;

namespace FigmaDocumentExporter
{
	static class MainClass
	{
		static void Main (string[] args)
		{
			var token = Environment.GetEnvironmentVariable ("TOKEN");
			FigmaEnvirontment.SetAccessToken (token);
			NSApplication.Init ();
			NSApplication.Main (args);
		}
	}
}
