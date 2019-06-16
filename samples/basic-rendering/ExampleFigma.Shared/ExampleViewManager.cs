using System;
using FigmaSharp;
using FigmaSharp.Services;
using System.Linq;
using System.Collections.Generic;

namespace ExampleFigma
{
    public class ExampleViewManager 
    {
        const string fileName = "nWFYvx7YMBEtPAgAydX66e";
        readonly IScrollViewWrapper scrollViewWrapper;
        readonly FigmaViewRendererService fileService;
        readonly FigmaViewRendererDistributionService rendererService;
        readonly FigmaRemoteFileProvider fileProvider;

        public ExampleViewManager(IScrollViewWrapper scrollViewWrapper)
        {
            this.scrollViewWrapper = scrollViewWrapper;
            fileProvider = new FigmaRemoteFileProvider();
            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();
            fileService = new FigmaViewRendererService(fileProvider, converters);
            rendererService = new FigmaViewRendererDistributionService(fileService);
        }
       
        public void Initialize ()
        {
            fileService.Start(fileName, new FigmaViewRendererServiceOptions() { StartPage = 0 });
            rendererService.Start();

            var mainNodes = fileService.NodesProcessed.Where(s => s.ParentView == null)
            .ToArray();

            //NOTE: some toolkits requires set the real size of the content of the scrollview before position layers
            var scrollSize = GetScrollSize (mainNodes);
            scrollViewWrapper.SetContentSize (scrollSize.Item1, scrollSize.Item2);

            var canvas = fileProvider.Nodes.OfType<FigmaCanvas>().FirstOrDefault();
            if (canvas != null)
                scrollViewWrapper.BackgroundColor = canvas.backgroundColor;

            Reposition(mainNodes);
        }

        private Tuple<float, float> GetScrollSize(ProcessedNode[] mainNodes)
        {
            float width = 0;
            float height = 0;
            foreach (var processedNode in mainNodes)
            {
                var node = processedNode.FigmaNode;
                if (node is IAbsoluteBoundingBox figmaNodeBounding)
                {
                    width += figmaNodeBounding.absoluteBoundingBox.width + Margin;
                    height = Math.Max(height, figmaNodeBounding.absoluteBoundingBox.height);
                }
            }
            return new Tuple<float, float>(width, height);
        }

        //Alignment 
        const int Margin = 20;

        public void Reposition(ProcessedNode[] mainNodes)
        {
            float currentX = Margin;
            foreach (var processedNode in mainNodes)
            {
                var view = processedNode.View;
                scrollViewWrapper.AddChild(view);
                view.SetPosition(currentX, 0);
                currentX += view.Width + Margin;
            }
        }
    }
}
