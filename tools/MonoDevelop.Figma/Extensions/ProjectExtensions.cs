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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FigmaSharp;
using FigmaSharp.Controls.Cocoa.Services;
using FigmaSharp.Models;
using FigmaSharp.Services;

using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using MonoDevelop.Projects;

namespace MonoDevelop.Figma
{
    public static class Extensions
	{
		public static async Task UpdateFigmaFilesAsync (this Project sender, ProgressMonitor monitor, IEnumerable<ProjectFile> projectFiles, FigmaBundle figmaBundle, FigmaFileVersion version, bool translateStrings)
        {
			var includeImages = true;
			var fileProvider = new ControlRemoteNodeProvider() { Version = version };
			await fileProvider.LoadAsync(figmaBundle.FileId);

			FigmaSharp.Services.LoggingService.LogInfo($"[Done] Loaded Remote File provider for Version {version?.id ?? "Current"}");
			var codeRendererService = new NativeViewCodeService(fileProvider) {
				TranslationService = new Services.MonoDevelopTranslationService()
			};

			await Task.Run(() => {
				figmaBundle.Update(version, fileProvider, includeImages: includeImages);
			});
			await sender.IncludeBundleAsync(monitor, figmaBundle, includeImages: includeImages);

			foreach (var designerFile in projectFiles) {
				if (designerFile.TryGetFigmaNode(fileProvider, out var figmaNode)) {
					var fileView = figmaBundle.GetFigmaFileView(figmaNode);
					fileView.GeneratePartialDesignerClass(codeRendererService,
						designerFile.FilePath.ParentDirectory,
						figmaBundle.Namespace,
						translateStrings);

					await sender.FormatFileAsync(designerFile.FilePath);
				}
			}
		}

		public static bool TryGetNodeName (this ProjectFile sender, out string nodeName)
		{
			nodeName = sender.Metadata.GetValue(FigmaFile.FigmaNodeCustomName, null);
			return !string.IsNullOrEmpty (nodeName);
		}

		public static bool TryGetFigmaPackageId (this ProjectFile sender, out string figmaPackageId)
		{
			figmaPackageId = sender.Metadata.GetValue(FigmaFile.FigmaPackageId, null);
			return !string.IsNullOrEmpty(figmaPackageId);
		}

		public static bool TryGetFigmaNode (this ProjectFile sender, NodeProvider fileProvider, out FigmaNode figmaNode)
		{
			if (sender.HasFigmaDesignerFileExtension () && TryGetNodeName (sender,out var figmaName)) {
				figmaNode = fileProvider.FindByCustomName (figmaName);
				return figmaNode != null;
            }
			figmaNode = null;
			return false;
		}

		public static bool HasFigmaDesignerFileExtension (this ProjectFile sender)
		{
			return sender.Metadata.HasProperty(FigmaFile.FigmaPackageId) &&
				sender.FilePath.FullPath.ToString()
				.EndsWith(FigmaBundleViewBase.PartialDesignerExtension);
		}

