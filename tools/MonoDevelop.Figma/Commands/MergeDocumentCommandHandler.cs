/* 
 * FigmaDisplayBinding.cs 
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

using System.IO;
using System.Threading.Tasks;
using FigmaSharp;
using FigmaSharp.Controls.Cocoa;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Projects;

namespace MonoDevelop.Figma.Commands
{
    class MergeFigmaFileCommandHandler : FigmaCommandHandler
	{
		protected async override void OnRun()
		{
			var selectedItem = IdeApp.ProjectOperations.CurrentSelectedItem;
			if (selectedItem is ProjectFile item)
			{
				if (TryGetProjectFiles(item, out var designerCs, out var publicCs))
				{
					designerCs.DependsOn = publicCs.FilePath;
					await IdeApp.ProjectOperations.SaveAsync(designerCs.Project);
				}
			}
		}

		bool TryGetProjectFiles (ProjectFile item, out ProjectFile designerCs, out ProjectFile publicCs)
        {
			designerCs = null;
			publicCs = null;

			var fileName = item.FilePath.FileNameWithoutExtension;
			var directoryPath = item.FilePath.ParentDirectory.FullPath;

			//we detect what file is
			if (item.IsFigmaDesignerFile ())
			{
				//hackname
				fileName = Path.GetFileNameWithoutExtension(fileName);
				designerCs = item;
				var publicCS = Path.Combine(directoryPath, $"{fileName}{FigmaBundleViewBase.PublicCsExtension}");
				publicCs = item.Project.GetProjectFile(publicCS);
			}
			else if (item.FilePath.Extension == ".cs")
			{
				publicCs = item;
				var designerCSPath = Path.Combine(directoryPath, $"{fileName}{FigmaBundleViewBase.PartialDesignerExtension}");
				designerCs = item.Project.GetProjectFile(designerCSPath);
			}

			if (designerCs != null && publicCs != null)
			{
				return true;
			}
			return false;
        }

		protected override void OnUpdate(CommandInfo info)
		{
			var selectedItem = IdeApp.ProjectOperations.CurrentSelectedItem;
			if (selectedItem is ProjectFile item)
			{
				if (TryGetProjectFiles (item, out var designerCs, out var publicCs) && designerCs.DependsOn != publicCs.FilePath.FullPath)
                {
					info.Visible = info.Enabled = true;
					return;
				}
			}

			info.Visible = info.Enabled = false;
		}
	}
}
