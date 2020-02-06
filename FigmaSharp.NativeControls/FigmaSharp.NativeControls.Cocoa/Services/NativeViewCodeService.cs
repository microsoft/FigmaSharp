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

		internal override bool IsNodeSkipped (FigmaCodeNode node)
		{
			if (node.Node.IsDialogParentContainer ())
				return true;
			if (node.Node.IsWindowContent ())
				return true;
			return false;
		}

		internal override bool IsMainViewContainer (FigmaCodeNode node)
		{
			return node.Node.IsWindowContent ();
		}

		internal override FigmaNode[] GetChildrenToRender (FigmaCodeNode node)
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

		internal override bool HasChildrenToRender (FigmaCodeNode node)
		{
			if (node.Node.IsDialogParentContainer ()) {
				return true;
			}
			return base.HasChildrenToRender (node);
		}

		protected override bool TryGetCodeViewName (FigmaCodeNode node, FigmaCodeNode parent, out string identifier)
		{
			if (node.Node.TryGetNodeCustomName (out identifier)) {
				return true;
			}
			return base.TryGetCodeViewName (node, parent, out identifier);
		}

		#endregion
	}
}
