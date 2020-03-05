using System.Linq;
using FigmaSharp.Models;
using FigmaSharp.Views;

namespace FigmaSharp.Services
{
    public class StoryboardLayoutManager
    {
        public StoryboardLayoutManager()
        {
           
        }

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

            //we need correct current initial positioning
            var rectangle = orderedNodes.Select(s => s.FigmaNode).GetBoundRectangle();

            //We want know the background color of the figma camvas and apply to our scrollview
            var canvas = rendererService.FileProvider.Nodes
                .OfType<FigmaCanvas>()
                .FirstOrDefault();
            if (canvas != null)
            {
                contentView.BackgroundColor = canvas.backgroundColor;
                if (contentView.Parent is IScrollView scrollview)
                {
                    scrollview.BackgroundColor = canvas.backgroundColor;
                    scrollview.SetContentSize(rectangle.Width + VerticalMargins * 2, rectangle.Height + HorizontalMargins * 2);
                }
            }
        
            foreach (var node in orderedNodes) {
                if (node.FigmaNode is IAbsoluteBoundingBox box)
                {
                    node.View.SetPosition(-rectangle.X + box.absoluteBoundingBox.X + VerticalMargins, -rectangle.Y + box.absoluteBoundingBox.Y + HorizontalMargins);
                }
            }
        }
    }
}
