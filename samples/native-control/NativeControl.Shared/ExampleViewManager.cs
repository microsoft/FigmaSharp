using FigmaSharp;
using FigmaSharp.NativeControls;
using FigmaSharp.Services;
using System;
using System.Linq;

namespace ExampleFigmaMac
{
    public class ExampleViewManager
    {
        const string fileName = "Dq1CFm7IrDi3UJC7KJ8zVjOt";
        readonly IScrollViewWrapper scrollViewWrapper;
        readonly FigmaRemoteFileProvider fileProvider;

        readonly FigmaViewRendererService fileService;
        readonly FigmaViewRendererDistributionService rendererService;

        public ExampleViewManager(IScrollViewWrapper scrollViewWrapper, FigmaViewConverter[] converters)
        {
            this.scrollViewWrapper = scrollViewWrapper;
            fileProvider = new FigmaRemoteFileProvider();
            fileService = new FigmaViewRendererService(fileProvider, converters);
            rendererService = new FigmaViewRendererDistributionService(fileService);
        }

        public void Initialize()
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
                view.SetPosition (currentX, 0);
                currentX += view.Width + Margin;
            }
        }
    }
}

