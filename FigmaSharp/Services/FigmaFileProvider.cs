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
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

using FigmaSharp.Models;
using FigmaSharp.Converters;
using System.IO;

namespace FigmaSharp.Services
{
    public interface IFigmaFileProvider
    {
        string File { get; }
        bool NeedsImageLinks { get; }
        event EventHandler ImageLinksProcessed;
        List<FigmaNode> Nodes { get; }
        FigmaResponse Response { get; }
        void Load(string file);
        void Save(string filePath);
        string GetContentTemplate(string file);
        void OnStartImageLinkProcessing(List<ProcessedNode> imageVectors);
    }

    public class FigmaLocalFileProvider : FigmaFileProvider
    {
        public FigmaLocalFileProvider (string resourcesDirectory)
        {
            ResourcesDirectory = resourcesDirectory;
        }

        public override string GetContentTemplate(string file)
        {
            return System.IO.File.ReadAllText(file);
        }

        public string ResourcesDirectory { get; set; }

        public string ImageFormat { get; set; } = ".png";

        public override void OnStartImageLinkProcessing(List<ProcessedNode> imageFigmaNodes)
        {
            //not needed in local files
            Console.WriteLine($"Loading images..");
           
            if (imageFigmaNodes.Count > 0)
            {
                foreach (var vector in imageFigmaNodes)
                {
                    try
                    {
                        var recoveredKey = FigmaResourceConverter.FromResource(vector.FigmaNode.id);
                        string filePath = Path.Combine(ResourcesDirectory, string.Concat(recoveredKey, ImageFormat));

                        if (!System.IO.File.Exists(filePath))
                        {
                            throw new FileNotFoundException(filePath);
                        }
                      
                        if (vector.View is IImageViewWrapper imageView)
                        {
                            var image = AppContext.Current.GetImageFromFilePath(filePath);
                            imageView.SetImage(image);
                        }
                    }
                    catch (FileNotFoundException ex)
                    {
                        Console.WriteLine("[FIGMA.RENDERER] Resource '{0}' not found.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }

            Console.WriteLine("Ended image link processing");
            OnImageLinkProcessed();
        }
    }

    public class FigmaRemoteFileProvider : FigmaFileProvider
    {
        public override bool NeedsImageLinks => true;

        public override string GetContentTemplate(string file)
        {
            return AppContext.Current.GetFigmaFileContent(file, AppContext.Current.Token);
        }

        public IEnumerable<string> GetKeys (List<FigmaImageResponse> responses, string image)
        {
            foreach (var item in responses)
            {
                foreach (var keys in item.images.Where(s => s.Value == image))
                {
                    yield return keys.Key;
                }
            }
        }

        public override void OnStartImageLinkProcessing(List<ProcessedNode> imageFigmaNodes)
        {
            if (imageFigmaNodes.Count == 0)
            {
                OnImageLinkProcessed();
                return;
            }

            Task.Run(() => {

                try
                {
                    var totalImages = imageFigmaNodes.Count();
                    //TODO: figma url has a limited character in urls we fixed the limit to 10 ids's for each call
                    var numberLoop = (totalImages / CallNumber) + 1;

                    //var imageCache = new Dictionary<string, List<string>>();
                    List<Tuple<string, List<string>>> imageCacheResponse = new List<Tuple<string, List<string>>>();
                    Console.WriteLine("Detected a total of {0} possible images.  ", totalImages);

                    var images = new List<string>();
                    for (int i = 0; i < numberLoop; i++)
                    {
                        var vectors = imageFigmaNodes.Skip(i * CallNumber).Take(CallNumber);
                        Console.WriteLine("[{0}/{1}] Processing Images ... {2} ", i, numberLoop, vectors.Count());
                        var figmaImageResponse = FigmaApiHelper.GetFigmaImages(File, vectors.Select(s => s.FigmaNode.id));

                        if (figmaImageResponse != null)
                        {
                            foreach (var image in figmaImageResponse.images)
                            {
                                if (image.Value == null)
                                {
                                    continue;
                                }

                                var img = imageCacheResponse.FirstOrDefault(s => image.Value == s.Item1);
                                if (img?.Item1 != null)
                                {
                                    img.Item2.Add(image.Key);
                                }
                                else
                                {
                                    imageCacheResponse.Add(new Tuple<string, List<string>>(image.Value, new List<string>() { image.Key }));
                                }
                            }
                        }
                    }

                    Console.WriteLine("Removing dupplicates...");

                    //get images not dupplicates
                    Console.WriteLine("Finished image to download {0}", images.Count);

                    //with all the keys now we get the dupplicated images
                    foreach (var imageUrl in imageCacheResponse)
                    {
                        var imageWrapper = AppContext.Current.GetImage(imageUrl.Item1);
                        foreach (var figmaNodeId in imageUrl.Item2)
                        {
                            var vector = imageFigmaNodes.FirstOrDefault(s => s.FigmaNode.id == figmaNodeId);
                            Console.Write("[{0}:{1}:{2}] {3}...", vector.FigmaNode.GetType(), vector.FigmaNode.id, vector.FigmaNode.name, imageUrl);

                            if (vector != null && vector.View is IImageViewWrapper imageView)
                            {
                                AppContext.Current.BeginInvoke(() =>
                                {
                                    imageView.SetImage(imageWrapper);
                                });
                            }
                            Console.Write("OK \n");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
               
                OnImageLinkProcessed();
            });
        }
        const int CallNumber = 250;
    }

    public class FigmaManifestFileProvider : FigmaFileProvider
    {
        public Assembly Assembly { get; set; }

        public FigmaManifestFileProvider (Assembly assembly)
        {
            Assembly = assembly;
        }

        public override string GetContentTemplate(string file)
        {
            return AppContext.Current.GetManifestResource(Assembly, file);
        }

        public override void OnStartImageLinkProcessing(List<ProcessedNode> imageFigmaNodes)
        {
            Console.WriteLine($"Loading images..");

            if (imageFigmaNodes.Count > 0)
            {
                foreach (var vector in imageFigmaNodes)
                {
                    var recoveredKey = FigmaResourceConverter.FromResource(vector.FigmaNode.id);
                    var image = AppContext.Current.GetImageFromManifest(Assembly, recoveredKey);
                    if (image != null && vector.View is IImageViewWrapper imageView)
                    {
                        imageView.SetImage(image);
                    }
                }
            }

            Console.WriteLine("Ended image link processing");
            OnImageLinkProcessed();
        }
    }

    public abstract class FigmaFileProvider : IFigmaFileProvider
    {
        public virtual bool NeedsImageLinks => false;

        public event EventHandler ImageLinksProcessed;

        public FigmaResponse Response { get; private set; }
        public List<FigmaNode> Nodes { get; } = new List<FigmaNode>();

        public bool ImageProcessed;

        internal void OnImageLinkProcessed()
        {
            ImageProcessed = true;
            ImageLinksProcessed?.Invoke(this, new EventArgs());
        }

        public string File { get; private set; }

        public void Load(string file)
        {
            this.File = file;

            ImageProcessed = false;
            try
            {
                Nodes.Clear();

                var contentTemplate = GetContentTemplate(file);

                Response = AppContext.Current.GetFigmaResponseFromContent(contentTemplate);

                foreach (var item in Response.document.children)
                {
                    ProcessNodeRecursively(item, null);
                }
            }
            catch (System.Net.WebException ex)
            {
                if (!AppContext.Current.IsConfigured)
                    Console.WriteLine($"Cannot connect to FigmaServer, TOKEN not configured.");
                else
                    Console.WriteLine($"Cannot connect to FigmaServer, wrong TOKEN?");

                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading remote resources. Ensure you added NewtonSoft nuget?");
                Console.WriteLine(ex);
            }
        }

        void ProcessNodeRecursively(FigmaNode node, FigmaNode parent)
        {
            node.Parent = parent;
            Nodes.Add(node);

            if (node is FigmaInstance instance)
            {
                if (Response.components.TryGetValue(instance.componentId, out var figmaComponent))
                {
                    instance.Component = figmaComponent;
                }
            }

            if (node is IFigmaNodeContainer nodeContainer)
            {
                foreach (var item in nodeContainer.children)
                    ProcessNodeRecursively(item, node);
            }
        }

        public abstract string GetContentTemplate(string file);

        public abstract void OnStartImageLinkProcessing(List<ProcessedNode> imageFigmaNodes);

        public void Save(string filePath)
        {
            AppContext.Current.SetFigmaResponseFromContent(Response, filePath);
        }
    }
}
