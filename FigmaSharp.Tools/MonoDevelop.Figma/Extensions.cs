using System;
using FigmaSharp;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using MonoDevelop.Projects;

namespace MonoDevelop.Figma
{
	public static class Extensions
	{
		public static bool IsDocumentDirectoryBundle (this ProjectFolder pr)
		{
			return pr.Path.Extension == FigmaBundle.FigmaBundleDirectoryExtension
			//&& pr.Parent is ProjectFolder figmaBundles && figmaBundles.Path.FileName == FigmaBundlesDirectoryName
			&& pr.Parent is ProjectFolder figmaProject && figmaProject.Path.FileName == FigmaBundle.FigmaDirectoryName
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
