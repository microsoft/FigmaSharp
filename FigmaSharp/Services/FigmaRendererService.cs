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
        FigmaFileService figmaFileService;

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

                var children = figmaFileService.NodesProcessed.Where(s => s.ParentView == mainNodes[i]);
                if (children.Any())
                {
                    ProcessNodeSubviews(mainNodes[i], i, children);
                }
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
                        wrapper.SetImage(image);
                    }
                }
            });
        }

        void ProcessNodeSubviews(ProcessedNode parentProcessedNode, int id, IEnumerable<ProcessedNode> nodes)
        {
            //we start to process all nodes
            foreach (var processedNode in nodes)
            {
                parentProcessedNode.View.AddChild(processedNode.View);

                if (processedNode.FigmaNode is IAbsoluteBoundingBox absoluteBounding && parentProcessedNode.FigmaNode is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
                {
                    processedNode.View.X = absoluteBounding.absoluteBoundingBox.x - parentAbsoluteBoundingBox.absoluteBoundingBox.x;
                    processedNode.View.Y = absoluteBounding.absoluteBoundingBox.y - parentAbsoluteBoundingBox.absoluteBoundingBox.y; // currentHeight - absoluteBounding.absoluteBoundingBox.height - (absoluteBounding.absoluteBoundingBox.y - parentAbsoluteBoundingBox.absoluteBoundingBox.y);
                }

                var children = figmaFileService.NodesProcessed.Where(s => s.ParentView == processedNode);
                if (children.Any())
                {
                    ProcessNodeSubviews(processedNode, id, children);
                }
            }
        }
    }
}
