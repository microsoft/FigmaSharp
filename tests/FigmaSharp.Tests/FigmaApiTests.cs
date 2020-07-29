// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Linq;
using NUnit.Framework;

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
