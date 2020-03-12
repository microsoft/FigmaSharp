using NUnit.Framework;
using System.Linq;

namespace FigmaSharp.Tests
{
	[TestFixture]
	public class FigmaApiTests
	{
		public FigmaApiTests ()
		{
			var token = "XX";
			AppContext.Current.SetAccessToken (token);
		}

		[Test]
		public void RemoteConverterTest ()
		{
			var query = new FigmaImageQuery ("Jv8kwhoRsrmtJDsSHcTgWGYu", new[] { "I56:3;56:205" });
			var file = AppContext.Api.GetImage (query);
			Assert.IsNotNull (file);
		}

		[Test]
		public void GetImage ()
		{
			var fileId = "NxLlq5B5yRY0HbBcia7LmA";
			var fileResponse = AppContext.Api.GetFile (new FigmaFileQuery (fileId));
			var figmaImages = fileResponse.document.FindImageNodes ().ToArray ();
			var image = figmaImages.FirstOrDefault ();

			var query = new FigmaImageQuery (fileId, new[] { image.id });
			var file = AppContext.Api.GetImage (query);
			Assert.IsNotNull (file);
			Assert.IsNotNull (file.images);
			Assert.IsTrue (file.images.Count > 0);
		}

		[Test]
		public void GetFileTest ()
		{
			var file = AppContext.Api.GetFile (new FigmaFileQuery ("fKugSkFGdwOF4vDsPGnJee"));
			Assert.IsNotNull (file);
		}

		[Test]
		public void GetFileVersionTest ()
		{
			var response = AppContext.Api.GetFileVersions (new FigmaFileVersionQuery ("QzEgq2772k2eeMF2sVNc3kEY"));
			Assert.IsNotNull (response);
		}

		[Test]
		public void GetFirstFileVersionTest ()
		{
			var fileId = "QzEgq2772k2eeMF2sVNc3kEY";
			var firstVersion = AppContext.Api.GetFileVersions (new FigmaFileVersionQuery (fileId))
				.versions.GroupByCreatedAt()
				.FirstOrDefault ();

			var file = AppContext.Api.GetFile (new FigmaFileQuery (fileId, firstVersion));
			Assert.IsNotNull (file);
			Assert.AreEqual (file.version, firstVersion.id);
		}
	}
}
