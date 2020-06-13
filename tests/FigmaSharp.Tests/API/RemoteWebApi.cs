using FigmaSharp;
using NUnit.Framework;
using System.Linq;

namespace Api
{
    public static class Resources
    {
		public static string TestToken = "49227-8200a9de-7753-4144-8336-a51dd9d4e8ad";
    }

	[TestFixture]
	public class RemoteWebApi
	{
		public RemoteWebApi ()
		{
			AppContext.Current.SetAccessToken (Resources.TestToken);
		}

		//[Test]
		//public void RemoteConverterTest ()
		//{
		//	var query = new FigmaImageQuery ("Jv8kwhoRsrmtJDsSHcTgWGYu", new[] { "I56:3;56:205" });
		//	var file = AppContext.Api.GetImage (query);
		//	Assert.IsNotNull (file);
		//}

		//[Test]
		//public void GetImage ()
		//{
		//	var fileId = "NxLlq5B5yRY0HbBcia7LmA";
		//	var fileResponse = AppContext.Api.GetFile (new FigmaFileQuery (fileId));
		//	var figmaImages = fileResponse.document.FindImageNodes ().ToArray ();
		//	var image = figmaImages.FirstOrDefault ();


		//	var query = new FigmaImageQuery (fileId, new[] { new  image.id });
		//	var file = AppContext.Api.GetImage (query);
		//	Assert.IsNotNull (file);
		//	Assert.IsNotNull (file.images);
		//	Assert.IsTrue (file.images.Count > 0);
		//}

		[Test]
		public void GetFileTest ()
		{
			var file = AppContext.Api.GetFile (new FigmaFileQuery ("912LElrH9TQsV17omH1WBh"));
			Assert.IsNotNull (file);
		}

		[Test]
		public void GetFileVersionTest ()
		{
			var response = AppContext.Api.GetFileVersions (new FigmaFileVersionQuery ("912LElrH9TQsV17omH1WBh"));
			Assert.IsNotNull (response);
		}

		[Test]
		public void GetFirstFileVersionTest ()
		{
			var fileId = "912LElrH9TQsV17omH1WBh";
			var firstVersion = AppContext.Api.GetFileVersions (new FigmaFileVersionQuery (fileId))
				.versions.GroupByCreatedAt()
				.FirstOrDefault ();

			var file = AppContext.Api.GetFile (new FigmaFileQuery (fileId, firstVersion));
			Assert.IsNotNull (file);
			Assert.AreEqual (file.version, firstVersion.id);
		}
	}
}
