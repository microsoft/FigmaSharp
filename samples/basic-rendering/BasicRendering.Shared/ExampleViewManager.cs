/* 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;

using FigmaSharp;
using FigmaSharp.Models;
using FigmaSharp.Services;

using LiteForms;

namespace ExampleFigma
{
    public class ExampleViewManager 
    {
        const string fileName = "CobaSo7LmEYsuGZB0ED0ewSs";

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
            var rendererService = new FigmaFileRendererService(fileProvider, converters);
            rendererService.Start(fileName, scrollView);

            //now we have all the views processed and the relationship we can distribute all the views into the desired base view
            var distributionService = new FigmaViewRendererDistributionService(rendererService);
            distributionService.Start();

            //We want know the background color of the figma camvas and apply to our scrollview
            var canvas = fileProvider.Nodes.OfType<FigmaCanvas>().FirstOrDefault();
            if (canvas != null)
                scrollView.BackgroundColor = canvas.backgroundColor;

            //NOTE: some toolkits requires set the real size of the content of the scrollview before position layers
            scrollView.AdjustToContent();
        }
    }
}
