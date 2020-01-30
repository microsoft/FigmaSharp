using System;
using System.IO;
using FigmaSharp;
using System.Linq;

namespace FigmaDocumentExporter.Shell
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
			var response = FigmaSharp.AppContext.Api.GetFile (query);
			response.Save (outputFilePath);

			Console.WriteLine ("[Import] Success.");

			if (processImages) {

				var mainNode = response.document.children.FirstOrDefault ();
				var figmaModelImages = mainNode.OfTypeImage ().ToArray ();

				Console.WriteLine ("[Import] Downloading {0} image/s...", figmaModelImages.Length);

				var figmaImageIds = figmaModelImages.Select (s => s.id).ToArray ();
				if (figmaImageIds.Length > 0) {
					var figmaImageResponse = FigmaSharp.AppContext.Api.GetImages (fileId, figmaImageIds);
					FileHelper.SaveFiles (outputDirectory, ".png", figmaImageResponse.images);
				}
				Console.WriteLine ("[Import] Success.");
			}
		}
	}
}
