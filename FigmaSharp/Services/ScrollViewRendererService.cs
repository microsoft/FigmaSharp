using System.Collections.Generic;
using FigmaSharp;
using System.Linq;

namespace FigmaSharp.Services
{
    public class ScrollViewRendererService : IRendererService<IScrollViewWrapper>
    {
        public ProcessedNode[] MainViews { get; private set; }

        IEnumerable<ProcessedNode> processedNodes;
        IViewWrapper currentView;

        public void Start (IScrollViewWrapper targetView, IEnumerable<ProcessedNode> processedNodes)
        {
            this.currentView = targetView;
            this.processedNodes = processedNodes;

            var mainNodes = processedNodes.Where(s => s.ParentView == null).ToArray ();
            MainViews = new ProcessedNode[mainNodes.Length];
            for (int i = 0; i < mainNodes.Length; i++)
            {
                MainViews[i] = mainNodes[i];

                var children = processedNodes.Where(s => s.ParentView == mainNodes[i]);
                if (children.Any())
                {
                    ProcessNodeSubviews(mainNodes[i], children);
                }
            }

            targetView.ClearSubviews();

            //Alignment 
            const int Margin = 20;

            float currentX = Margin;
            foreach (var processedNode in MainViews)
            {
                var view = processedNode.View;

                targetView.AddChild(view);

                view.X = currentX;
                view.Y = Margin;
                currentX += view.Width + Margin;
            }
        }

        void ProcessNodeSubviews(ProcessedNode parentProcessedNode, IEnumerable<ProcessedNode> nodes)
        {
            //we start to process all nodes
            foreach (var processedNode in nodes)
            {
                parentProcessedNode.View.AddChild(processedNode.View);

                if (processedNode.FigmaNode is IAbsoluteBoundingBox absoluteBounding && parentProcessedNode.FigmaNode is IAbsoluteBoundingBox parentAbsoluteBoundingBox) {
                    processedNode.View.X = absoluteBounding.absoluteBoundingBox.x - parentAbsoluteBoundingBox.absoluteBoundingBox.x;
                    processedNode.View.Y = absoluteBounding.absoluteBoundingBox.y - parentAbsoluteBoundingBox.absoluteBoundingBox.y;
                }

                var children = processedNodes.Where(s => s.ParentView == processedNode);
                if (children.Any ())
                {
                    ProcessNodeSubviews(processedNode, children);
                }
            }
        }
    }
}
