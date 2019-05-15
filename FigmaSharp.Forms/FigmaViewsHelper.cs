using System;
using System.Reflection;
using Xamarin.Forms;

namespace FigmaSharp
{
    public static class FigmaViewsHelper
    {
        public static ImageSource GetManifestImageResource(Assembly assembly, string resource)
        {
            if (assembly == null)
            {
                //TODO: not safe
                assembly = Assembly.GetEntryAssembly();
            }
            try
            {
                //TODO: not safe
                var fullResourceName = string.Concat(assembly.GetName().Name, ".Resources.", resource);
                //var resources = assembly.GetManifestResourceNames();
                using (var stream = assembly.GetManifestResourceStream(fullResourceName))
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