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
		public static async Task UpdateFigmaFilesAsync (this Project sender, IEnumerable<ProjectFile> projectFiles, FigmaBundle figmaBundle, FigmaFileVersion version, bool translateStrings)
        {
			var includeImages = true;
			var fileProvider = new ControlsRemoteFileProvider() { Version = version };
			await fileProvider.LoadAsync(figmaBundle.FileId);

			Console.WriteLine($"[Done] Loaded Remote File provider for Version {version?.id ?? "Current"}");
			var codeRendererService = new NativeViewCodeService(fileProvider);

			await Task.Run(() => {
				figmaBundle.Update(version, fileProvider, includeImages: includeImages);
			});
			await sender.IncludeBundleAsync(figmaBundle, includeImages: includeImages);

			foreach (var designerFile in projectFiles) {
				if (designerFile.TryGetFigmaNode(fileProvider, out var figmaNode)) {
					var fileView = figmaBundle.GetFigmaFileView(figmaNode);
					fileView.GeneratePartialDesignerClass(codeRendererService,
						designerFile.FilePath.ParentDirectory,
						figmaBundle.Namespace,
						translateStrings);
				}
			}
		}

		public static bool TryGetNodeName (this ProjectFile sender, out string nodeName)
		{
			nodeName = sender.Metadata.GetValue(FigmaFile.FigmaNodeId, null);
			return !string.IsNullOrEmpty (nodeName);
		}

		public static bool TryGetFigmaPackageId (this ProjectFile sender, out string figmaPackageId)
		{
			figmaPackageId = sender.Metadata.GetValue(FigmaFile.FigmaPackageId, null);
			return !string.IsNullOrEmpty(figmaPackageId);
		}

		public static bool TryGetFigmaNode (this ProjectFile sender, NodeProvider fileProvider, out FigmaNode figmaNode)
		{
			if (sender.IsFigmaDesignerFile () && TryGetNodeName (sender,out var figmaName)) {
				figmaNode = fileProvider.FindById (figmaName);
				return figmaNode != null;
            }
			figmaNode = null;
			return false;
		}

		public static bool IsFigmaDesignerFile (this ProjectFile sender)
		{
			return sender.Metadata.HasProperty(FigmaFile.FigmaPackageId) &&
				sender.FilePath.FullPath.ToString()
				.EndsWith(FigmaBundleViewBase.PartialDesignerExtension);
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
            if (!sender.PathExistsInProject(figmaBundleView.PublicCsClassFilePath))
				sender.AddFile(figmaBundleView.PublicCsClassFilePath);

            if (!sender.PathExistsInProject(figmaBundleView.PartialDesignerClassFilePath))
            {
                var partialFilePath = sender.AddFile(figmaBundleView.PartialDesignerClassFilePath);
                partialFilePath.DependsOn = figmaBundleView.PublicCsClassFilePath;

                partialFilePath.Metadata.SetValue(FigmaFile.FigmaPackageId, figmaBundleView.Bundle.FileId);
				partialFilePath.Metadata.SetValue(FigmaFile.FigmaNodeId, figmaBundleView.FigmaNode.id);
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
			return currentProject.Files.OfType<ProjectFile>().Where(s => s.IsFigmaDesignerFile());
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

		public static async Task IncludeBundleAsync (this Project currentProject, FigmaBundle bundle, bool includeImages = false, bool savesInProject = true)
		{
			Ide.IdeApp.Workbench.StatusBar.ShowMessage("Including files into current project…");

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
						projectFile.Metadata.SetValue("LogicalName", projectFile.ResourceId, "");
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
	}
}
