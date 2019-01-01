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
        IScrollViewWrapper scrollViewWrapper;
        readonly FigmaRemoteFileService fileService;
        readonly RendererService rendererService;

        ProcessedNode[] MainNodes;

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

            MainNodes = fileService.NodesProcessed.Where(s => s.ParentView == null)
            .ToArray();

            Reposition();

            scrollViewWrapper.AdjustToContent();
        }

        public void Reposition()
        {
            //Alignment 
            const int Margin = 20;
            float currentX = Margin;
            foreach (var processedNode in MainNodes)
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
