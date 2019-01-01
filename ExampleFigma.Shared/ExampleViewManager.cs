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
        readonly RendererService rendererService;

        public ExampleViewManager(IScrollViewWrapper scrollViewWrapper, string fileName)
        {
            this.scrollViewWrapper = scrollViewWrapper;
            this.fileName = fileName;

            fileService = new FigmaRemoteFileService();
            rendererService = new RendererService(fileService);
        }
       
        public void Initialize ()
        {
            fileService.Start(fileName);
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

                view.X = currentX;
                view.Y = 0; //currentView.Height + currentHeight;
                currentX += view.Width + Margin;
            }
        }
    }
}
