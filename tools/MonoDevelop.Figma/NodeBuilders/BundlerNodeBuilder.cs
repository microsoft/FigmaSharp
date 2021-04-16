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

		public override void BuildNode (ITreeBuilder builder, object dataObject, NodeInfo nodeInfo)
		{
			if (dataObject is Projects.ProjectFile file)
			{
				if (file.IsFigmaDesignerFile () || file.IsFigmaCSFile())
				{
					nodeInfo.OverlayBottomLeft = figmaOverlay;
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
					Task.Run(() => {
						var query = new FigmaFileVersionQuery(bundle.FileId);
						var figmaFileVersions = FigmaSharp.AppContext.Api.GetFileVersions(query).versions;
						return figmaFileVersions
							.GroupByCreatedAt()
							.OrderByDescending (s => s.created_at)
							.FirstOrDefault ();
					}).ContinueWith (s => {
						if (s.Result != null && s.Result.id != bundle.Version.id) {
							Runtime.RunInMainThread(() => {
								nodeInfo.StatusIcon = Context.GetIcon(packageUpdateIcon);

								if (s.Result.IsNamed)
									nodeInfo.StatusMessage = $"Update available: {s.Result.label}";
								else
									nodeInfo.StatusMessage = $"Update available: {s.Result.created_at.ToString("g")}";
							});
						}
					});
					return;
				}

				if (pr.IsFigmaDirectory ()) {
					nodeInfo.Label = FigmaFolderLabel;
					nodeInfo.ClosedIcon = nodeInfo.Icon = Context.GetIcon (Resources.Icons.FigmaPad);
					return;
				}
			}
		}
		//	public override int GetSortIndex (ITreeNavigator node)
		//	{
		//		return -200;
		//	}
	}
}
