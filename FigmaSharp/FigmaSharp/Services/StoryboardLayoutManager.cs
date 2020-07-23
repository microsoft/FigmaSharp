using System.Linq;
using FigmaSharp.Models;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Views;

namespace FigmaSharp.Services
{
    public class StoryboardLayoutManager
    {
        public bool UsesConstraints { get; set; }
        public int HorizontalMargins { get; set; } = 64;
        public int VerticalMargins { get; set; } = 64;

        public void Run(IView contentView, ViewRenderService rendererService)
        {
            var mainNodes = rendererService.NodesProcessed.Where(s => s.Node.Parent is FigmaCanvas)
                .ToArray ();
            Run(mainNodes, contentView, rendererService);
        }

        public void Run (ViewNode[] mainViews, IView contentView, ViewRenderService rendererService)
        {
            var orderedNodes = mainViews
        .OrderBy(s => ((IAbsoluteBoundingBox)s.Node).absoluteBoundingBox.Left)
        .ThenBy(s => ((IAbsoluteBoundingBox)s.Node).absoluteBoundingBox.Top)
        .ToArray();

            //We want know the background color of the figma camvas and apply to our scrollview
            var firstNode = orderedNodes.FirstOrDefault();
            if (firstNode == null)
                return;

            var canvas = firstNode.ParentView?.Node as FigmaCanvas;
            if (canvas != null) {
                contentView.BackgroundColor = canvas.backgroundColor;
                if (contentView.Parent is IScrollView scrollview) {
                    scrollview.BackgroundColor = canvas.backgroundColor;

                    //we need correct current initial positioning
                    var rectangle = orderedNodes
                        .Select(s => s.Node)
                        .GetBoundRectangle();
                    scrollview.SetContentSize(rectangle.Width + VerticalMargins * 2, rectangle.Height + HorizontalMargins * 2);
                }
            }

            if (UsesConstraints) {
                foreach (var node in orderedNodes)
                {
                    Converters.NodeConverter converter = rendererService.GetConverter(node.Node);
                    rendererService.PropertySetter.Configure(PropertyNames.Constraints,
                        node, node.ParentView, converter, rendererService);
                }
            } else {
                var rectangle = orderedNodes
                       .Select(s => s.Node)
                       .GetBoundRectangle();

                foreach (var node in orderedNodes)
                {
                    //we need correct current initial positioning
                    if (node.Node is IAbsoluteBoundingBox box)
                    {
                        node.View.SetPosition(-rectangle.X + box.absoluteBoundingBox.X + VerticalMargins, -rectangle.Y + box.absoluteBoundingBox.Y + HorizontalMargins);
                    }
                } 
            }
        }
    }
}
