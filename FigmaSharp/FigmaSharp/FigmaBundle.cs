using System;
using System.Linq;
using System.IO;
using FigmaSharp.Models;
using System.Collections.Generic;

namespace FigmaSharp
{
	public class FigmaBundle
	{
		public FigmaManifest Manifest { get; set; }

		public List<FigmaBundleView> Views { get; } = new List<FigmaBundleView> ();

		public string DirectoryPath { get; private set; }

		public string Name => DirectoryPath == null ? null : Path.GetFileName (DirectoryPath);
		public string FileId => Manifest.DocumentUrl;

		public string DocumentFilePath => Path.Combine (DirectoryPath, DocumentFileName);
		public string ViewsDirectoryPath => Path.Combine (DirectoryPath, ViewsDirectoryName);

		public string Namespace { get; set; } = "FigmaSharp";

		internal const string FigmaBundleDirectoryExtension = ".figmabundle";
		//const string FigmaBundlesDirectoryName = ".bundles";
		internal const string FigmaDirectoryName = ".figma";
		internal const string ManifestFileName = "manifest.json";
		internal const string DocumentFileName = "document.figma";
		internal const string ResourcesDirectoryName = "Resources";
		internal const string ViewsDirectoryName = "Views";
		internal const string ImageFormat = ".png";

		public void RefreshViews ()
		{
			Views.Clear ();

			if (!Directory.Exists (ViewsDirectoryPath)) {
				return;
			}

			foreach (var viewFullPath in Directory.EnumerateFiles (ViewsDirectoryPath, $"*{FigmaBundleView.PartialDesignerExtension}")) {
				var name = viewFullPath.Substring (0, viewFullPath.Length - FigmaBundleView.PartialDesignerExtension.Length);
				Views.Add (new FigmaBundleView (this, name));
			}
		}

		public void Load (string bundleDirectoryPath)
		{
			if (!Directory.Exists (bundleDirectoryPath)) {
				throw new DirectoryNotFoundException ("directory doesn't exists");
			}

			var manifestFullPath = Path.Combine (bundleDirectoryPath, ManifestFileName);
			if (!File.Exists (manifestFullPath)) {
				throw new FileNotFoundException ("manifest doesn't exists");
			}

			DirectoryPath = bundleDirectoryPath;

			try {
				Manifest = FigmaManifest.FromFilePath (manifestFullPath);
			} catch (Exception ex) {
				throw new FileLoadException ("error reading manifest file", ex);
			}

			RefreshViews ();
		}

		public void Save ()
		{
			if (DirectoryPath == null) {
				return;
			}

			if (!Directory.Exists (DirectoryPath)) {
				Directory.CreateDirectory (DirectoryPath);
			}

			if (Manifest != null) {
				Manifest.Save (Path.Combine (DirectoryPath, ManifestFileName));
			}
		}

		public static FigmaBundle Create (string fileId, string directoryPath)
		{
			var bundle = new FigmaBundle () {
				DirectoryPath = directoryPath
			};
			bundle.Manifest = new FigmaManifest () {
				ApiVersion = AppContext.Current.Version,
				RemoteApiVersion = AppContext.Current.RemoteApiVersion,
				Date = DateTime.Now,
				DocumentUrl = fileId
			};

			return bundle;
		}

		public static FigmaBundle FromDirectoryPath (string fullPath)
		{
			try {
				var bundle = new FigmaBundle ();
				bundle.Load (fullPath);
				return bundle;
			} catch (Exception ex) {
				Console.WriteLine (ex);
				//not a bundle
				return null;
			}
		}

		internal static FigmaResponse GenerateDocumentOutputFile (string fileId, string directoryPath)
		{
			var documentFilePath = Path.Combine (directoryPath, DocumentFileName);
			if (File.Exists (documentFilePath))
				File.Delete (documentFilePath);
			//generate also Document.figma
			var content = FigmaApiHelper.GetFigmaFileContent (fileId);

			File.WriteAllText (documentFilePath, content);
			var figmaResponse = FigmaApiHelper.GetFigmaResponseFromContent (content);
			return figmaResponse;
		}

		internal static void GenerateOutputResourceFiles (string fileId, FigmaResponse figmaResponse, string resourcesDirectoryPath)
		{
			var mainNode = figmaResponse.document.children.FirstOrDefault ();

			var figmaImageIds = mainNode.OfTypeImage ()
				.Select (s => s.id)
				.ToArray ();

			if (figmaImageIds.Length > 0) {

				if (!Directory.Exists (resourcesDirectoryPath)) {
					Directory.CreateDirectory (resourcesDirectoryPath);
				}

				var figmaImageResponse = FigmaApiHelper.GetFigmaImages (fileId, figmaImageIds);
				FileHelper.SaveFiles (resourcesDirectoryPath, ImageFormat, figmaImageResponse.images);
			}
		}

		internal void GenerateLocalDocument (bool includeImages)
		{
			if (string.IsNullOrEmpty (FileId)) {
				throw new InvalidOperationException ("id not set");
			}

			//get image resources
			var figmaResponse = GenerateDocumentOutputFile (Manifest.DocumentUrl, DirectoryPath);

			if (!includeImages)
				return;

			var resourcesDirectoryPath = Path.Combine (DirectoryPath, ResourcesDirectoryName);
			GenerateOutputResourceFiles (FileId, figmaResponse, resourcesDirectoryPath);
		}
	}
}
