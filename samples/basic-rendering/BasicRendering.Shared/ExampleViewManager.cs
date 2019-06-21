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
