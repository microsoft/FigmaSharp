using System.Reflection;

namespace BasicGraphics.Cocoa
{
	static class FileHelper
	{
		public static string GetFileDataFromBundle (string fileName)
		{
			var path = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
			path = System.IO.Path.Combine(path, "Resources", "svg", fileName);
			var data = System.IO.File.ReadAllText(path);
			return data;
		}
	}
}
