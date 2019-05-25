using FigmaSharp;
using FigmaSharp.NativeControls;
using FigmaSharp.Services;
using System;
using System.Linq;

namespace ExampleFigmaMac
{
    public class ExampleViewManager
    {
        string fileName;
        readonly IScrollViewWrapper scrollViewWrapper;
        readonly FigmaRemoteFileService fileService;
        readonly FigmaRendererService rendererService;

        public ExampleViewManager(IScrollViewWrapper scrollViewWrapper, string fileName, FigmaViewConverter[] converters, CustomViewConverter[] customViewConverters)
        {
            this.scrollViewWrapper = scrollViewWrapper;
            this.fileName = fileName;

            //var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();
            fileService = new FigmaRemoteFileService(converters);


            fileService.CustomViewConverters.AddRange(customViewConverters);

            rendererService = new FigmaRendererService(fileService);
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

                view.X = currentX;
                view.Y = 0; //currentView.Height + currentHeight;
                currentX += view.Width + Margin;
            }
        }
    }
}

