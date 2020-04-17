using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using FigmaSharp;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using MonoDevelop.Projects;
using FigmaSharp.Services;
using MonoDevelop.Ide;

namespace MonoDevelop.Figma
{
	public static class Extensions
	{
		public static bool HasAnyFigmaPackage (this Project sender)
		{
			var figmaFolder = sender.GetFigmaFolder();
			if (!System.IO.Directory.Exists(figmaFolder))
				return false;
			return Directory.EnumerateDirectories (figmaFolder).Any ();
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
				namesSpace = $"{dotNetProject.DefaultNamespace}.FigmaPackages";
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

		public static FigmaBundle CreateBundle (this Project project, string fileId, FigmaSharp.Models.FigmaFileVersion version, IFigmaFileProvider fileProvider, string namesSpace = null)
		{
			var figmaFolder = project.GetFigmaFolder();

			if (!Directory.Exists (figmaFolder)) {
				Directory.CreateDirectory (figmaFolder);
			}

			//Bundle generation - We generate an empty bundle and store in the folder
			var fullBundlePath = Path.Combine (figmaFolder, fileId);

			var bundle = FigmaBundle.Empty (fileId, version, fullBundlePath, namesSpace);
			bundle.Reload (fileProvider);
			return bundle;
		}

		public static async Task IncludeBundle (this Project currentProject, FigmaBundle bundle, bool includeImages = false)
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

			//files
			var viewsDirectoryPath = bundle.ViewsDirectoryPath;
			if (!currentProject.PathExistsInProject(viewsDirectoryPath))
			{
				currentProject.AddDirectory(FileService.AbsoluteToRelativePath(currentProject.BaseDirectory, viewsDirectoryPath));
			}

			foreach (var view in bundle.Views)
			{
				if (!currentProject.PathExistsInProject(view.PublicCsClassFilePath))
					currentProject.AddFile(view.PublicCsClassFilePath);

				if (!currentProject.PathExistsInProject(view.PartialDesignerClassFilePath))
				{
					var partialFilePath = currentProject.AddFile(view.PartialDesignerClassFilePath);
					partialFilePath.DependsOn = view.PublicCsClassFilePath;
					
					partialFilePath.Metadata.SetValue(FigmaFile.FigmaPackageId, bundle.FileId);
				}
			}

			await IdeApp.ProjectOperations.SaveAsync (currentProject);
			currentProject.NeedsReload = true;
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
