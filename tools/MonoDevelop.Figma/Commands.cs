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
using System.Threading.Tasks;

using FigmaSharp;
using FigmaSharp.NativeControls.Cocoa;
using FigmaSharp.Services;

using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using MonoDevelop.Projects;

namespace MonoDevelop.Figma.Commands
{
	class CreateEmptyManifesCommandHandler : FigmaCommandHandler
	{
		protected override void OnUpdate (CommandInfo info)
		{
			info.Visible = info.Enabled = IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder &&
					currentFolder.IsDocumentDirectoryBundle () &&
					!File.Exists (Path.Combine (currentFolder.Path.FullPath, FigmaBundle.ManifestFileName));
		}

		protected async override void OnRun ()
		{
			if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder) {
				try {
					var manifestFilePath = Path.Combine (currentFolder.Path.FullPath, FigmaBundle.ManifestFileName);

					if (!File.Exists (manifestFilePath)) {
						var project = currentFolder.Project;

						var manifest = new FigmaManifest () {
							Namespace = project.GetDefaultFigmaNamespace (),
							DocumentVersion = "0",
							ApiVersion = FigmaSharp.AppContext.Current.Version,
							RemoteApiVersion = FigmaSharp.AppContext.Api.Version.ToString (),
							Date = DateTime.Now
						};
						manifest.Save (manifestFilePath);
					
						project.AddFile (manifestFilePath);
						project.NeedsReload = true;
						await IdeApp.ProjectOperations.SaveAsync (project);
					}
				} catch (Exception ex) {
					LoggingService.LogInternalError (ex);
				}
			}
		}
	}

	class OpenLocalFigmaFileCommandHandler : FigmaCommandHandler
	{
		protected override void OnUpdate (CommandInfo info)
		{
			try {
				if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder &&
					currentFolder.IsDocumentDirectoryBundle ()
					) {

					var manifestFilePath = Path.Combine (currentFolder.Path.FullPath, FigmaBundle.DocumentFileName);

					if (File.Exists (manifestFilePath)) {
						info.Visible = info.Enabled = true;
						return;
					}
				};
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}
			info.Visible = info.Enabled = false;
		}

		protected override void OnRun ()
		{
			if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder) {
				try {
					var documentFilePath = new FilePath (Path.Combine (currentFolder.Path.FullPath, FigmaBundle.DocumentFileName));

					IdeApp.OpenFiles (new[] { new Ide.Gui.FileOpenInformation (documentFilePath)});
				} catch (Exception ex) {
					LoggingService.LogInternalError (ex);
				}
			}
		}
	}

	class OpenRemoteFigmaFileCommandHandler : FigmaCommandHandler
	{
		const string figmaUrl = "https://www.figma.com/file/{0}/";

		protected override void OnUpdate (CommandInfo info)
		{
			try {
				if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder &&
					currentFolder.IsDocumentDirectoryBundle ()
					) {

					var manifestFilePath = Path.Combine (currentFolder.Path.FullPath, FigmaBundle.ManifestFileName);

					if (!File.Exists (manifestFilePath)) {
						throw new FileNotFoundException (manifestFilePath);
					}

					var manifest = FigmaManifest.FromFilePath (manifestFilePath);
					if (manifest.FileId != null) {
						info.Visible = info.Enabled = true;
						return;
					}
				};
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}
			info.Visible = info.Enabled = false;
		}

		protected override void OnRun ()
		{
			if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder) {
			try {
					var manifestFilePath = Path.Combine (currentFolder.Path.FullPath, FigmaBundle.ManifestFileName);
					var manifest = FigmaManifest.FromFilePath (manifestFilePath);
					if (manifest.FileId != null) {
						IdeServices.DesktopService.ShowUrl (string.Format (figmaUrl, manifest.FileId));
						return;
					}
				} catch (Exception ex) {
					LoggingService.LogInternalError (ex);
				}
			}
		}
	}

	[Obsolete ("No longer used")]
	class FigmaNewFileViewCommandHandler : FigmaCommandHandler
	{
		protected override void OnUpdate (CommandInfo info)
		{
			info.Visible = info.Enabled = false;
			//info.Visible = info.Enabled =IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder folder
			//	&& folder.IsDocumentDirectoryBundle ();
		}

		const string figmaNodeName = "bundleFigmaDocument";

		protected async override void OnRun ()
		{
			if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder && currentFolder.IsDocumentDirectoryBundle ()) {

				var bundle = FigmaBundle.FromDirectoryPath (currentFolder.Path.FullPath);
				if (bundle != null) {

					var currentProject = currentFolder.Project;

					var fileProvider = new FigmaLocalFileProvider (bundle.ResourcesDirectoryPath);
					fileProvider.Load (bundle.DocumentFilePath);

					var converters = NativeControlsContext.Current.GetConverters ();
					var codePropertyConverter = NativeControlsContext.Current.GetCodePropertyConverter ();
					var codeRendererService = new NativeViewCodeService (fileProvider, converters, codePropertyConverter);

					var fignaNode = fileProvider.FindByName (figmaNodeName);
					var figmaBundleView = new FigmaBundleView (bundle, "test", fignaNode);

					figmaBundleView.Generate (codeRendererService);

					if (!currentProject.PathExistsInProject (bundle.ViewsDirectoryPath)) {
						currentProject.AddDirectory (FileService.AbsoluteToRelativePath (currentProject.BaseDirectory, bundle.ViewsDirectoryPath));
					}

					var designerProjectFile = currentProject.AddFile (figmaBundleView.PartialDesignerClassFilePath);
					var csProjectFile = currentProject.AddFile (figmaBundleView.PublicCsClassFilePath);
					designerProjectFile.DependsOn = csProjectFile.FilePath;

					currentProject.NeedsReload = true;
					await IdeApp.ProjectOperations.SaveAsync (currentProject);
				}
			}
		}
	}

	class FigmaUpdateViewCommandHandler : FigmaCommandHandler
	{
		protected override void OnUpdate(CommandInfo info)
		{
			
			if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder &&
				currentFolder.IsDocumentDirectoryBundle())
			{
				var manifestFilePath = Path.Combine(currentFolder.Path.FullPath, FigmaBundle.DocumentFileName);
				if (File.Exists(manifestFilePath)) {
					info.Visible = info.Enabled = true;
					return;
				}
			};
		
			info.Visible = info.Enabled = false;
		}

		protected override void OnRun()
		{
			var currentFolder = IdeApp.ProjectOperations.CurrentSelectedItem as ProjectFolder;
			if (currentFolder == null)
				return;

			var bundle = FigmaBundle.FromDirectoryPath(currentFolder.Path.FullPath);
			var figmaBundleWindow = new Packages.PackageUpdateWindow();
			figmaBundleWindow.Load (bundle, currentFolder.Project);

			var currentIdeWindow = Components.Mac.GtkMacInterop.GetNSWindow(IdeApp.Workbench.RootWindow);
			currentIdeWindow.AddChildWindow(figmaBundleWindow, AppKit.NSWindowOrderingMode.Above);
			MessageService.PlaceDialog(figmaBundleWindow, MessageService.RootWindow);
			IdeServices.DesktopService.FocusWindow(figmaBundleWindow);
		}
	}

	class FigmaNewBundlerCommandHandler : FigmaCommandHandler
	{
		protected override void OnUpdate (CommandInfo info)
		{
			info.Visible = info.Enabled = IdeApp.ProjectOperations.CurrentSelectedItem is Project ||
				(
					IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder folder
					&& folder.IsFigmaDirectory ()
				);
		}

		protected override void OnRun ()
		{
			//when window is closed we need to create all the stuff
			Project currentProject = null;
			if (IdeApp.ProjectOperations.CurrentSelectedItem is Project project) {
				currentProject = project;
			} else if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder projectFolder
				  && projectFolder.IsFigmaDirectory()) {
				currentProject = projectFolder.Project;
			}

			if (currentProject == null) {
				return;
			}

			var figmaBundleWindow = new Packages.FigmaPackageWindow(currentProject);
		
			var currentIdeWindow = Components.Mac.GtkMacInterop.GetNSWindow (IdeApp.Workbench.RootWindow);
			currentIdeWindow.AddChildWindow (figmaBundleWindow, AppKit.NSWindowOrderingMode.Above);
			MessageService.PlaceDialog (figmaBundleWindow, MessageService.RootWindow);
			IdeServices.DesktopService.FocusWindow (figmaBundleWindow);
		}
	}

	class RegenerateFigmaDocumentCommandHandler : FigmaCommandHandler
	{
		protected override void OnUpdate (CommandInfo info)
		{
			if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder) {
				if (currentFolder.IsDocumentDirectoryBundle ()) {
					info.Text = "Regenerate from Figma Document";
					info.Visible = info.Enabled = true;
					return;
				}
			}
			info.Visible = info.Enabled = false;
		}

		protected async override void OnRun ()
		{
			if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder && currentFolder.IsDocumentDirectoryBundle ()) {
				var bundle = FigmaBundle.FromDirectoryPath(currentFolder.Path.FullPath);
				if (bundle == null) {
					return;
				}
				var includeImages = true;

				IdeApp.Workbench.StatusBar.AutoPulse = true;
				IdeApp.Workbench.StatusBar.BeginProgress($"Regenerating ‘{bundle.Manifest.DocumentTitle}’…");

				await Task.Run(() => {
					//we need to ask to figma server to get nodes as demmand
					var fileProvider = new FigmaLocalFileProvider(bundle.ResourcesDirectoryPath);
					fileProvider.Load(bundle.DocumentFilePath);
					bundle.Reload(fileProvider);

					var codeRendererService = new NativeViewCodeService(fileProvider);
					bundle.SaveAll(codeRendererService, includeImages, false);
				});

				IdeApp.Workbench.StatusBar.EndProgress ();
				IdeApp.Workbench.StatusBar.AutoPulse = false;

				await currentFolder.Project.IncludeBundle (bundle, includeImages)
					.ConfigureAwait (true);
			}
		}
	}

	abstract class FigmaCommandHandler : CommandHandler
	{
		protected override void Update (CommandInfo info)
		{
			OnUpdate (info);

			if (!FigmaSharp.AppContext.Current.IsApiConfigured) {
				info.Enabled = false;
				return;
			}
		}

		protected override void Run ()
		{
			OnRun ();
		}

		protected abstract void OnUpdate (CommandInfo info);
		protected abstract void OnRun ();
	}

	public class FigmaInitCommand : CommandHandler
	{
		protected override void Run ()
		{
			Resources.Init ();
			NativeControlsApplication.Init (FigmaRuntime.Token);
		}
	}
}
