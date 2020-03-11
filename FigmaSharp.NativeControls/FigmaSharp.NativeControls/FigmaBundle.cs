/* 
 * CustomTextFieldConverter.cs
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using FigmaSharp.Models;
using FigmaSharp.Services;

namespace FigmaSharp
{
    public class FigmaBundle
	{
		public FigmaFileVersion Version { get; set; }
		public FigmaFileResponse Document { get; set; }
		public FigmaManifest Manifest { get; set; }

		public List<FigmaBundleViewBase> Views { get; } = new List<FigmaBundleViewBase> ();

		public string DirectoryPath { get; private set; }

		public string Name => DirectoryPath == null ? null : Path.GetFileName (DirectoryPath);
		public string FileId => Manifest.FileId;

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

			foreach (var viewFullPath in Directory.EnumerateFiles (ViewsDirectoryPath, $"*{FigmaBundleViewBase.PartialDesignerExtension}")) {
				var name = viewFullPath.Substring (0, viewFullPath.Length - FigmaBundleViewBase.PartialDesignerExtension.Length);
				//TODO: right not it's not possible to read the content of the current .cs file then we create a fake file

				var bundleView = NativeControlsContext.Current.GetBundleView (this, name, new FigmaNode ());
				Views.Add (bundleView);
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
				Version = new FigmaFileVersion() { id = Manifest.DocumentVersion };
			}
			catch (Exception ex) {
				throw new FileLoadException ("error reading manifest file", ex);
			}
		
			RefreshViews ();
		}

		public void Update (FigmaFileVersion version)
		{
			Version = version;
			LoadLocalDocument();
			Save();
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
			Document = AppContext.Api.GetFile (new FigmaFileQuery (FileId, Version));

			if (Manifest != null && Document != null) {
				Manifest.DocumentTitle = Document.name;
				Manifest.DocumentVersion = Document.version;
			}

			if (Version == null) {
				Version = new FigmaFileVersion() { id = Manifest.DocumentVersion };
			}
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

		public static FigmaBundle Empty (string fileId, FigmaFileVersion version, string directoryPath)
		{
			var bundle = new FigmaBundle () {
				DirectoryPath = directoryPath,
				Version = version
			};
			bundle.Manifest = new FigmaManifest () {
				ApiVersion = AppContext.Current.Version,
				RemoteApiVersion = AppContext.Api.Version.ToString (),
				Date = DateTime.Now,
				FileId = fileId,
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

		public static IEnumerable<FigmaNode> OfTypeImage (FigmaNode child)
		{
			if (child.name.Contains ("!image") || child is FigmaVector) {
				yield return child;
			}

			if (child is IFigmaNodeContainer nodeContainer) {
				foreach (var item in nodeContainer.children) {
					foreach (var resultItems in OfTypeImage (item)) {
						yield return resultItems;
					}
				}
			}
		}

		//Generates all the resources from the current .figmafile
		internal static void GenerateOutputResourceFiles (string fileId, FigmaFileResponse figmaResponse, string resourcesDirectoryPath)
		{
			var mainNode = figmaResponse.document.children.FirstOrDefault ();

			var figmaImageIds = OfTypeImage (mainNode)
				.Select (s => s.id)
				.ToArray ();

			if (figmaImageIds.Length > 0) {

				if (!Directory.Exists (resourcesDirectoryPath)) {
					Directory.CreateDirectory (resourcesDirectoryPath);
				}

				var figmaImageResponse = AppContext.Api.GetImages (fileId, figmaImageIds);
				FileHelper.SaveFiles (figmaResponse, resourcesDirectoryPath, ImageFormat, figmaImageResponse.images);
			}
		}

		//reads all the main layers from the remote document
		public void LoadRemoteMainLayers (Services.IFigmaFileProvider figmaFileProvider)
		{
			Views.Clear();
			var mainNodes = figmaFileProvider.GetMainLayers();
			foreach (var item in mainNodes) {
				GenerateFigmaFile (item);
			}
		}

		void GenerateFigmaFile (FigmaNode figmaNode)
		{
			var name = figmaNode.GetClassName ();
			if (HasCorrectClassName (name)) {
				var figmaBundleView = NativeControlsContext.Current.GetBundleView (this, name, figmaNode);
				Views.Add (figmaBundleView);
			} else {
				Console.WriteLine ("Cannot generate a file for '{0}': Invalid ClassName. Skipping...", name);
			}
		}

		bool HasCorrectClassName (string name)
		{
			if (name?.Length == 0)
				return false;
			if (int.TryParse (name, out _)) 
				return false;
			if (char.IsDigit (name[0]))
				return false;

			return true;
		}

		internal void SaveViews (Services.FigmaCodeRendererService codeRendererService, bool writePublicClassIfExists = true)
		{
			foreach (var view in Views) {
				view.Generate (codeRendererService, writePublicClassIfExists);
			}
		}

		/// <summary>
		/// Regenerates all bundle based in the current provider
		/// </summary>
		/// <param name="fileProvider"></param>
		public void Reload (IFigmaFileProvider fileProvider)
		{
			//generate .figma file
			LoadLocalDocument();

			//this reads all the main layers ready and fills our Views models
			LoadRemoteMainLayers(fileProvider);
		}

		public void SaveAll (FigmaCodeRendererService codeRendererService, bool includeImages, bool writePublicClassIfExists = true)
		{
			Save();
			SaveLocalDocument(includeImages);
			SaveViews(codeRendererService, writePublicClassIfExists);
		}

		#endregion
	}
}
