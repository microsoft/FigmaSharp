using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicRendering.Wpf
{
    public class ExampleViewManager
    {
        const string fileName = "qVIUSmz8AkK158q8pY3L23";// https://www.figma.com/file/qVIUSmz8AkK158q8pY3L23/NicoTest?node-id=0%3A1

        readonly FigmaRemoteFileProvider fileProvider;

        public string WindowTitle => fileProvider.Response.name;

        public ExampleViewManager(IScrollView scrollView)
        {
            //we get the default basic view converters from the current loaded toolkit
            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();

            //TIP: the render consist in 2 steps:
            //1) generate all the views, decorating and calculate sizes
            //2) with this views we generate the hierarchy and position all the views based in the
            //native toolkit positioning system

            //in this case we want use a remote file provider (figma url from our document)
            fileProvider = new FigmaRemoteFileProvider();

            //we initialize our renderer service, this uses all the converters passed
            //and generate a collection of NodesProcessed which is basically contains <FigmaModel, IView, FigmaParentModel>
            var rendererService = new FigmaViewRendererService(fileProvider, converters);
            rendererService.Start(fileName, scrollView);

            //now we have all the views processed and the relationship we can distribute all the views into the desired base view
            //var distributionService = new FigmaViewRendererDistributionService(rendererService);
            //distributionService.Start();

            var layoutManager = new StoryboardLayoutManager();
            layoutManager.Run(scrollView.ContentView, rendererService);

            //We want know the background color of the figma camvas and apply to our scrollview
            var canvas = fileProvider.Nodes.OfType<FigmaCanvas>().FirstOrDefault();
            if (canvas != null)
                scrollView.BackgroundColor = canvas.backgroundColor;

            //NOTE: some toolkits requires set the real size of the content of the scrollview before position layers
            scrollView.AdjustToContent();
        }
    }
}
