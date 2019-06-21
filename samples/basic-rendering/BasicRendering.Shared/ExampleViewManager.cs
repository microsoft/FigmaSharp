using System;
using System.Collections.Generic;
using System.Linq;

using FigmaSharp;
using FigmaSharp.Services;

namespace ExampleFigma
{
    public class ExampleViewManager 
    {
        const string fileName = "YdrY6p8JHY2UaKlSFgOwwUnd";
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
            fileService.Start(fileName, scrollViewWrapper);

            rendererService.Start();

            var mainNodes = fileService.NodesProcessed
                .Where (s => s.ParentView?.FigmaNode is FigmaCanvas)
                .ToArray();

            //NOTE: some toolkits requires set the real size of the content of the scrollview before position layers
            var canvas = fileProvider.Nodes.OfType<FigmaCanvas>().FirstOrDefault();
            if (canvas != null)
                scrollViewWrapper.BackgroundColor = canvas.backgroundColor;

            scrollViewWrapper.AdjustToContent();
        }
    }
}
