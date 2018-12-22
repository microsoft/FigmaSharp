using System.Collections.Generic;
using FigmaSharp;
using System.Linq;
using System;

namespace FigmaSharp.Services
{
    public class ScrollViewRendererService : IRendererService<IScrollViewWrapper>
    {
        public ProcessedNode[] MainViews { get; private set; }

        IEnumerable<ProcessedNode> processedNodes;
        IViewWrapper currentView;
        float currentHeight;

        public void Start (IScrollViewWrapper targetView, IEnumerable<ProcessedNode> processedNodes)
        {
            this.currentView = targetView;
            this.processedNodes = processedNodes;

            var mainNodes = processedNodes.Where(s => s.ParentView == null).ToArray ();
            MainViews = new ProcessedNode[mainNodes.Length];
            for (int i = 0; i < mainNodes.Length; i++)
            {
                MainViews[i] = mainNodes[i];

                currentHeight = ((IAbsoluteBoundingBox)mainNodes[i].FigmaNode).absoluteBoundingBox.height;

                var children = processedNodes.Where(s => s.ParentView == mainNodes[i]);
                if (children.Any())
                {
                    ProcessNodeSubviews(mainNodes[i], i, children);
                }
            }

            targetView.ClearSubviews();

            Reposition();


            targetView.AdjustToContent();

        }

        public void Reposition()
        {
            //Alignment 
            const int Margin = 20;

            float currentX = Margin;
            foreach (var processedNode in MainViews)
            {
                var view = processedNode.View;
                currentHeight = ((IAbsoluteBoundingBox)processedNode.FigmaNode).absoluteBoundingBox.y;

                currentView.AddChild(view);

                view.X = currentX;
                view.Y = 0; //currentView.Height + currentHeight;
                currentX += view.Width + Margin;
            }
        }

        void ProcessNodeSubviews(ProcessedNode parentProcessedNode, int id, IEnumerable<ProcessedNode> nodes)
        {
            //we start to process all nodes
            foreach (var processedNode in nodes)
            {
                parentProcessedNode.View.AddChild(processedNode.View);

                if (processedNode.FigmaNode is IAbsoluteBoundingBox absoluteBounding && parentProcessedNode.FigmaNode is IAbsoluteBoundingBox parentAbsoluteBoundingBox) {
                    processedNode.View.X =  absoluteBounding.absoluteBoundingBox.x - parentAbsoluteBoundingBox.absoluteBoundingBox.x;
                    processedNode.View.Y = absoluteBounding.absoluteBoundingBox.y - parentAbsoluteBoundingBox.absoluteBoundingBox.y; // currentHeight - absoluteBounding.absoluteBoundingBox.height - (absoluteBounding.absoluteBoundingBox.y - parentAbsoluteBoundingBox.absoluteBoundingBox.y);
                }

                var children = processedNodes.Where(s => s.ParentView == processedNode);
                if (children.Any ())
                {
                    ProcessNodeSubviews(processedNode, id, children);
                }
            }
        }
    }
}
