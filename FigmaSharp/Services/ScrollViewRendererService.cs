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
                    ProcessNodeSubviews(mainNodes[i].View, children);
                }
            }

            targetView.ClearSubviews();

            //Alignment 
            int MaxColumns = 3;
            int currentColumn = 0;
            int file = 0;

            float currentX = 0;
            foreach (var processedNode in MainViews)
            {
                var view = processedNode.View;
                targetView.AddChild(view);
                view.X = currentX;

                currentX += view.Width + 20;
            }
        }

        void ProcessNodeSubviews(IViewWrapper parentView, IEnumerable<ProcessedNode> nodes)
        {
            //we start to process all nodes
            foreach (var processedNode in nodes)
            {
                parentView.AddChild(processedNode.View);
                var children = processedNodes.Where(s => s.ParentView == processedNode);
                if (children.Any ())
                {
                    ProcessNodeSubviews(processedNode.View, children);
                }
            }
        }
    }
}
