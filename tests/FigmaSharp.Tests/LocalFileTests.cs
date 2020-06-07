using System.IO;
using FigmaSharp.Helpers;
using NUnit.Framework;

namespace FigmaSharp.Tests
{
	[TestFixture]
	public class LocalFileTests
	{
		const string fileExample = "test.json";
		readonly string currentPath;
		public LocalFileTests ()
		{
			currentPath = Path.GetDirectoryName (this.GetType ().Assembly.Location);
		}

		[Test]
		public void LocalFileTest ()
		{
			var file = Path.Combine (currentPath, fileExample);
			var data = File.ReadAllText (file);
			var doc = WebApiHelper.GetFigmaResponseFromFileContent (data);
			Assert.IsNotNull (file);
		}
	}
}
