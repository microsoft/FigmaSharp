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

using System.Threading.Tasks;
using FigmaSharp;
using FigmaSharp.Controls.Cocoa.Services;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;

namespace MonoDevelop.Figma.Commands
{
    class RegenerateFigmaDocumentCommandHandler : FigmaCommandHandler
    {
        protected override void OnUpdate(CommandInfo info)
        {
            if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder)
            {
                if (currentFolder.IsDocumentDirectoryBundle())
                {
                    info.Text = "Regenerate from Figma Document";
                    info.Visible = info.Enabled = true;
                    return;
                }
            }
            info.Visible = info.Enabled = false;
        }

        protected async override void OnRun()
        {
            if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder && currentFolder.IsDocumentDirectoryBundle())
            {
                var bundle = FigmaBundle.FromDirectoryPath(currentFolder.Path.FullPath);
                if (bundle == null)
                {
                    return;
                }
                var includeImages = true;

                IdeApp.Workbench.StatusBar.AutoPulse = true;
                IdeApp.Workbench.StatusBar.BeginProgress($"Regenerating ‘{bundle.Manifest.DocumentTitle}’…");

                await Task.Run(() =>
                {
                    //we need to ask to figma server to get nodes as demmand
                    var fileProvider = new ControlFileNodeProvider(bundle.ResourcesDirectoryPath);
                    fileProvider.Load(bundle.DocumentFilePath);
                    bundle.Reload();

                    var codeRendererService = new NativeViewCodeService(fileProvider);
                    bundle.SaveAll(includeImages, fileProvider);
                });

                IdeApp.Workbench.StatusBar.EndProgress();
                IdeApp.Workbench.StatusBar.AutoPulse = false;

                await currentFolder.Project.IncludeBundleAsync(bundle, includeImages)
                    .ConfigureAwait(true);
            }
        }
    }
}
