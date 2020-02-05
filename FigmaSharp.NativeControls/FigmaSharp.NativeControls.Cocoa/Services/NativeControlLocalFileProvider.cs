using System;
using FigmaSharp.Models;
using FigmaSharp.NativeControls;
using System.Linq;

namespace FigmaSharp.Services
{
	public class NativeControlRemoteFileProvider : FigmaRemoteFileProvider
	{
		public NativeControlRemoteFileProvider ()
		{
		}

		#region Rendering

		public override bool IsNodeSkipped (FigmaCodeNode node)
		{
			if (node.Node.IsDialogParentContainer ())
				return true;
			if (node.Node.IsWindowContent ())
				return true;
			return false;
		}

		public override bool IsMainViewContainer (FigmaCodeNode node)
		{
			return node.Node.IsWindowContent ();
		}

		public override FigmaNode[] GetChildrenToRender (FigmaCodeNode node)
		{
			if (node.Node is FigmaBoolean) {
				return new FigmaNode[0];
			}

			if (node.Node.IsDialogParentContainer (NativeControlType.WindowStandard)) {
				if (node.Node is IFigmaNodeContainer nodeContainer) {
					var item = nodeContainer.children.FirstOrDefault (s => s.IsNodeWindowContent ());
					if (item != null && item is IFigmaNodeContainer children) {
						return children.children;
					}
				}
			}
			return base.GetChildrenToRender (node);
		}

		public override bool HasChildrenToRender (FigmaCodeNode node)
		{
			if (node.Node.IsDialogParentContainer ()) {
				return true;
			}
			return base.HasChildrenToRender (node);
		}

		#endregion

	}

	public class NativeControlLocalFileProvider : FigmaLocalFileProvider
	{
		public NativeControlLocalFileProvider (string resourcesDirectory) : base (resourcesDirectory)
		{
		}

		#region Rendering

		public override bool IsNodeSkipped (FigmaCodeNode node)
		{
			if (node.Node.IsDialogParentContainer ())
				return true;
			if (node.Node.IsWindowContent ())
				return true;
			return false;
		}

		public override bool IsMainViewContainer (FigmaCodeNode node)
		{
			return node.Node.IsWindowContent ();
		}

		public override FigmaNode[] GetChildrenToRender (FigmaCodeNode node)
		{
			if (node.Node is FigmaBoolean) {
				return new FigmaNode[0];
			}

			if (node.Node.IsDialogParentContainer (NativeControlType.WindowStandard)) {
				if (node.Node is IFigmaNodeContainer nodeContainer) {
					var item = nodeContainer.children.FirstOrDefault (s => s.IsNodeWindowContent ());
					if (item != null && item is IFigmaNodeContainer children) {
						return children.children;
					}
				}
			}
			return base.GetChildrenToRender (node);
		}

		public override bool HasChildrenToRender (FigmaCodeNode node)
		{
			if (node.Node.IsDialogParentContainer ()) {
				return true;
			}
			return base.HasChildrenToRender (node);
		}

		#endregion


	}
}
