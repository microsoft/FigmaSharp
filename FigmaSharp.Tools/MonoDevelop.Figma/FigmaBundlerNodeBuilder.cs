using System;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Components;
using MonoDevelop.Ide.Gui.Pads.ProjectPad;

namespace MonoDevelop.Figma
{
	public class CustomFigmaBundlerNodeBuilder : NodeBuilderExtension
	{
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
				if (pr.IsFigmaBundleDirectory ()) {
					nodeInfo.Label = "Figma Bundles";
					nodeInfo.Icon = Context.GetIcon (Stock.AssetsFolder);
					nodeInfo.ClosedIcon = Context.GetIcon (Stock.AssetsFolder);
					return;
				}

				if (pr.IsDocumentDirectoryBundle ()) {
					nodeInfo.Label = pr.Path.FileNameWithoutExtension;
					nodeInfo.Icon = Context.GetIcon ("md-reference-package");
					nodeInfo.ClosedIcon = Context.GetIcon ("md-reference-package");
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
