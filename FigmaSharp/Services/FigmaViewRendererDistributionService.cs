using System.Collections.Generic;
using System.Linq;
using System;
using FigmaSharp;
using FigmaSharp.Models;

namespace FigmaSharp.Services
{
    public class FigmaViewRendererDistributionService
    {
        public ProcessedNode[] MainViews { get; private set; }
        protected FigmaRendererService figmaFileService;

        public FigmaViewRendererDistributionService(FigmaRendererService figmaFileService)
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
                Recursively(mainNodes[i]);
            }
        }

        void Recursively(ProcessedNode parentNode)
        {
            var children = figmaFileService.NodesProcessed.Where(s => s.ParentView == parentNode);
            foreach (var child in children)
            {
                if (child.FigmaNode is IAbsoluteBoundingBox absoluteBounding && parentNode.FigmaNode is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
                {
                    parentNode.View.AddChild(child.View);

                    var x = Math.Max (absoluteBounding.absoluteBoundingBox.X - parentAbsoluteBoundingBox.absoluteBoundingBox.X, 0);
                    float y;
                    if (AppContext.Current.IsVerticalAxisFlipped)
                    {
                        var parentY = parentAbsoluteBoundingBox.absoluteBoundingBox.Y + parentAbsoluteBoundingBox.absoluteBoundingBox.Height;
                        var actualY = absoluteBounding.absoluteBoundingBox.Y + absoluteBounding.absoluteBoundingBox.Height;
                        y = parentY - actualY;
                    }
                    else
                    {
                        y = absoluteBounding.absoluteBoundingBox.Y - parentAbsoluteBoundingBox.absoluteBoundingBox.Y;
                    }

                    child.View.SetAllocation(x, y, Math.Max (absoluteBounding.absoluteBoundingBox.Width, 1), Math.Max (1, absoluteBounding.absoluteBoundingBox.Height));
                }

                Recursively(child);
            }
        }
    }
}
