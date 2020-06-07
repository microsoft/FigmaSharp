using System;
using System.Reflection;
using Xamarin.Forms;
using System.Linq;

namespace FigmaSharp.Forms
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
            var label = new Label()
            {
                Text = text ?? "",
                Font = font, HorizontalTextAlignment = alignment
            };
            return label;
        }
    }
}