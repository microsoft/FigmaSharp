/* 
 * FigmaViewsHelper.cs - Helper methods for NSViews
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using AppKit;
using System.Linq;
using CoreGraphics;
using System.IO;
using System.Reflection;

namespace FigmaSharp
{
    public static class FigmaViewsHelper
    {
        public static NSImage GetManifestImageResource(Assembly assembly, string resource)
        {
            if (assembly == null)
            {
                //TODO: not safe
                assembly = Assembly.GetEntryAssembly();
            }
            try
            {
                //TODO: not safe
                var fullResourceName = string.Concat(assembly.GetName ().Name, ".Resources.", resource);
                //var resources = assembly.GetManifestResourceNames();
                using (var stream = assembly.GetManifestResourceStream(fullResourceName))
                {
                    return NSImage.FromStream(stream);
                }
            }
            catch (System.ArgumentNullException)
            {
                Console.WriteLine("[ERROR] File '{0}' not found in Resources and/or not set Build action to EmbeddedResource", resource);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);

            }
            return null;
        }

        public static NSTextField CreateLabel(string text, NSFont font = null, NSTextAlignment alignment = NSTextAlignment.Left)
        {
            var label = new NSTextField()
            {
                StringValue = text ?? "",
                Font = font ?? GetSystemFont(false),
                Editable = false,
                Bordered = false,
                Bezeled = false,
                DrawsBackground = false,
                Selectable = false,
                Alignment = alignment
            };
            label.TranslatesAutoresizingMaskIntoConstraints = false;
            return label;
        }

        public static NSFont GetSystemFont(bool bold, float size = 0.0f)
        {
            if (size <= 0)
            {
                size = (float)NSFont.SystemFontSize;
            }
            if (bold)
                return NSFont.BoldSystemFontOfSize(size);
            return NSFont.SystemFontOfSize(size);
        }
    }
}
