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

using System;
using System.IO;
using System.Linq;

using FigmaSharp.Services;

namespace FigmaSharp.Backup
{
    class Program
	{
		static void Main (string[] args)
		{
			const string noimages = "--noimages";
			const string outputFile = "downloaded.figma";
			Console.WriteLine ();
			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine ("Figma Remote File Exporter");
			Console.WriteLine ("--------------------------");
			Console.WriteLine ();

			Console.ForegroundColor = default (ConsoleColor);

			#region Parameters

			if (args.Length == 0) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine ("Error. No parameters defined");
				Console.ForegroundColor = default (ConsoleColor);

				Console.WriteLine ("");
				Console.WriteLine ($"dotnet FigmaFileExporter.dll [document_id] [figma_token] {{output_directory}} {{{noimages}}}");
				Console.WriteLine ("");
				return;
			}

			string token = null;
			if (args.Length > 1) {
				token = args[1];
			}

			if (string.IsNullOrEmpty (token)) {
				Console.WriteLine ("Error. Figma Token is not defined.");
				return;
			}

			Console.WriteLine ($"Token: {token}");

			string outputDirectory = null;
			if (args.Length > 2 && Directory.Exists (outputDirectory)) {
				outputDirectory = args[2];
			}

			if (outputDirectory == null) {
				Console.WriteLine ("Output directory is not defined. Using current directory like default.");
				outputDirectory = Directory.GetCurrentDirectory ();
			}

			Console.WriteLine ($"Default Directory: {outputDirectory}");

			var processImages = !args.Any (s => s.ToLower () == noimages);

			#endregion

			FigmaSharp.AppContext.Current.SetAccessToken (token);

			var fileId = args[0];

			var outputFilePath = Path.Combine (outputDirectory, outputFile);
			if (File.Exists (outputFilePath)) {
				File.Delete (outputFilePath);
			}

			Console.WriteLine ();
			Console.WriteLine ("[Import] Starting from remote document '{0}' ({1} images) in local file: {2}", fileId, processImages ? "with" : "without", outputFilePath);

			var query = new FigmaFileQuery (fileId);

			var fileProvider = new RemoteNodeProvider();
			fileProvider.Load(fileId);

			fileProvider.Save(outputFilePath);

			Console.WriteLine ("[Import] Success.");

			if (processImages) {

				var mainNode = fileProvider.Response.document.children.FirstOrDefault ();
				var figmaImageNodes = fileProvider.SearchImageNodes(fileProvider.Response.document)
					.ToArray ();

				Console.WriteLine ("[Import] Downloading {0} image/s...", figmaImageNodes.Length);

				var figmaImageIds = figmaImageNodes.Select (s => fileProvider.CreateEmptyImageNodeRequest (s)).ToArray ();
				if (figmaImageIds.Length > 0) {
					FigmaSharp.AppContext.Api.ProcessDownloadImages (fileId, figmaImageIds, scale: 2);
					FigmaSharp.AppContext.Api.ProcessDownloadImages(fileId, figmaImageIds, scale: 1);
					fileProvider.SaveResourceFiles(outputDirectory, ".png", figmaImageIds);
				}
				Console.WriteLine ("[Import] Success.");
			}
		}
	}
}
