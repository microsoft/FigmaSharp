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

namespace FigmaSharp.Services
{
    public class FigmaViewRendererService
    {
        readonly FigmaViewConverter[] FigmaDefaultConverters;
        readonly FigmaViewConverter[] FigmaCustomConverters;

        public List<ProcessedNode> NodesProcessed = new List<ProcessedNode> ();

        public readonly Dictionary<FigmaVectorEntity, string> ImageVectors = new Dictionary<FigmaVectorEntity, string> ();

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
                if (fileProvider.NeedsImageLinks && ProcessImages)
                {
                    foreach (var processedNode in NodesProcessed)
                    {
                        if (processedNode.FigmaNode is FigmaVectorEntity vectorEntity)
                        {
                            if (vectorEntity.GetType () == typeof (FigmaVectorEntity))
                            {
                                ImageVectors.Add(vectorEntity, null);
                            } else
                            {
                                var figmaPaint = vectorEntity.fills.OfType<FigmaPaint>().FirstOrDefault();
                                if (figmaPaint != null && figmaPaint.type == "IMAGE")
                                {
                                    ImageVectors.Add(vectorEntity, null);
                                }
                            }
                           
                        }
                        //Image processing
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
            Console.WriteLine("Starting Image Binding process...");
            fileProvider.ImageLinksProcessed -= FigmaProvider_ImageLinksProcessed;
            //loading views
            foreach (var vector in ImageVectors)
            {
                Console.Write ("[{0}][{1}] Processing... ", vector.Key.id, vector.Key.name);
                var processedNode = NodesProcessed.FirstOrDefault(s => s.FigmaNode == vector.Key);
                if (!string.IsNullOrEmpty(vector.Value))
                {
                    var imageWrapper = AppContext.Current.GetImage(vector.Value);
                    if (processedNode.View is IImageViewWrapper imageViewWrapper)
                    {
                        AppContext.Current.BeginInvoke(() => {
                            try
                            {
                                imageViewWrapper.SetImage(imageWrapper);
                                Console.WriteLine("DONE");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        });
                    } else
                    {
                        Console.WriteLine("Current container is not a IImageView");
                    }
                } else
                {
                    Console.WriteLine("NO URL");
                }

                Thread.Sleep(100);
            }
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

        ProcessedNode GetProcessedNode(FigmaNode currentNode, IEnumerable<CustomViewConverter> customViewConverters, ProcessedNode parent, FigmaViewRendererServiceOptions options)
        {
            foreach (var customViewConverter in customViewConverters)
            {
                if (customViewConverter.CanConvert(currentNode))
                {
                    var currentView = options.IsToViewProcessed ? customViewConverter.ConvertTo(currentNode, parent) : null;
                    var currentElement = new ProcessedNode() { FigmaNode = currentNode, View = currentView, ParentView = parent };
                    return currentElement;
                }

            }
            return null;
        }

        //TODO: This 
        void GenerateViewsRecursively(FigmaNode currentNode, ProcessedNode parent, FigmaViewRendererServiceOptions options)
        {
            Console.WriteLine("[{0}.{1}] Processing {2}..", currentNode?.id, currentNode?.name, currentNode?.GetType());

            bool navigateChild = true;

            var currentProcessedNode = GetProcessedNode(currentNode, FigmaCustomConverters, parent, options);

            if (currentProcessedNode == null)
            {
                currentProcessedNode = GetProcessedNode(currentNode, FigmaDefaultConverters, parent, options);
            }
            else
            {
                navigateChild = false;
            }

            if (currentProcessedNode != null)
            {
                NodesProcessed.Add(currentProcessedNode);
            }
            else
            {
                Console.WriteLine("[{1}.{2}] There is no Converter for this type: {0}", currentNode.GetType(), currentNode.id, currentNode.name);
            }

            if (navigateChild && currentNode is IFigmaNodeContainer nodeContainer)
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
