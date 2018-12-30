using System.Collections.Generic;
using FigmaSharp;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace FigmaSharp.Services
{
    public class ScrollViewRendererService : IRendererService<IScrollViewWrapper>
    {
        public ProcessedNode[] MainViews { get; private set; }

        FigmaFileService figmaFileService;
        IViewWrapper currentView;

        public void Start (IScrollViewWrapper targetView, FigmaFileService figmaFileService)
        {
            this.currentView = targetView;
            this.figmaFileService = figmaFileService;

            var mainNodes = figmaFileService.NodesProcessed.Where(s => s.ParentView == null).ToArray ();
            MainViews = new ProcessedNode[mainNodes.Length];
            for (int i = 0; i < mainNodes.Length; i++)
            {
                MainViews[i] = mainNodes[i];


                var children = figmaFileService.NodesProcessed.Where(s => s.ParentView == mainNodes[i]);
                if (children.Any())
                {
                    ProcessNodeSubviews(mainNodes[i], i, children);
                }
            }

            targetView.ClearSubviews();

            Reposition();

            targetView.AdjustToContent();

            //loading views
            Task.Run(() =>
           {
               foreach (var vector in figmaFileService.ImageVectors)
               {
                   var processedNode = figmaFileService.NodesProcessed.FirstOrDefault(s => s.FigmaNode == vector.Key);
                   if (!string.IsNullOrEmpty(vector.Value))
                   {
                       var image = AppContext.Current.GetImage(vector.Value);
                       var wrapper = processedNode.View as IImageViewWrapper;
                       wrapper.SetImage(image);
                   }
               }
           });
        }

       
        public void Reposition()
        {
            //Alignment 
            const int Margin = 20;
            float currentX = Margin;
            foreach (var processedNode in MainViews)
            {
                var view = processedNode.View;

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

                var children = figmaFileService.NodesProcessed.Where(s => s.ParentView == processedNode);
                if (children.Any ())
                {
                    ProcessNodeSubviews(processedNode, id, children);
                }
            }
        }
    }
}
