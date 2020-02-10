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
using FigmaSharp.NativeControls.Cocoa;

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
							DocumentVersion = "0",
							ApiVersion = FigmaSharp.AppContext.Current.Version,
							RemoteApiVersion = FigmaSharp.AppContext.Api.Version.ToString (),
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
			var figmaBundleWindow = new FigmaBundles.bundleFigmaDocument ();
			figmaBundleWindow.BundleCreated += async (s, e) => {
				var window = (FigmaBundles.bundleFigmaDocument)s;

				var includeImages = true;
				await GenerateBundle (window.FileId, window.SelectedFileVersion, includeImages);
				window.Close ();
			};

			var currentIdeWindow = Components.Mac.GtkMacInterop.GetNSWindow (IdeApp.Workbench.RootWindow);
			currentIdeWindow.AddChildWindow (figmaBundleWindow, AppKit.NSWindowOrderingMode.Above);
			MessageService.PlaceDialog (figmaBundleWindow, MessageService.RootWindow);
			IdeServices.DesktopService.FocusWindow (figmaBundleWindow);
		}

		async Task GenerateBundle (string fileId, FigmaSharp.Models.FigmaFileVersion version, bool includeImages)
		{
			//when window is closed we need to create all the stuff
		
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

			//we need to ask to figma server to get nodes as demmand
			var fileProvider = new FigmaRemoteFileProvider () { File = fileId };
			fileProvider.Load (fileId);

			//var bundleName = $"MyTestCreated{FigmaBundle.FigmaBundleDirectoryExtension}";
			var projectBundle = CreateBundleFromProject (currentProject, fileId, version, fileProvider, includeImages: includeImages);

			//set namespace
			if (currentProject is DotNetProject dotNetProject) {
				projectBundle.Namespace = $"{dotNetProject.DefaultNamespace}.FigmaBundles" ;
			} else {
				projectBundle.Namespace = $"{GetType ().Namespace}.FigmaBundles"; ;
			}

			//to generate all layers we need a code renderer
			var converters = NativeControlsContext.Current.GetConverters (true);
			var codePropertyConverter = NativeControlsContext.Current.GetCodePropertyConverter ();
			var codeRendererService = new NativeViewCodeService (fileProvider, converters, codePropertyConverter);

			projectBundle.Save ();
			projectBundle.SaveLocalDocument (false);
			projectBundle.SaveViews (codeRendererService);

			//now we need to add to Monodevelop all the stuff
			await IncludeBundleInProject (currentProject, projectBundle).ConfigureAwait (true);
		}

		FigmaBundle CreateBundleFromProject (Project project, string fileId, FigmaSharp.Models.FigmaFileVersion version, IFigmaFileProvider fileProvider, bool includeImages)
		{
			var figmaFolder = Path.Combine (project.BaseDirectory.FullPath, FigmaBundle.FigmaDirectoryName);

			if (!Directory.Exists (figmaFolder)) {
				Directory.CreateDirectory (figmaFolder);
			}

			//Bundle generation - We generate an empty bundle and store in the folder
			var fullBundlePath = Path.Combine (figmaFolder, fileId);

			var bundle = FigmaBundle.Empty (fileId, version, fullBundlePath);

			//generate .figma file
			bundle.LoadLocalDocument ();

			//this reads all the main layers ready and fills our Views models
			bundle.LoadRemoteMainLayers (fileProvider);

			return bundle;

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

		async Task IncludeBundleInProject (Project currentProject, FigmaBundle bundle, bool includeImages = false)
		{
			var figmaFolder = Path.Combine (currentProject.BaseDirectory.FullPath, FigmaBundle.FigmaDirectoryName);
			if (!currentProject.PathExistsInProject (figmaFolder)) {
				//we add figma folder in the case doesn't exists
				currentProject.AddDirectory (FileService.AbsoluteToRelativePath (currentProject.BaseDirectory, figmaFolder));
			}

			//now we need to add the content
			//bundle
			var fullBundlePath = bundle.DirectoryPath;
			if (!currentProject.PathExistsInProject (fullBundlePath)) {
				currentProject.AddDirectory (FileService.AbsoluteToRelativePath (currentProject.BaseDirectory, fullBundlePath));
			}

			//manifest
			var manifestFullDirectoryPath = bundle.ManifestFilePath;
			if (!currentProject.PathExistsInProject (manifestFullDirectoryPath))
				currentProject.AddFile (manifestFullDirectoryPath);

			
			//document
			var documentFullDirectoryPath = bundle.DocumentFilePath;
			if (!currentProject.PathExistsInProject (documentFullDirectoryPath))
				currentProject.AddFile (documentFullDirectoryPath);

			//TODO: images are not enabled by now
			if (includeImages) {
				//resources
				var resourcesDirectoryPath = bundle.ResourcesDirectoryPath;
				currentProject.AddDirectory (FileService.AbsoluteToRelativePath (currentProject.BaseDirectory, resourcesDirectoryPath));

				//we add to the project for each resource inside the 
				//foreach (var image in Directory.EnumerateFiles (resourcesDirectoryPath, "*.png")) {
				//	currentProject.AddFile (image);
				//}

				var images = Directory.EnumerateFiles (resourcesDirectoryPath, $"*{FigmaBundle.ImageFormat}").Select (s => new FilePath (s));

				foreach (var item in images) {
					if (!currentProject.PathExistsInProject (item)) {
						currentProject.AddFile (item);
					}
				}
			}

			//files
			var viewsDirectoryPath = bundle.ViewsDirectoryPath;
			if (!currentProject.PathExistsInProject (viewsDirectoryPath)) {
				currentProject.AddDirectory (FileService.AbsoluteToRelativePath (currentProject.BaseDirectory, viewsDirectoryPath));
			}

			foreach (var path in Directory.EnumerateFiles (viewsDirectoryPath)) {
				if (!currentProject.PathExistsInProject (path)) {
					currentProject.AddFile (path);
				}
			}

			currentProject.NeedsReload = true;
			await IdeApp.ProjectOperations.SaveAsync (currentProject);

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

			manifest.LoadLocalDocument ();
			manifest.SaveLocalDocument (false);

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
