/* 
 * FigmaViewExtensions.cs - Extension methods for NSViews
 * 
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
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace FigmaSharp.Services
{
    public abstract class FigmaFileService
    {
        readonly public List<CustomViewConverter> CustomConverters = new List<CustomViewConverter>();
        readonly FigmaViewConverter[] FigmaDefaultConverters;

        public readonly List<IImageViewWrapper> FigmaImages = new List<IImageViewWrapper>();
        public IFigmaDocumentContainer Document { get; private set; }
        public IViewWrapper ContentView { get; private set; }

        public string File { get; private set; }
        public bool ImagesLoaded { get; private set; }

        public FigmaFileService ()
        {
            ContentView = AppContext.Current.CreateEmptyView();
            FigmaDefaultConverters = AppContext.Current.GetFigmaConverters();
        }

        public void Start(string file)
        {
            Console.WriteLine("[FigmaRemoteFileService] Starting service process..");
            Console.WriteLine($"Reading {file} from resources..");

            File = file;

            try
            {
                var template = GetContentTemplate(file);
                Document = AppContext.Current.GetFigmaDialogFromContent(template);
                Console.WriteLine($"Reading successfull");

                Console.WriteLine($"Loading views..");
                AppContext.Current.LoadFigmaFromFrameEntity(ContentView, Document, FigmaImages, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading resource");
                Console.WriteLine(ex);
            }
        }

        protected abstract string GetContentTemplate(string file);

        //TODO: This 
        IViewWrapper Recursively(FigmaNode currentNode, IViewWrapper parentView, FigmaNode parentNode)
        {
            Console.WriteLine("[{0}({1})] Processing {2}..", currentNode.id, currentNode.name, currentNode.GetType());
            IViewWrapper nextView = null;

            foreach (var customConverter in CustomConverters)
            {
                if (customConverter.CanConvert(currentNode))
                {
                    var view = customConverter.ConvertTo(currentNode, parentNode, parentView);
                    parentView.AddChild(view);
                    view.CreateConstraints(parentNode, parentView);
                    nextView = view;
                    break;
                }
            }

            if (nextView == null)
            {
                foreach (var converter in FigmaDefaultConverters)
                {
                    if (converter.CanConvert(currentNode))
                    {
                        var view = converter.ConvertTo(currentNode, parentNode, parentView);
                        if (view != null)
                        {
                            parentView.AddChild(view);
                            view.CreateConstraints(parentNode, parentView);
                            nextView = view;
                        }
                        break;
                    }
                }
            }

            Console.WriteLine("[{1}({2})] Not implemented: {0}", currentNode.GetType(), currentNode.id, currentNode.name);
            if (currentNode is IFigmaNodeContainer nodeContainer)
            {
                foreach (var item in nodeContainer.children)
                {
                    Recursively(parentNode, parentView, item);
                }
            }
            return nextView;
        }
    }

    public class FigmaLocalFileService : FigmaFileService
    {
        protected override string GetContentTemplate(string file)
        {
            return AppContext.Current.GetManifestResource(GetType().Assembly, file);
        }
    }

    public class FigmaRemoteFileService : FigmaFileService
    {
        protected override string GetContentTemplate(string file)
        {
            return AppContext.Current.GetFigmaFileContent(file, AppContext.Current.Token);
        }
    }
}
