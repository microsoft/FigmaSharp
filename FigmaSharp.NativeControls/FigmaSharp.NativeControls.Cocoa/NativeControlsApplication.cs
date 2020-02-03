using System;
using FigmaSharp;
using FigmaSharp.Cocoa;
using FigmaSharp.NativeControls;

namespace FigmaSharp.NativeControls.Cocoa
{
	public static class NativeControlsApplication
	{
		public static void Init (string token)
		{
			//Figma initialization
			FigmaApplication.Init (token);

			var applicationDelegate = new FigmaNativeControlsDelegate ();
			NativeControlsContext.Current.Configuration (applicationDelegate);
		}

		public static void Init ()
		{
			FigmaApplication.Init ();
			var applicationDelegate = new FigmaNativeControlsDelegate ();
			NativeControlsContext.Current.Configuration (applicationDelegate);
		}
	}
}
