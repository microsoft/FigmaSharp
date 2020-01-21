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
