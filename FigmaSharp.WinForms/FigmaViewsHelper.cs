using System;
using System.Drawing;
using System.Reflection;

namespace FigmaSharp.WinForms
{
    public static class FigmaViewsHelper
    {
        public static Image GetManifestImageResource(Assembly assembly, string resource)
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
                    var imageData = Image.FromStream(stream);
                    return imageData;
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

        //public static UITextField CreateLabel(string text, UIFont font = null, UITextAlignment alignment = UITextAlignment.Left)
        //{
        //    var label = new UITextField()
        //    {
        //        Text = text ?? "",
        //        Font = font ?? GetSystemFont(false),
        //    };
        //    label.TranslatesAutoresizingMaskIntoConstraints = false;
        //    return label;
        //}

        //public static UIFont GetSystemFont(bool bold, float size = 0.0f)
        //{
        //    if (size <= 0)
        //    {
        //        size = (float)UIFont.SystemFontSize;
        //    }
        //    if (bold)
        //        return UIFont.BoldSystemFontOfSize(size);
        //    return UIFont.SystemFontOfSize(size);
        //}
    }
}