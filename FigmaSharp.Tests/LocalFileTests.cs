using System.IO;
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
			var doc = FigmaApiHelper.GetFigmaResponseFromFileContent (data);
			Assert.IsNotNull (file);
		}
	}
}
