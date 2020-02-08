using System;
using FigmaSharp.Models;
using FigmaSharp.Cocoa;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.NativeControls;
using FigmaSharp.NativeControls.Cocoa;
using FigmaSharp;

namespace FigmaSharp.Services
{
	public class NativeViewRenderingService : FigmaViewRendererService
	{
		public NativeViewRenderingService (IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters) : base (figmaProvider, figmaViewConverters)
		{
		}

		public void RenderInWindow (IWindow mainWindow, FigmaNode node, FigmaViewRendererServiceOptions options)
		{
			if (node is IAbsoluteBoundingBox bounNode) {
				mainWindow.Size = new Size (bounNode.absoluteBoundingBox.Width, bounNode.absoluteBoundingBox.Height);
			}

			ProcessFromNode (node, null, options);
			var processedNode = FindProcessedNodeById (node.id);
			Recursively (processedNode);
			mainWindow.Content = processedNode.View;
		}

		public override bool ProcessesImageFromNode (FigmaNode node)
		{
 			return node.IsImageNode () || node.IsFigmaImageViewNode ();
		}

		protected override bool NodeScansChildren (FigmaNode currentNode, CustomViewConverter converter, FigmaViewRendererServiceOptions options)
		{
			if (currentNode.IsFigmaImageViewNode ())
				return false;

			return base.NodeScansChildren (currentNode, converter, options);
		}

		protected override bool SkipsNode (FigmaNode currentNode, ProcessedNode parent, FigmaViewRendererServiceOptions options)
		{
			if (currentNode.IsDialog ()) {
				return true;
			}
			return false;
		}
	}
}
