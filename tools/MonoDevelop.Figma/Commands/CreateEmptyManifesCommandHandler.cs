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

using System;
using System.IO;
using FigmaSharp;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;

namespace MonoDevelop.Figma.Commands
{
    class CreateEmptyManifesCommandHandler : FigmaCommandHandler
    {
        protected override void OnUpdate(CommandInfo info)
        {
            info.Visible = info.Enabled = IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder &&
                    currentFolder.IsDocumentDirectoryBundle() &&
                    !File.Exists(Path.Combine(currentFolder.Path.FullPath, FigmaBundle.ManifestFileName));
        }

        protected async override void OnRun()
        {
            if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder)
            {
                try
                {
                    var manifestFilePath = Path.Combine(currentFolder.Path.FullPath, FigmaBundle.ManifestFileName);

                    if (!File.Exists(manifestFilePath))
                    {
                        var project = currentFolder.Project;

                        var manifest = new FigmaManifest()
                        {
                            Namespace = project.GetDefaultFigmaNamespace(),
                            DocumentVersion = "0",
                            ApiVersion = FigmaSharp.AppContext.Current.Version,
                            RemoteApiVersion = FigmaSharp.AppContext.Api.Version.ToString(),
                            Date = DateTime.Now
                        };
                        manifest.Save(manifestFilePath);

                        project.AddFile(manifestFilePath);
                        project.NeedsReload = true;
                        await IdeApp.ProjectOperations.SaveAsync(project);
                    }
                }
                catch (Exception ex)
                {
                    LoggingService.LogInternalError(ex);
                }
            }
        }
    }
}
