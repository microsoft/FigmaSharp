using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FigmaSharp.Views.Wpf
{
    public static class ViewsHelper
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
                    var imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.StreamSource = stream;
                    imageSource.EndInit();

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
    }
}
