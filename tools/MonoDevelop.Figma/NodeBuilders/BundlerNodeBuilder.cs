using System;
using System.Linq;
using System.Threading.Tasks;

using FigmaSharp;

using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;

namespace MonoDevelop.Figma
{
	public class CustomFigmaBundlerNodeBuilder : NodeBuilderExtension
	{
		//const string BundlesFolderLabel = "Bundles";
		const string FigmaFolderLabel = "Figma Packages";

		public override bool CanBuildNode (Type dataType)
		{
			return typeof (ProjectFolder).IsAssignableFrom (dataType) || typeof(Projects.ProjectFile).IsAssignableFrom(dataType);
		}

		public override Type CommandHandlerType {
			get { return typeof (FigmaFileViewNodeCommandHandler); }
		}

		protected override void Initialize ()
		{
			base.Initialize ();
		}

		public override void Dispose ()
		{
			base.Dispose ();
		}

		public override void BuildChildNodes (ITreeBuilder builder, object dataObject)
		{
			//if (GtkDesignInfo.HasDesignedObjects ((Project)dataObject))
			//	builder.AddChild (new WindowsFolder ((Project)dataObject));
		}

		static IconId packageUpdateIcon = new IconId("md-package-update");
		static Xwt.Drawing.Image figmaOverlay = Xwt.Drawing.Image.FromResource(typeof(CustomFigmaBundlerNodeBuilder).Assembly, "figma-overlay.png");
		static Xwt.Drawing.Image figmaOverlayError = Xwt.Drawing.Image.FromResource(typeof(CustomFigmaBundlerNodeBuilder).Assembly, "figma-error-overlay.png");

		bool HasError (Projects.ProjectFile designerFile, Projects.ProjectFile projectFile)
		{
			if (!(designerFile.DependsOnFile == projectFile && designerFile.Metadata.HasProperty(FigmaFile.FigmaPackageId)
					&& designerFile.Metadata.HasProperty(FigmaFile.FigmaNodeCustomName)))
				return true;

			//get current package id
			var packageId = designerFile.Metadata.GetValue(FigmaFile.FigmaPackageId);

			//the package was removed from any reason
			var found = designerFile.Project.GetFigmaPackages ().Any (s => s.FileId == packageId);
			return !found;
		}

		public override async void BuildNode (ITreeBuilder builder, object dataObject, NodeInfo nodeInfo)
		{
			if (dataObject is Projects.ProjectFile file)
			{
				if (file.IsFigmaDesignerFile ())
				{
					nodeInfo.OverlayBottomLeft = HasError(file, file.DependsOnFile) ? figmaOverlayError : figmaOverlay;
				}
				else if (file.IsFigmaCSFile (out var designerFile))
				{
					nodeInfo.OverlayBottomLeft = HasError(designerFile, file) ? figmaOverlayError : figmaOverlay;
				}
			}
			else if (dataObject is ProjectFolder pr) {

				if (pr.IsDocumentDirectoryBundle ())
				{
					FigmaBundle bundle = null;
					try {
						bundle = FigmaBundle.FromDirectoryPath(pr.Path.FullPath);
					} catch (Exception ex) {
						//if the bundle is removed by any reason whyle updating
						LoggingService.LogError("CustomFigmaFundlerNodeBuilder", ex);
					}

					if (bundle != null && bundle.Manifest != null && !string.IsNullOrEmpty (bundle.Manifest.DocumentTitle)) {
						nodeInfo.Label = bundle.Manifest.DocumentTitle;
					} else {
						nodeInfo.Label = pr.Path.FileNameWithoutExtension;
					}
				
					nodeInfo.ClosedIcon = nodeInfo.Icon = Context.GetIcon (Stock.Package);

                    var query = new FigmaFileVersionQuery(bundle.FileId);

					//TODO: make async
					var figmaFileVersions = await FigmaSharp.AppContext.Api.GetFileVersionsAsync(query);
                    var versions = figmaFileVersions.versions
                                 .GroupByCreatedAt()
                                 .OrderByDescending(s => s.created_at)
                                 .FirstOrDefault();

                    if (versions != null && versions.id != bundle.Version.id)
                    {
                        nodeInfo.StatusIcon = Context.GetIcon(packageUpdateIcon);

                        if (versions.IsNamed)
                            nodeInfo.StatusMessage = $"Update available: {versions.label}";
                        else
                            nodeInfo.StatusMessage = $"Update available: {versions.created_at.ToString("g")}";
                        return;
					}

					if (pr.IsFigmaDirectory ()) {
						nodeInfo.Label = FigmaFolderLabel;
						nodeInfo.ClosedIcon = nodeInfo.Icon = Context.GetIcon (Resources.Icons.FigmaPad);
						return;
					}
				}
			}
		}
		//	public override int GetSortIndex (ITreeNavigator node)
		//	{
		//		return -200;
		//	}
	}
}
