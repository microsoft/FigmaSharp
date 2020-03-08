using System;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;

namespace MonoDevelop.Figma
{
	public class CustomFigmaBundlerNodeBuilder : NodeBuilderExtension
	{
		//const string BundlesFolderLabel = "Bundles";
		const string FigmaFolderLabel = "Figma";

		public override bool CanBuildNode (Type dataType)
		{
			return typeof (ProjectFolder).IsAssignableFrom (dataType);
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

		public override void BuildNode (ITreeBuilder builder, object dataObject, NodeInfo nodeInfo)
		{
			if (dataObject is ProjectFolder pr) {
				//if (pr.IsFigmaBundleDirectory ()) {
				//	nodeInfo.Label = BundlesFolderLabel;
				//	nodeInfo.ClosedIcon = nodeInfo.Icon = Context.GetIcon (Stock.AssetsFolder);
				//	return;
				//}

				if (pr.IsDocumentDirectoryBundle ()) {
					var bundle = FigmaSharp.FigmaBundle.FromDirectoryPath (pr.Path.FullPath);
					if (bundle != null && bundle.Manifest != null && !string.IsNullOrEmpty (bundle.Manifest.DocumentTitle)) {
						nodeInfo.Label = bundle.Manifest.DocumentTitle;
					} else {
						nodeInfo.Label = pr.Path.FileNameWithoutExtension;
					}
					nodeInfo.ClosedIcon = nodeInfo.Icon = Context.GetIcon (Stock.Package);
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

	internal class FigmaFileViewNodeCommandHandler : NodeCommandHandler
	{

	}
}
