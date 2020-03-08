using System;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;
using MonoDevelop.Projects;

namespace MonoDevelop.Figma
{
	public class FigmaReferencesProjectsNode
	{
		DotNetProject project;

		public FigmaReferencesProjectsNode()
		{
			//this.project = project;
		}
	}

	sealed class FigmaFolderNodeBuilder : TypeNodeBuilder
	{
		public override Type NodeDataType
		{
			get
			{
				return typeof(FigmaReferencesProjectsNode);
			}
		}

		public override string GetNodeName(ITreeNavigator thisNode, object dataObject)
		{
			return "FigmaReferenceNode";
		}

		public override void BuildNode(ITreeBuilder treeBuilder, object dataObject, NodeInfo nodeInfo)
		{
			nodeInfo.Label = "Figma References";
			nodeInfo.ClosedIcon = nodeInfo.Icon = Context.GetIcon(Resources.Icons.FigmaPad);
		}
	}

	public class CustomFigmaBundlerNodeBuilder : NodeBuilderExtension
	{
		//const string BundlesFolderLabel = "Bundles";
		const string FigmaFolderLabel = "Figma";

		public override bool CanBuildNode (Type dataType)
		{
			return typeof(ProjectReferenceCollection).IsAssignableFrom(dataType);
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
			builder.AddChild(new FigmaReferencesProjectsNode ());

			//ProjectPackagesFolderNode packagesFolder = GetPackagesFolderNode(builder);
			//if (packagesFolder != null)
			//{
			//	return packagesFolder.AnyPackageReferences();
			//}
			//return false;

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
