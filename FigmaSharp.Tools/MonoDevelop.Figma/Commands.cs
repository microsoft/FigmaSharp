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
using System.Linq;
using System;
using System.IO;
using CoreGraphics;
using FigmaSharp;
using FigmaSharp.Cocoa;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;
using MonoDevelop.Components.Commands;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using MonoDevelop.Projects;
using System.Threading.Tasks;
using FigmaSharp.Cocoa.Converters;

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
						var manifest = new FigmaManifest () {
							DocumentVersion = 0,
							ApiVersion = FigmaSharp.AppContext.Current.Version,
							RemoteApiVersion = FigmaSharp.AppContext.Current.RemoteApiVersion,
							Date = DateTime.Now
						};
						manifest.Save (manifestFilePath);
						var project = currentFolder.Project;
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
					if (manifest.DocumentUrl != null) {
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
					if (manifest.DocumentUrl != null) {
						IdeServices.DesktopService.ShowUrl (string.Format (figmaUrl, manifest.DocumentUrl));
						return;
					}
				} catch (Exception ex) {
					LoggingService.LogInternalError (ex);
				}
			}
		}
	}

	class FigmaNewFileViewCommandHandler : FigmaCommandHandler
	{
		protected override void OnUpdate (CommandInfo info)
		{
			info.Visible = info.Enabled =IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder folder
				&& folder.IsDocumentDirectoryBundle ();
		}

		const string figmaNodeName = "1.0. Bundle Figma Bundle";

		protected async override void OnRun ()
		{
			if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder && currentFolder.IsDocumentDirectoryBundle ()) {

				var bundle = FigmaBundle.FromDirectoryPath (currentFolder.Path.FullPath);
				if (bundle != null) {

					var currentProject = currentFolder.Project;

					var figmaBundleView = new FigmaBundleView (bundle, "test", figmaNodeName);

					var fileProvider = new FigmaLocalFileProvider (bundle.ResourcesDirectoryPath);
					fileProvider.Load (bundle.DocumentFilePath);
					var converters = FigmaSharp.NativeControls.Cocoa.Resources.GetConverters ();
					//var rendererOptions = new FigmaViewRendererServiceOptions () { ScanChildrenFromFigmaInstances = false };

					var addChildConverter = new FigmaCodeAddChildConverter ();
					var positionConverter = new FigmaCodePositionConverter ();
					var codeRendererService = new FigmaCodeRendererService (fileProvider, converters, positionConverter, addChildConverter);
					figmaBundleView.Generate (fileProvider, codeRendererService);

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

	class FigmaNewBundlerCommandHandler : FigmaCommandHandler
	{
		const string AddFigmaDocumentSource = "AddFigmaDocument.figma";
		const string MainWindowName = "1.0. Bundle Figma Bundle";

		protected override void OnUpdate (CommandInfo info)
		{
			info.Visible = info.Enabled = IdeApp.ProjectOperations.CurrentSelectedItem is Project ||
				(
					IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder folder
					&& folder.IsFigmaDirectory ()
				);
		}
	
		protected async override void OnRun ()
		{
			Project currentProject = null;

			if (IdeApp.ProjectOperations.CurrentSelectedItem is Project project) {
				currentProject = project;
			} else if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder projectFolder
					&& projectFolder.IsFigmaDirectory ()) {
				currentProject = projectFolder.Project;
			}

			if (currentProject == null) {
				return;
			}

			var figmaFolder = Path.Combine (currentProject.BaseDirectory.FullPath, FigmaBundle.FigmaDirectoryName);

			if (!Directory.Exists (figmaFolder)) {
				Directory.CreateDirectory (figmaFolder);
				currentProject.AddDirectory (FileService.AbsoluteToRelativePath (currentProject.BaseDirectory, figmaFolder));
			}

			var bundleName = $"MyTestCreated{FigmaBundle.FigmaBundleDirectoryExtension}";
			var fullBundlePath = Path.Combine (figmaFolder, bundleName);

			var bundle = FigmaBundle.Create ("EGTUYgwUC9rpHmm4kJwZQXq4", fullBundlePath);
			bundle.Save ();

			bool includeImages = false; //false for the moment

			bundle.GenerateLocalDocument (includeImages);
			//now we need to add the content
			//bundle
			currentProject.AddDirectory (FileService.AbsoluteToRelativePath (currentProject.BaseDirectory, fullBundlePath));
			//manifest
			currentProject.AddFile (Path.Combine (fullBundlePath, FigmaBundle.ManifestFileName));
			//document
			currentProject.AddFile (Path.Combine (fullBundlePath, FigmaBundle.DocumentFileName));

			if (includeImages) {
				//resources
				var resourcesDirectoryPath = Path.Combine (fullBundlePath, FigmaBundle.ResourcesDirectoryName);
				currentProject.AddDirectory (FileService.AbsoluteToRelativePath (currentProject.BaseDirectory, resourcesDirectoryPath));

				//we add to the project for each resource inside the 
				//foreach (var image in Directory.EnumerateFiles (resourcesDirectoryPath, "*.png")) {
				//	currentProject.AddFile (image);
				//}

				var images = Directory.EnumerateFiles (resourcesDirectoryPath, $"*{FigmaBundle.ImageFormat}").Select (s => new FilePath (s));
				currentProject.AddFiles (images);
			}

			currentProject.NeedsReload = true;
			await IdeApp.ProjectOperations.SaveAsync (currentProject);

			currentProject.AddDirectory (FigmaBundle.FigmaDirectoryName);
			if (!(IdeApp.ProjectOperations.CurrentSelectedItem is Project ||
				(
					IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder folder
					&& folder.IsFigmaDirectory ()
				))) {
				return;
			}
			//var window = new Gtk.Dialog ();
			//window.Modal = true;
			//window.SetSizeRequest (200, 300);
			//window.Title = "Figma Bundle Generator";

			////var resourcesFullPath = Path.Combine (currentProject.BaseDirectory.FullPath, FigmaBundle.ResourcesDirectoryName);

			////var fileFullPath = Path.Combine (currentProject.BaseDirectory.FullPath, AddFigmaDocumentSource);

			//var fileProvider = new FigmaManifestFileProvider (this.GetType ().Assembly, AddFigmaDocumentSource);
			//fileProvider.Load (AddFigmaDocumentSource);

			//var converters = FigmaSharp.NativeControls.Cocoa.Resources.GetConverters ();
			//var rendererService = new FigmaViewRendererService (fileProvider, converters);
			//var rendererOptions = new FigmaViewRendererServiceOptions () { ScanChildrenFromFigmaInstances = false };
			//var view = rendererService.RenderByPath<IView> (rendererOptions, "");

			////var urlTextField = rendererService.Find<TextBox> ("FigmaUrlTextField");
			////var bundleButton = rendererService.FindViewByName<Button> ("BundleButton");
			////var cancelButton = rendererService.FindViewByName<Button> ("CancelButton");

			//var gtkViewHost = new Gtk.GtkNSViewHost (view.NativeObject as AppKit.NSView);
			//if (window.Child is Gtk.VBox vb) {
			//	vb.PackStart (gtkViewHost, true, true, 0);
			//	vb.ShowAll ();
			//}
			//MessageService.ShowCustomDialog (window, IdeApp.Workbench.RootWindow);
		}
	}

	class RegenerateFigmaDocumentCommandHandler : FigmaCommandHandler
	{
		protected override void OnUpdate (CommandInfo info)
		{
			if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder) {
				if (currentFolder.IsDocumentDirectoryBundle ()) {
					var manifestFullFilePath = Path.Combine (currentFolder.Path.FullPath, FigmaBundle.DocumentFileName);
					info.Text = string.Format ("{0} {1}", currentFolder.Project.PathExistsInProject (manifestFullFilePath) ? "Regenerate" : "Generate",  FigmaBundle.DocumentFileName);
					info.Visible = info.Enabled = true;
					return;
				}
			} else if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFile currentFile && currentFile.IsFigmaManifest ()) {
				info.Text = string.Format ("Regenerate {0}", FigmaBundle.DocumentFileName);
				info.Visible = info.Enabled = true;
				return;
			}

			info.Visible = info.Enabled = false;
		}

		protected async override void OnRun ()
		{
			if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFolder currentFolder && currentFolder.IsDocumentDirectoryBundle ()) {
				await RegenerateDocument (currentFolder.Project, currentFolder.Path.FullPath);
			} else if (IdeApp.ProjectOperations.CurrentSelectedItem is ProjectFile currentFile && currentFile.IsFigmaManifest ()) {
				await RegenerateDocument (currentFile.Project, currentFile.FilePath.ParentDirectory.FullPath);
				return;
			}
		}

		async Task RegenerateDocument (Project project, string bundleDirectoryPath)
		{
			var manifest = FigmaBundle.FromDirectoryPath (bundleDirectoryPath);
			if (manifest == null) {
				return;
			}
			manifest.GenerateLocalDocument (false);

			var documentFilePath = manifest.DocumentFilePath;
			//we need to add the file to the project in case is not added
			if (!project.PathExistsInProject (documentFilePath)) {
				project.AddFile (documentFilePath);
				project.NeedsReload = true;
				await IdeApp.ProjectOperations.SaveAsync (project);
			}
		}
	}

	abstract class FigmaCommandHandler : CommandHandler
	{
		protected override void Update (CommandInfo info)
		{
			OnUpdate (info);

			if (!Resources.IsFigmaEnabled) {
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
			FigmaApplication.Init (FigmaRuntime.Token);
		}
	}
}
