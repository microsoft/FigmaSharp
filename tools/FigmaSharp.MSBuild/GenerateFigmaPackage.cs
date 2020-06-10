using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FigmaSharp.Services;
using Microsoft.Build.Framework;

namespace FigmaSharp.MSBuild.Tasks
{
	public class GenerateFigmaPackage : Microsoft.Build.Utilities.Task
	{
		[Required]
		public string Token { get; set; }

		[Required]
		public string FileId { get; set; }

		public string OutputDirectoryPath { get; set; }

		public string OutputFigmaDocument { get; set; }

		public string OutputImageFormat { get; set; } = ".png";

		public bool ProcessImages { get; set; } = true;

		public override bool Execute()
		{
			try
			{
				#region Parameters

				if (string.IsNullOrEmpty(OutputFigmaDocument))
					OutputFigmaDocument = "document.figma";

				if (string.IsNullOrEmpty(OutputImageFormat))
					OutputImageFormat = ".png";

				if (string.IsNullOrEmpty (Token) || string.IsNullOrEmpty (FileId))
				{
					Log.LogError ("Error. No token and fileId parameters are defined");
					return false;
				}

				if (string.IsNullOrEmpty (OutputDirectoryPath))
				{
					Log.LogMessage("Output directory is not defined. Using current directory like default.");
					OutputDirectoryPath = Directory.GetCurrentDirectory();
				}
				else if (!Directory.Exists(OutputDirectoryPath))
                {
					Log.LogError("Error. OutputDirectory doesn't exists or is not valid");
					return false;
				}

				Log.LogMessage ($"Default Directory: {OutputDirectoryPath}");

				#endregion

				FigmaSharp.AppContext.Current.SetAccessToken(Token);

				var outputFilePath = Path.Combine(OutputDirectoryPath, OutputFigmaDocument);
				if (File.Exists(outputFilePath))
					File.Delete(outputFilePath);

				Log.LogMessage("");
				Log.LogMessage("[Import] Starting from remote document '{0}' ({1} images) in local file: {2}", FileId, ProcessImages ? "with" : "without", outputFilePath);

				var query = new FigmaFileQuery(FileId);

				var fileProvider = new RemoteNodeProvider();
				fileProvider.Load(FileId);

				fileProvider.Save(outputFilePath);

				Log.LogMessage("[Import] Success.");

				if (ProcessImages)
				{

					var mainNode = fileProvider.Response.document.children.FirstOrDefault();
					var figmaImageNodes = fileProvider.SearchImageNodes(fileProvider.Response.document)
						.ToArray();

					Log.LogMessage("[Import] Downloading {0} image/s...", figmaImageNodes.Length);

					var figmaImageIds = figmaImageNodes.Select(s => fileProvider.CreateEmptyImageNodeRequest(s)).ToArray();
					if (figmaImageIds.Length > 0)
					{
						FigmaSharp.AppContext.Api.ProcessDownloadImages(FileId, figmaImageIds, scale: 2);
						FigmaSharp.AppContext.Api.ProcessDownloadImages(FileId, figmaImageIds, scale: 1);
						fileProvider.SaveResourceFiles(OutputDirectoryPath, ".png", figmaImageIds);
					}
					Log.LogMessage("[Import] Success.");
				}
				return true;
			}
			catch (Exception ex)
			{
				Log.LogError($"Cannot execute GenerateBuildInfoFiles Task. Exception: {ex}");
			}
			return false;
		}
	}
}