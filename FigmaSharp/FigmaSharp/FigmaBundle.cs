using System;
using System.Linq;
using System.IO;
using FigmaSharp.Models;
using System.Collections.Generic;

namespace FigmaSharp
{
	public class FigmaBundle
	{
		public FigmaFileResponse Document { get; set; }
		public FigmaManifest Manifest { get; set; }

		public List<FigmaBundleView> Views { get; } = new List<FigmaBundleView> ();

		public string DirectoryPath { get; private set; }

		public string Name => DirectoryPath == null ? null : Path.GetFileName (DirectoryPath);
		public string FileId => Manifest.DocumentUrl;

		public string DocumentFilePath => Path.Combine (DirectoryPath, DocumentFileName);
		public string ViewsDirectoryPath => Path.Combine (DirectoryPath, ViewsDirectoryName);
		public string ResourcesDirectoryPath => Path.Combine (DirectoryPath, ResourcesDirectoryName);
		public string ManifestFilePath => Path.Combine (DirectoryPath, ManifestFileName);

		public string Namespace { get; set; } = "FigmaSharp";
	
		internal const string FigmaBundleDirectoryExtension = ".figmabundle";
		//const string FigmaBundlesDirectoryName = ".bundles";
		internal const string FigmaDirectoryName = ".figma";
		internal const string ManifestFileName = "manifest.json";
		internal const string DocumentFileName = "document.figma";
		internal const string ResourcesDirectoryName = "Resources";
		internal const string ViewsDirectoryName = "Views";
		internal const string ImageFormat = ".png";

		//reads all the views from the current views directory with empty FigmaBundleViews (not implemented the read)
		public void RefreshViews ()
		{
			Views.Clear ();

			if (!Directory.Exists (ViewsDirectoryPath)) {
				return;
			}

			foreach (var viewFullPath in Directory.EnumerateFiles (ViewsDirectoryPath, $"*{FigmaBundleView.PartialDesignerExtension}")) {
				var name = viewFullPath.Substring (0, viewFullPath.Length - FigmaBundleView.PartialDesignerExtension.Length);
				//TODO: right not it's not possible to read the content of the current .cs file then we create a fake file
				Views.Add (new FigmaBundleView (this, name, null));
			}
		}

		//this happens when we call to FigmaBundle.FromDirectoryPath
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

		internal void LoadLocalDocument ()
		{
			//generate also Document.figma
			Document = AppContext.Api.GetFile (new FigmaFileQuery (FileId));
		}

		//Generates the .figmafile
		internal void SaveLocalDocument (bool includeImages)
		{
			if (string.IsNullOrEmpty (FileId)) {
				throw new InvalidOperationException ("id not set");
			}

			if (Document == null) {
				throw new InvalidOperationException ("document not loaded not set");
			}
			var documentFilePath = DocumentFilePath;
			if (File.Exists (documentFilePath))
				File.Delete (documentFilePath);

			Document.Save (documentFilePath);

			//get image resources
			if (!includeImages)
				return;

			var resourcesDirectoryPath = Path.Combine (DirectoryPath, ResourcesDirectoryName);
			GenerateOutputResourceFiles (FileId, Document, resourcesDirectoryPath);
		}

		public void GenerateDocuments ()
		{

		}

		#region Static Methods

		public static FigmaBundle Create (string fileId, string directoryPath)
		{
			var bundle = new FigmaBundle () {
				DirectoryPath = directoryPath
			};
			bundle.Manifest = new FigmaManifest () {
				ApiVersion = AppContext.Current.Version,
				RemoteApiVersion = AppContext.Api.Version.ToString (),
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

		//Generates all the resources from the current .figmafile
		internal static void GenerateOutputResourceFiles (string fileId, FigmaFileResponse figmaResponse, string resourcesDirectoryPath)
		{
			var mainNode = figmaResponse.document.children.FirstOrDefault ();

			var figmaImageIds = mainNode.OfTypeImage ()
				.Select (s => s.id)
				.ToArray ();

			if (figmaImageIds.Length > 0) {

				if (!Directory.Exists (resourcesDirectoryPath)) {
					Directory.CreateDirectory (resourcesDirectoryPath);
				}

				var figmaImageResponse = AppContext.Api.GetImages (fileId, figmaImageIds);
				FileHelper.SaveFiles (resourcesDirectoryPath, ImageFormat, figmaImageResponse.images);
			}
		}

		//reads all the main layers from the remote document
		public void LoadRemoteMainLayers (Services.IFigmaFileProvider figmaFileProvider)
		{
			var mainNodes = figmaFileProvider.Nodes
				.Where (s => s.Parent is FigmaCanvas)
				.ToArray ();

			foreach (var item in mainNodes) {
				GenerateFigmaFile (item);
			}
		}

		void GenerateFigmaFile (FigmaNode figmaNode)
		{
			var name = figmaNode.GetRealName ();
			var figmaBundleView = new FigmaBundleView (this, name, figmaNode);
			Views.Add (figmaBundleView);
		}

		internal void SaveViews (Services.FigmaCodeRendererService codeRendererService)
		{
			foreach (var view in Views) {
				view.Generate (codeRendererService);
			}
		}

		#endregion
	}
}
