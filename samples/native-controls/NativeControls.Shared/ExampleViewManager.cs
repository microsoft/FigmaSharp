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
using System.Linq;

using FigmaSharp;
using FigmaSharp.NativeControls;
using FigmaSharp.Services;
using FigmaSharp.Models;

namespace LocalFile.Shared
{
    public class ExampleViewManager
    {
        const string fileName = "EGTUYgwUC9rpHmm4kJwZQXq4";
        readonly IScrollViewWrapper scrollViewWrapper;
        readonly FigmaRemoteFileProvider fileProvider;

        readonly FigmaViewRendererService rendererService;
        readonly FigmaViewRendererDistributionService distributionService;

        public ExampleViewManager(IScrollViewWrapper scrollViewWrapper, FigmaViewConverter[] converters)
        {
            this.scrollViewWrapper = scrollViewWrapper;
            fileProvider = new FigmaRemoteFileProvider();
            rendererService = new FigmaViewRendererService(fileProvider, converters);
          
            rendererService.Start(fileName, scrollViewWrapper.ContentView);

            distributionService = new FigmaViewRendererDistributionService(rendererService);
            distributionService.Start();

            //We want know the background color of the figma camvas and apply to our scrollview
            var canvas = fileProvider.Nodes.OfType<FigmaCanvas>().FirstOrDefault();
            if (canvas != null)
                scrollViewWrapper.BackgroundColor = canvas.backgroundColor;


            scrollViewWrapper.AdjustToContent();
        }
    }
}

