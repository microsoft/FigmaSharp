using System;
using FigmaSharp;
using FigmaSharp.Services;
using System.Linq;
using System.Collections.Generic;

namespace ExampleFigma
{
    public class ExampleViewManager 
    {
        string fileName;
        readonly IScrollViewWrapper scrollViewWrapper;
        readonly FigmaRemoteFileService fileService;
        readonly FigmaRendererService rendererService;

        public ExampleViewManager(IScrollViewWrapper scrollViewWrapper, string fileName)
        {
            this.scrollViewWrapper = scrollViewWrapper;
            this.fileName = fileName;

            fileService = new FigmaRemoteFileService();
            rendererService = new FigmaRendererService(fileService);
        }
       
        public void Initialize ()
        {
            fileService.Start(fileName, 0);
            rendererService.Start();

            var mainNodes = fileService.NodesProcessed.Where(s => s.ParentView == null)
            .ToArray();

            Reposition(mainNodes);

            scrollViewWrapper.AdjustToContent();
        }

        public void Reposition(ProcessedNode[] mainNodes)
        {
            //Alignment 
            const int Margin = 20;
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
