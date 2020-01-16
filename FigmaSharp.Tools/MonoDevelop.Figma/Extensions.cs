using System;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using MonoDevelop.Projects;

namespace MonoDevelop.Figma
{
	public static class Extensions
	{
		const string FigmaBundleDirectoryExtension = ".figmabundle";
		const string FigmaBundlesDirectoryName = ".figmabundles";

		public static bool IsDocumentDirectoryBundle (this ProjectFolder pr)
		{
			return pr.Path.Extension == FigmaBundleDirectoryExtension
			&& pr.Parent is ProjectFolder figmaBundles
			&& figmaBundles.Path.FileName == FigmaBundlesDirectoryName
			&& figmaBundles.Parent is Project;
		}

		public static bool IsFigmaBundleDirectory (this ProjectFolder pr)
		{
			return pr.Path.FileName == FigmaBundlesDirectoryName && pr.Parent is Project;
		}
	}
}
