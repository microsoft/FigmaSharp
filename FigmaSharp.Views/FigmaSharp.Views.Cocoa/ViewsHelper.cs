// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Reflection;

using AppKit;
using FigmaSharp.Services;
using FigmaSharp.Views.Native.Cocoa;

namespace FigmaSharp.Views.Cocoa
{
    public static class ViewsHelper
	{
		public static int ToAppKitFontWeight (float font_weight)
		{
			float weight = font_weight;
			if (weight <= 50 || weight >= 950)
				return 5;

			var select_weight = (int)Math.Round (weight / 100) - 1;
			return app_kit_font_weights[select_weight];
		}

		static int[] app_kit_font_weights = {
			2,   // FontWeight100
      3,   // FontWeight200
      4,   // FontWeight300
      5,   // FontWeight400
      6,   // FontWeight500
      8,   // FontWeight600
      9,   // FontWeight700
      10,  // FontWeight800
      12,  // FontWeight900
            };

		public static NSFont GetSystemFont (bool bold, float size = 0.0f)
		{
			if (size <= 0) {
				size = (float)NSFont.SystemFontSize;
			}
			if (bold)
				return NSFont.BoldSystemFontOfSize (size);
			return NSFont.SystemFontOfSize (size);
		}

		public static NSImage GetManifestImageResource (Assembly assembly, string resource)
		{
			if (assembly == null) {
				//TODO: not safe
				assembly = Assembly.GetEntryAssembly ();
			}
			try {
				//TODO: not safe
				var fullResourceName = string.Concat (assembly.GetName ().Name, ".Resources.", resource);
				//var resources = assembly.GetManifestResourceNames();
				using (var stream = assembly.GetManifestResourceStream (fullResourceName)) {
					return NSImage.FromStream (stream);
				}
			} catch (System.ArgumentNullException) {
				LoggingService.LogError(string.Format("[FIGMA]  File '{0}' not found in Resources and/or not set Build action to EmbeddedResource", resource));
			} catch (System.Exception ex) {
				LoggingService.LogError("[FIGMA] Error.", ex);
			}
			return null;
		}

		public static FNSTextField CreateLabel (string text, NSFont font = null, NSTextAlignment alignment = NSTextAlignment.Left)
		{
			var label = new FNSTextField ();
			label.Cell = new VerticalAlignmentTextCell ();
			label.StringValue = text ?? "";
			label.Font = font ?? GetSystemFont (false);
			label.Editable = false;
			label.Bordered = false;
			label.Bezeled = false;
			label.DrawsBackground = false;
			label.Selectable = false;
			label.Alignment = alignment;
			return label;
		}
	}
}
