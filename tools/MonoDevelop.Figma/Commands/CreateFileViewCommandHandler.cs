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

using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using MonoDevelop.Projects;

namespace MonoDevelop.Figma.Commands
{
    class FigmaNewFileViewCommandHandler : FigmaCommandHandler
    {
        protected override void OnUpdate(CommandInfo info)
        {
            var selectedItem = IdeApp.ProjectOperations.CurrentSelectedItem;

            info.Visible = info.Enabled = selectedItem.TryGetProject(out var project) && project.HasAnyFigmaPackage() && (selectedItem is ProjectFolder || selectedItem is Project);
        }

        protected async override void OnRun()
        {
            var selectedItem = IdeApp.ProjectOperations.CurrentSelectedItem;
            string filePath = null;
            Project project = null;
            if (selectedItem is Project project1)
            {
                project = project1;
                filePath = project1.FileName.ParentDirectory;
            }
            else if (selectedItem is ProjectFolder projectItem)
            {
                project = projectItem.Project;
                filePath = projectItem.Path.FullPath;
            }
            else
                return;

            var figmaBundleWindow = new GenerateViewsWindow(filePath, project);
            await figmaBundleWindow.LoadAsync();

            var currentIdeWindow = Components.Mac.GtkMacInterop.GetNSWindow(IdeApp.Workbench.RootWindow);
            currentIdeWindow.AddChildWindow(figmaBundleWindow, AppKit.NSWindowOrderingMode.Above);
            MessageService.PlaceDialog(figmaBundleWindow, MessageService.RootWindow);
            IdeServices.DesktopService.FocusWindow(figmaBundleWindow);
        }
    }
}