		public static bool IsFigmaDesignerFile(this ProjectFile file)
		{
			if (file.FilePath.FullPath.ToString().EndsWith(FigmaBundleViewBase.PartialDesignerExtension))
			{
				if (file.Metadata.HasProperty(FigmaFile.FigmaPackageId)
					&& file.Metadata.HasProperty(FigmaFile.FigmaNodeCustomName))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsFigmaCSFile (this ProjectFile file, out ProjectFile designerFile)
		{
			var filePath = file.FilePath.FullPath.ToString();
			if (!filePath.EndsWith(FigmaBundleViewBase.PartialDesignerExtension) && filePath.EndsWith(FigmaBundleViewBase.PublicCsExtension))
			{
				var fileName = Path.GetFileNameWithoutExtension(filePath) + FigmaBundleViewBase.PartialDesignerExtension;
				var fullpath = Path.Combine(file.FilePath.ParentDirectory.FullPath, fileName);
				var projectFile = file.Project.GetProjectFile(fullpath);
				if (projectFile != null && IsFigmaDesignerFile(projectFile))
				{
					designerFile = projectFile;
					return true;
				}
			}
			designerFile = null;
			return false;
		}

		public static bool HasAnyFigmaPackage (this Project sender)
		{
			var figmaFolder = sender.GetFigmaFolder();
			if (!System.IO.Directory.Exists(figmaFolder))
				return false;
			return Directory.EnumerateDirectories (figmaFolder).Any ();
		}

		public static async Task AddFigmaBundleViewAsync (this Project sender, FigmaBundleViewBase figmaBundleView, bool savesInProject = true)
		{
			await sender.FormatFileAsync(figmaBundleView.PublicCsClassFilePath);
			await sender.FormatFileAsync(figmaBundleView.PartialDesignerClassFilePath);

			if (!sender.PathExistsInProject(figmaBundleView.PublicCsClassFilePath))
				sender.AddFile(figmaBundleView.PublicCsClassFilePath);

            if (!sender.PathExistsInProject(figmaBundleView.PartialDesignerClassFilePath))
            {
                var partialFilePath = sender.AddFile(figmaBundleView.PartialDesignerClassFilePath);
                partialFilePath.DependsOn = figmaBundleView.PublicCsClassFilePath;

                partialFilePath.Metadata.SetValue(FigmaFile.FigmaPackageId, figmaBundleView.Bundle.FileId);

				if (!figmaBundleView.FigmaNode.TryGetNodeCustomName (out var customName)) {
					customName = figmaBundleView.FigmaNode.name;
				}
				partialFilePath.Metadata.SetValue(FigmaFile.FigmaNodeCustomName, customName);
			}

            if (savesInProject)
				await IdeApp.ProjectOperations.SaveAsync(sender);
		}

		//TODO: Convert to Async
		public static IEnumerable<FigmaBundle> GetFigmaPackages (this Project sender)
		{
			var figmaFolder = sender.GetFigmaFolder();
			foreach (var figmaProject in System.IO.Directory.GetDirectories(figmaFolder))
			{
				yield return FigmaBundle.FromDirectoryPath(figmaProject);
			}
		}

		public static bool TryGetProject (this object sender, out Project project)
		{
			if (sender is Project project1) {
				project = project1;
			}
			else if (sender is ProjectFolder projectItem) {
				project = projectItem.Project;
			}
			else
				project = null;
			return project != null;
		}

		public static string GetDefaultFigmaNamespace (this Project currentProject)
		{
			string namesSpace;
			//set namespace
			if (currentProject is DotNetProject dotNetProject) {
				namesSpace = dotNetProject.DefaultNamespace;
			}
			else
			{
				namesSpace = "FigmaSharp.FigmaPackages";
			}
			return namesSpace;
		}

		public static string GetFigmaFolder (this Project currentProject)
		{
			return Path.Combine(currentProject.BaseDirectory.FullPath, FigmaBundle.FigmaDirectoryName);
		}

		public static IEnumerable<ProjectFile> GetAllFigmaDesignerFiles (this Project currentProject)
		{
			return currentProject.Files.OfType<ProjectFile>().Where(s => s.HasFigmaDesignerFileExtension());
		}

		public static FigmaBundle CreateBundle (this Project project, string fileId, FigmaSharp.Models.FigmaFileVersion version, INodeProvider fileProvider, string namesSpace = null)
		{
			var figmaFolder = project.GetFigmaFolder();

			if (!Directory.Exists (figmaFolder)) {
				Directory.CreateDirectory (figmaFolder);
			}

			//Bundle generation - We generate an empty bundle and store in the folder
			var fullBundlePath = Path.Combine (figmaFolder, fileId);

			var bundle = FigmaBundle.Empty (fileId, version, fullBundlePath, namesSpace);
			bundle.Reload ();
			return bundle;
		}

		public static async Task IncludeBundleAsync (this Project currentProject, ProgressMonitor monitor, FigmaBundle bundle, bool includeImages = false, bool savesInProject = true)
		{
			using var task = monitor.BeginTask("Including files into current project…", 1);

			var figmaFolder = Path.Combine(currentProject.BaseDirectory.FullPath, FigmaBundle.FigmaDirectoryName);
			if (!currentProject.PathExistsInProject(figmaFolder))
			{
				//we add figma folder in the case doesn't exists
				currentProject.AddDirectory(FileService.AbsoluteToRelativePath(currentProject.BaseDirectory, figmaFolder));
			}

			//now we need to add the content
			//bundle
			var fullBundlePath = bundle.DirectoryPath;
			if (!currentProject.PathExistsInProject(fullBundlePath))
			{
				currentProject.AddDirectory(FileService.AbsoluteToRelativePath(currentProject.BaseDirectory, fullBundlePath));
			}

			//manifest
			var manifestFullDirectoryPath = bundle.ManifestFilePath;
			if (!currentProject.PathExistsInProject(manifestFullDirectoryPath))
				currentProject.AddFile(manifestFullDirectoryPath);


			//document
			var documentFullDirectoryPath = bundle.DocumentFilePath;
			if (!currentProject.PathExistsInProject(documentFullDirectoryPath))
				currentProject.AddFile(documentFullDirectoryPath);

			//TODO: images are not enabled by now
			if (includeImages)
			{
				//resources
				var resourcesDirectoryPath = bundle.ResourcesDirectoryPath;
				currentProject.AddDirectory(FileService.AbsoluteToRelativePath(currentProject.BaseDirectory, resourcesDirectoryPath));

				//we add to the project for each resource inside the 
				//foreach (var image in Directory.EnumerateFiles (resourcesDirectoryPath, "*.png")) {
				//	currentProject.AddFile (image);
				//}

				var images = Directory.EnumerateFiles(resourcesDirectoryPath, $"*{FigmaBundle.ImageFormat}").Select(s => new FilePath(s));

				foreach (var item in images)
				{
					if (!currentProject.PathExistsInProject(item))
					{
						var projectFile = new ProjectFile(item, BuildAction.BundleResource);
						projectFile.ResourceId = item.FileName;
						currentProject.AddFile(projectFile);
					}
				}
			}
			if (savesInProject)
				await IdeApp.ProjectOperations.SaveAsync(currentProject);
		}

		public static bool IsDocumentDirectoryBundle (this ProjectFolder pr)
		{
			return
			//pr.Path.Extension == FigmaBundle.FigmaBundleDirectoryExtension
			//&& pr.Parent is ProjectFolder figmaBundles && figmaBundles.Path.FileName == FigmaBundlesDirectoryName &&
			pr.Parent is ProjectFolder figmaProject && figmaProject.Path.FileName == FigmaBundle.FigmaDirectoryName
			&& figmaProject.Parent is Project;
		}

		public static bool IsFigmaManifest (this ProjectFile pr)
		{
			if (pr.FilePath.FileName != FigmaBundle.DocumentFileName) {
				return false;
			}

			var bundleDirectory = pr.FilePath.ParentDirectory;
			if (bundleDirectory == null || bundleDirectory.Extension != FigmaBundle.FigmaBundleDirectoryExtension) {
				return false;
			}

			var figmaDirectory = bundleDirectory.ParentDirectory;
			if (figmaDirectory == null || figmaDirectory.FileName != FigmaBundle.FigmaDirectoryName) {
				return false;
			}

			return figmaDirectory.ParentDirectory.FullPath == pr.Project.BaseDirectory.FullPath;
		}

		public static bool IsFigmaDirectory (this ProjectFolder pr)
		{
			return pr.Path.Extension == FigmaBundle.FigmaDirectoryName
			&& pr.Parent is Project;
		}

		//public static bool IsFigmaBundleDirectory (this ProjectFolder pr)
		//{
		//	return pr.Path.FileName == FigmaBundlesDirectoryName
		//		&& pr.Parent is ProjectFolder figmaProject && figmaProject.Path.FileName == FigmaDirectoryName
		//		&& figmaProject.Parent is Project;
		//}

		public static async Task FormatFileAsync(this Project project, string file)
		{
			try
			{
				await FormatFileInternalAsync(project, file);
			}
			catch (Exception ex)
			{
				FigmaSharp.Services.LoggingService.LogError("File formatting failed", ex);
			}
		}

		static async Task FormatFileInternalAsync(Project project, string file)
		{
			var template = new CustomSingleFileDescriptionTemplate();
			Stream stream = await template.CreateFileContentAsync(project, project, "C#", file, string.Empty);

			byte[] buffer = new byte[2048];
			int nr;
			FileStream fs = null;
			try
			{
				fs = File.Create(file);
				while ((nr = stream.Read(buffer, 0, 2048)) > 0)
					fs.Write(buffer, 0, nr);
			}
			finally
			{
				stream.Close();
				if (fs != null)
					fs.Close();
			}
		}
	}
}
