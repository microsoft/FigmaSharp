using NUnit.Framework;
using System.Linq;

namespace FigmaSharp.Tests
{
	[TestFixture]
	public class FigmaApiTests
	{
		public FigmaApiTests ()
		{
			var token = "TOKEN";
			AppContext.Current.SetAccessToken (token);
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
				.versions.OrderBy (s => s.created_at)
				.FirstOrDefault ();

			var file = AppContext.Api.GetFile (new FigmaFileQuery (fileId, firstVersion));
			Assert.IsNotNull (file);
			Assert.AreEqual (file.version, firstVersion.id);
		}
	}
}
