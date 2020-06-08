using System;
using System.IO;
using System.Reflection;

namespace FigmaSharp.Views.Helpers
{
	public static class FileHelper
	{
		public static string GetFileDataFromBundle (string fileName, Assembly assembly = null)
		{
			if (assembly == null)
				assembly = Assembly.GetEntryAssembly();

			var path = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(assembly.Location));
			path = System.IO.Path.Combine(path, "Resources", "svg", fileName);
			var data = System.IO.File.ReadAllText(path);
			return data;
		}

        public static string GetManifestResource(Assembly assembly, string resource)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetEntryAssembly();
            }
            try
            {
                var resources = assembly.GetManifestResourceNames();
                var fullResourceName = string.Format("{0}.{1}", assembly.GetName().Name, resource);

                foreach (var item in new string[] { resource,  fullResourceName })
                {
                    //TODO: not safe
                    using (var stream = assembly.GetManifestResourceStream(item))
                    {
                        if (stream == null)
                            continue;
                        using (TextReader tr = new StreamReader(stream))
                            return tr.ReadToEnd();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Cannot read resource '{0}' in assembly '{1}'", resource, ex);
                return null;
            }

            Console.WriteLine("Resource '{0}' not found in assembly '{1}'", resource, assembly.FullName); ;
            return null;
        }
    }
}
