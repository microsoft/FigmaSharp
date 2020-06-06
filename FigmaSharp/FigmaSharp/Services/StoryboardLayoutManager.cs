using System.Linq;
using FigmaSharp.Models;
using FigmaSharp.Views;

namespace FigmaSharp.Services
{
    public class StoryboardLayoutManager
    {
        public bool UsesConstraints { get; set; }
        public int HorizontalMargins { get; set; } = 64;
        public int VerticalMargins { get; set; } = 64;

        public void Run(IView contentView, FigmaViewRendererService rendererService)
        {
            var mainNodes = rendererService.NodesProcessed.Where(s => s.FigmaNode.Parent is FigmaCanvas)
                .ToArray ();
            Run(mainNodes, contentView, rendererService);
        }

        public void Run (ProcessedNode[] mainViews, IView contentView, FigmaViewRendererService rendererService)
        {
            var orderedNodes = mainViews
        .OrderBy(s => ((IAbsoluteBoundingBox)s.FigmaNode).absoluteBoundingBox.Left)
        .ThenBy(s => ((IAbsoluteBoundingBox)s.FigmaNode).absoluteBoundingBox.Top)
        .ToArray();

            //We want know the background color of the figma camvas and apply to our scrollview
            var firstNode = orderedNodes.FirstOrDefault();
            if (firstNode == null)
                return;

            var canvas = firstNode.ParentView?.FigmaNode as FigmaCanvas;
            if (canvas != null) {
                contentView.BackgroundColor = canvas.backgroundColor;
                if (contentView.Parent is IScrollView scrollview) {
                    scrollview.BackgroundColor = canvas.backgroundColor;

                    //we need correct current initial positioning
                    var rectangle = orderedNodes
                        .Select(s => s.FigmaNode)
                        .GetBoundRectangle();
                    scrollview.SetContentSize(rectangle.Width + VerticalMargins * 2, rectangle.Height + HorizontalMargins * 2);
                }
            }

            if (UsesConstraints) {
                foreach (var node in orderedNodes)
                    rendererService.PropertySetter.Configure(PropertyNames.Constraints,
                        node.View, node.FigmaNode, node.ParentView?.View, node.ParentView?.FigmaNode, rendererService);
            } else {
                var rectangle = orderedNodes
                       .Select(s => s.FigmaNode)
                       .GetBoundRectangle();

                foreach (var node in orderedNodes)
                {
                    //we need correct current initial positioning
                    if (node.FigmaNode is IAbsoluteBoundingBox box)
                    {
                        node.View.SetPosition(-rectangle.X + box.absoluteBoundingBox.X + VerticalMargins, -rectangle.Y + box.absoluteBoundingBox.Y + HorizontalMargins);
                    }
                } 
            }
        }
    }
}
