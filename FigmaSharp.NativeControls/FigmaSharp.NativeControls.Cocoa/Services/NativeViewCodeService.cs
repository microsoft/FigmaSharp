using System;
using FigmaSharp;
using FigmaSharp.Models;
using FigmaSharp.Services;
using System.Linq;
using FigmaSharp.NativeControls.Cocoa;
using FigmaSharp.NativeControls;

namespace FigmaSharp.Services
{
	public class NativeViewCodeService : FigmaCodeRendererService
	{
		public NativeViewCodeService (IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters, FigmaCodePropertyConverterBase codePropertyConverter) : base (figmaProvider, figmaViewConverters, codePropertyConverter)
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
