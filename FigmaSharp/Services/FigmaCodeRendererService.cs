using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FigmaSharp.Services
{
    public class FigmaCodeRendererService
    {
        protected FigmaFileService figmaFileService;

        public FigmaCodeRendererService(FigmaFileService figmaFileService)
        {
            this.figmaFileService = figmaFileService;
        }

        public string GetCode(FigmaNode node, bool recursively = false)
        {
            var current = figmaFileService.NodesProcessed.FirstOrDefault(s => s.FigmaNode == node);
            if (current == null) return string.Empty;

            var builder = new StringBuilder();
            builder.Append(current.Code);

            //if (child.FigmaNode is IAbsoluteBoundingBox absoluteBounding && node is IAbsoluteBoundingBox nodeAbsoluteBoundingBox && node.FigmaNode is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
            //{
            //    child.View.X = nodeAbsoluteBoundingBox.absoluteBoundingBox.x - parentAbsoluteBoundingBox.absoluteBoundingBox.x;

            //    var diferencia = nodeAbsoluteBoundingBox.absoluteBoundingBox.y - absoluteBounding.absoluteBoundingBox.y;
            //    child.View.Y = nodeAbsoluteBoundingBox.absoluteBoundingBox.height - diferencia;
            //    // currentHeight - absoluteBounding.absoluteBoundingBox.height - (absoluteBounding.absoluteBoundingBox.y - parentAbsoluteBoundingBox.absoluteBoundingBox.y);
            //}


            return builder.ToString ();
        }

        public void Recursively (StringBuilder builder, ProcessedNode node)
        {
            //we start to process all nodes
            var children = figmaFileService.NodesProcessed.Where(s => s.ParentView == node);
            foreach (var child in children)
            {
                if (child.FigmaNode is IAbsoluteBoundingBox absoluteBounding && node is IAbsoluteBoundingBox nodeAbsoluteBoundingBox && node.FigmaNode is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
                {
                    child.View.X = nodeAbsoluteBoundingBox.absoluteBoundingBox.x - parentAbsoluteBoundingBox.absoluteBoundingBox.x;

                    var diferencia = nodeAbsoluteBoundingBox.absoluteBoundingBox.y - absoluteBounding.absoluteBoundingBox.y;
                    child.View.Y = nodeAbsoluteBoundingBox.absoluteBoundingBox.height - diferencia;
                    // currentHeight - absoluteBounding.absoluteBoundingBox.height - (absoluteBounding.absoluteBoundingBox.y - parentAbsoluteBoundingBox.absoluteBoundingBox.y);
                }
            }

            //if (children.Any())
            //{

            //    Recursively(processedNode, id, children);
            //}
        }
    }
}
