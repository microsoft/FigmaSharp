using System.IO;
using FigmaSharp.Helpers;
using NUnit.Framework;

namespace Api
{
	[TestFixture]
	public class JsonFile
	{
		const string fileExample = "test.json";
		readonly string currentPath;
		public JsonFile()
		{
			currentPath = Path.GetDirectoryName (this.GetType ().Assembly.Location);
		}

		[Test]
		public void LocalFileTest ()
		{
			var file = Path.Combine (currentPath, fileExample);
			Assert.IsNotNull (file);
		}
	}
}
