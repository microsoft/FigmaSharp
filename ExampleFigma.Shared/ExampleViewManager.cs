using System;
using FigmaSharp;
using FigmaSharp.Services;

namespace ExampleFigma
{
    public class ExampleViewManager 
    {
        string fileName;
        IScrollViewWrapper scrollViewWrapper;

        public ExampleViewManager(IScrollViewWrapper scrollViewWrapper, string fileName)
        {
            this.scrollViewWrapper = scrollViewWrapper;
            this.fileName = fileName;
        }

        public void Initialize ()
        {
            var fileService = new FigmaRemoteFileService();
            fileService.Start(fileName);
            var builderService = new ScrollViewRendererService();
            builderService.Start(scrollViewWrapper, fileService);
            scrollViewWrapper.AdjustToContent();
        }
    }
}
