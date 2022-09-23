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
using System.Linq;
using System.Reflection;

using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace FigmaSharp.Maui
{
    public static class ViewsHelper
    {
        static string GetResourceDefaultName (Assembly assembly, string resourceName)
        {
            //TODO: not safe
            var fullResourceName = string.Concat(assembly.GetName().Name, ".Resources.", resourceName);
            return fullResourceName;
        }

        static bool IsResourceInAssembly(Assembly assembly, string fullResourceName) =>
            assembly.GetManifestResourceNames().Any(s => s == fullResourceName);

        static (string fullResourceName, Assembly assembly) GetAssemblyForResource (string resourceName, params Assembly[] assemblies)
        {
            string fullResourceName;
            foreach (var assembly in assemblies)
            {
                fullResourceName = GetResourceDefaultName(assembly, resourceName);
                if (IsResourceInAssembly(assembly, fullResourceName))
                {
                    return (fullResourceName, assembly);
                }
            }
            return (null, null);
        }


        public static ImageSource GetManifestImageResource(Assembly assembly, string resource)
        {
            try
            {
                //used for shared libraries
                var entryAssembly = GetAssemblyForResource(resource, assembly, Assembly.GetEntryAssembly());
                if (entryAssembly.assembly == null)
                {
                    throw new NullReferenceException($"resource name '{resource}' not found in the assembly resources");
                }

                //var resources = assembly.GetManifestResourceNames();
                using (var stream = entryAssembly.assembly.GetManifestResourceStream(entryAssembly.fullResourceName))
                {
                    var imageSource = ImageSource.FromStream (() => stream);
                    return imageSource;
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

        public static Label CreateLabel(string text, Font font, TextAlignment alignment = TextAlignment.Start)
        {
            var label = new Microsoft.Maui.Controls.Label()
            {
                Text = text ?? "", FontFamily = font.Family,
                HorizontalTextAlignment = alignment
            };
            return label;
        }
    }
}