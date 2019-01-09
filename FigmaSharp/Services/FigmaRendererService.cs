using System.Collections.Generic;
using FigmaSharp;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace FigmaSharp.Services
{
    public class FigmaRendererService
    {
        public ProcessedNode[] MainViews { get; private set; }
        protected FigmaFileService figmaFileService;

        public FigmaRendererService(FigmaFileService figmaFileService)
        {
            this.figmaFileService = figmaFileService;
        }

        public void Start ()
        {
            var mainNodes = figmaFileService.NodesProcessed.Where(s => s.ParentView == null)
                .ToArray();

            MainViews = new ProcessedNode[mainNodes.Length];

            for (int i = 0; i < mainNodes.Length; i++)
            {
                MainViews[i] = mainNodes[i];
                ProcessNodeSubviews(mainNodes[i]);
            }

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

                        AppContext.Current.BeginInvoke(() => {
                           wrapper.SetImage(image);
                        });
                    }
                }
            });
        }

        void ProcessNodeSubviews(ProcessedNode parentProcessedNode)
        {
            var children = figmaFileService.NodesProcessed.Where(s => s.ParentView == parentProcessedNode);
            foreach (var child in children)
            {
                if (child.FigmaNode is IAbsoluteBoundingBox absoluteBounding && parentProcessedNode.FigmaNode is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
                {
                    child.View.X = absoluteBounding.absoluteBoundingBox.x -  parentAbsoluteBoundingBox.absoluteBoundingBox.x;
                   
                    if (AppContext.Current.IsYAxisFlipped)
                    {
                        var parentY = parentAbsoluteBoundingBox.absoluteBoundingBox.y + parentAbsoluteBoundingBox.absoluteBoundingBox.height;
                        var actualY = absoluteBounding.absoluteBoundingBox.y + absoluteBounding.absoluteBoundingBox.height;
                        child.View.Y = parentY - actualY;
                    }
                    else
                    {
                        child.View.Y = absoluteBounding.absoluteBoundingBox.y - parentAbsoluteBoundingBox.absoluteBoundingBox.y;
                    }
                }
                parentProcessedNode.View.AddChild(child.View);

                ProcessNodeSubviews(child);
            }
        }
    }
}
