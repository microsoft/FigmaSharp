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
using System.Linq;
using System.Reflection;
using System.Threading;

using FigmaSharp.Models;

namespace FigmaSharp.Services
{
    public class ImageProcessed
    {
        public ImageProcessed(FigmaVectorEntity node)
        {
            Node = node;
        }

        public FigmaVectorEntity Node { get; set; }
        public string Url { get; set; }
        public IImageWrapper Image { get; set; }
        public IImageViewWrapper ViewWrapper { get; set; }
    }

    public class FigmaViewRendererService
    {
        readonly FigmaViewConverter[] FigmaDefaultConverters;
        readonly FigmaViewConverter[] FigmaCustomConverters;

        public List<ProcessedNode> NodesProcessed = new List<ProcessedNode> ();

        public readonly List<ImageProcessed> ImageVectors = new List<ImageProcessed>();

        public string File { get; private set; }
        public int Page { get; private set; }
        public bool ProcessImages { get; private set; }
        IFigmaFileProvider fileProvider;

        public FigmaViewRendererService(IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters)
        {
            this.fileProvider = figmaProvider;
            FigmaDefaultConverters = figmaViewConverters.Where (s => s.IsLayer).ToArray ();
            FigmaCustomConverters = figmaViewConverters.Where(s => !s.IsLayer).ToArray();
        }

        public Task StartAsync(string file, IViewWrapper container) => StartAsync(file, container, new FigmaViewRendererServiceOptions());
        public Task StartAsync(string file, IViewWrapper container, FigmaViewRendererServiceOptions options) => Task.Run(() => Start(file, container, options: options));

        public void Refresh (FigmaViewRendererServiceOptions options)
        {
            try
            {
                ImageVectors.Clear();
                NodesProcessed.Clear();

                Console.WriteLine($"Reading successfull");
                Console.WriteLine($"Loading views for page {Page}..");

                var canvas = fileProvider.Response.document.children.FirstOrDefault ();
                var processedParentView = new ProcessedNode() { FigmaNode = canvas, View = container };
                NodesProcessed.Add (processedParentView);

                FigmaRectangle contentRect = FigmaRectangle.Zero;
                for (int i = 0; i < canvas.children.Length; i++)
                {
                    if (canvas.children[i] is IAbsoluteBoundingBox box)
                    {
                        if (i == 0)
                        {
                            contentRect = box.absoluteBoundingBox;
                        }
                        else
                        {
                            contentRect = contentRect.UnionWith(box.absoluteBoundingBox);
                        }
                    }
                }
          
                //figma cambas
                canvas.absoluteBoundingBox = contentRect;

                foreach (var item in canvas.children)
                    GenerateViewsRecursively(item, processedParentView, options);

                //Images
                if (ProcessImages)
                {
                    foreach (var processedNode in NodesProcessed)
                    {
                        if (processedNode.FigmaNode is FigmaVectorEntity vectorEntity)
                        {
                            //TODO: this should be replaced by svg
                            if (vectorEntity.HasImage ())
                            {
                                var imageProcessed = new ImageProcessed(vectorEntity) { ViewWrapper = processedNode.View as IImageViewWrapper };
                                ImageVectors.Add(imageProcessed);
                            }
                        }
                    }

                    fileProvider.ImageLinksProcessed += FigmaProvider_ImageLinksProcessed;
                    fileProvider.OnStartImageLinkProcessing(ImageVectors, File);
                }

                Console.WriteLine("View generation finished.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading resource");
                Console.WriteLine(ex);
            }
        }

        void FigmaProvider_ImageLinksProcessed(object sender, EventArgs e)
        {
            Console.WriteLine("Ended Image Binding process.");
        }

        IViewWrapper container;

        public void Start(string file, IViewWrapper container) =>
            Start(file, container, new FigmaViewRendererServiceOptions());

        public void Start(string file, IViewWrapper container, FigmaViewRendererServiceOptions options)
        {
            Console.WriteLine("[FigmaRemoteFileService] Starting service process..");
            Console.WriteLine($"Reading {file} from resources..");

            this.container = container;

            ProcessImages = options.AreImageProcessed;
            Page = options.StartPage;
            File = file;

            try
            {
                fileProvider.Load(file);
                Refresh(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading resource");
                Console.WriteLine(ex);
            }
        }

        CustomViewConverter GetProcessedConverter (FigmaNode currentNode, IEnumerable<CustomViewConverter> customViewConverters, ProcessedNode parent, FigmaViewRendererServiceOptions options)
        {
            foreach (var customViewConverter in customViewConverters)
            {
                if (customViewConverter.CanConvert(currentNode))
                {
                    return customViewConverter;
                }
            }
            return null;
        }

        //TODO: This 
        void GenerateViewsRecursively(FigmaNode currentNode, ProcessedNode parent, FigmaViewRendererServiceOptions options)
        {
            Console.WriteLine("[{0}.{1}] Processing {2}..", currentNode?.id, currentNode?.name, currentNode?.GetType());

            bool scanChildren = true;

            var converter = GetProcessedConverter(currentNode, FigmaCustomConverters, parent, options);

            if (converter == null)
            {
                converter = GetProcessedConverter (currentNode, FigmaDefaultConverters, parent, options);
            }

            ProcessedNode currentProcessedNode = null;
            if (converter != null)
            {
                var currentView = options.IsToViewProcessed ? converter.ConvertTo(currentNode, parent) : null;
                currentProcessedNode = new ProcessedNode() { FigmaNode = currentNode, View = currentView, ParentView = parent };
                NodesProcessed.Add(currentProcessedNode);

                scanChildren = converter.ScanChildren (currentNode);
            } else
            {
                scanChildren = false;
                Console.WriteLine("[{1}.{2}] There is no Converter for this type: {0}", currentNode.GetType(), currentNode.id, currentNode.name);
            }

            if (scanChildren && currentNode is IFigmaNodeContainer nodeContainer)
            {
                foreach (var item in nodeContainer.children)
                {
                    GenerateViewsRecursively(item, currentProcessedNode ?? parent, options);
                }
            }
        }
    }

    public class FigmaViewRendererServiceOptions
    {
        public bool IsToViewProcessed { get; set; } = true;
        public bool AreImageProcessed { get; set; } = true;
        public int StartPage { get; set; } = 0;
    }
}
