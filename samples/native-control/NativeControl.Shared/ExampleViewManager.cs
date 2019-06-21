using System;
using System.Linq;

using FigmaSharp;
using FigmaSharp.NativeControls;
using FigmaSharp.Services;

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
            fileService.Start(fileName, scrollViewWrapper);
            rendererService.Start();

            scrollViewWrapper.AdjustToContent();
        }
    }
}

