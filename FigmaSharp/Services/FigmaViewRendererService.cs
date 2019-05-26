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

namespace FigmaSharp.Services
{
    public interface IFigmaFileProvider
    {
        List<FigmaNode> Nodes { get; }
        FigmaResponse Response { get; }
        void Load(string path);
        void Save(string filePath);
        string GetContentTemplate(string file);
        void OnStartImageProcessing(Dictionary<FigmaVectorEntity, string> imageVectors, string file);
    }

    public class FigmaLocalFileProvider : FigmaFileProvider
    {
        public override string GetContentTemplate(string file)
        {
            return System.IO.File.ReadAllText(file);
        }

        public override void OnStartImageProcessing(Dictionary<FigmaVectorEntity, string> imageVectors, string file)
        {
            //not needed in local files
        }
    }

    public class FigmaRemoteFileProvider : FigmaFileProvider
    {
        public override string GetContentTemplate(string file)
        {
            return AppContext.Current.GetFigmaFileContent(file, AppContext.Current.Token);
        }

        public override void OnStartImageProcessing(Dictionary<FigmaVectorEntity, string> imageVectors, string file)
        {
            //Remote files need get the real image url to get the file
            var vectorsIds = imageVectors.Select(s => s.Key.id);
            var figmaImageResponse = FigmaApiHelper.GetFigmaImages(file, vectorsIds);
            if (figmaImageResponse != null)
            {
                foreach (var imageResponse in figmaImageResponse.images)
                {
                    var image = imageVectors.FirstOrDefault(s => s.Key.id == imageResponse.Key).Key;
                    imageVectors[image] = imageResponse.Value;
                }
            }
        }
    }

    public class FigmaManifestFileProvider : FigmaFileProvider
    {
        public override string GetContentTemplate(string file)
        {
            return AppContext.Current.GetManifestResource(null, file);
        }

        public override void OnStartImageProcessing(Dictionary<FigmaVectorEntity, string> imageVectors, string file)
        {
            //not needed in local files
        }
    }

    public abstract class FigmaFileProvider : IFigmaFileProvider
    {
        public FigmaResponse Response { get; private set; }
        public List<FigmaNode> Nodes { get; } = new List<FigmaNode>();

        public void Load (string path)
        {
            try
            {
                Nodes.Clear();
                var template = GetContentTemplate(path);
                Response = AppContext.Current.GetFigmaResponseFromContent(template);

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
                Console.WriteLine($"Error reading resource");
                Console.WriteLine(ex);
            }
        }

        void ProcessNodeRecursively (FigmaNode node, FigmaNode parent)
        {
            node.Parent = parent;
            Nodes.Add(node);

            if (node is IFigmaNodeContainer nodeContainer)
            {
                foreach (var item in nodeContainer.children)
                    ProcessNodeRecursively(item, node);
            }
        }

        public abstract string GetContentTemplate(string file);

        public abstract void OnStartImageProcessing(Dictionary<FigmaVectorEntity, string> imageVectors, string file);

        public void Save(string filePath)
        {
            AppContext.Current.SetFigmaResponseFromContent(Response, filePath);
        }
    }

    public class FigmaViewRendererService
    {
        readonly FigmaViewConverter[] FigmaDefaultConverters;

        readonly FigmaViewConverter[] FigmaCustomConverters;

        public List<ProcessedNode> NodesProcessed = new List<ProcessedNode> ();

        public readonly Dictionary<FigmaVectorEntity, string> ImageVectors = new Dictionary<FigmaVectorEntity, string> ();

        public string File { get; private set; }
        public int Page { get; private set; }
        public bool ProcessImages { get; private set; }
        IFigmaFileProvider figmaProvider;

        public FigmaViewRendererService(IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters)
        {
            this.figmaProvider = figmaProvider;
            FigmaDefaultConverters = figmaViewConverters.Where (s => s.IsLayer).ToArray ();
            FigmaCustomConverters = figmaViewConverters.Where(s => !s.IsLayer).ToArray();
        }

        public Task StartAsync(string file) => StartAsync(file, new FigmaViewRendererServiceOptions());
        public Task StartAsync(string file, FigmaViewRendererServiceOptions options) => Task.Run(() => Start(file, options: options));

        public void Refresh (FigmaViewRendererServiceOptions options)
        {
            try
            {
                ImageVectors.Clear();
                NodesProcessed.Clear();

                Console.WriteLine($"Reading successfull");
                Console.WriteLine($"Loading views for page {Page}..");

                var canvas = figmaProvider.Response.document.children[Page];
                foreach (var item in canvas.children)
                    GenerateViewsRecursively(item, null, options);

                //Images
                if (ProcessImages)
                {
                    figmaProvider.OnStartImageProcessing(ImageVectors, File);
                }

                Console.WriteLine("View generation finished.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading resource");
                Console.WriteLine(ex);
            }
        }

        public void Start(string file) =>
            Start(file, new FigmaViewRendererServiceOptions());

        public void Start(string file, FigmaViewRendererServiceOptions options)
        {
            Console.WriteLine("[FigmaRemoteFileService] Starting service process..");
            Console.WriteLine($"Reading {file} from resources..");

            ProcessImages = options.AreImageProcessed;
            Page = options.StartPage;
            File = file;

            try
            {
                figmaProvider.Load(file);
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
            Console.WriteLine("[{0}({1})] Processing {2}..", currentNode?.id, currentNode?.name, currentNode?.GetType());

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

                //Image processing
                if (currentProcessedNode.FigmaNode is FigmaVectorEntity vectorEntity && vectorEntity.ImageSupported)
                {
                    ImageVectors.Add((FigmaVectorEntity)currentProcessedNode.FigmaNode, null);
                }
            }
            else
            {
                Console.WriteLine("[{1}({2})] There is no Converter for this type: {0}", currentNode.GetType(), currentNode.id, currentNode.name);
            }

            if (navigateChild && currentNode is IFigmaNodeContainer nodeContainer)
            {
                foreach (var item in nodeContainer.children)
                {
                    GenerateViewsRecursively(item, currentProcessedNode, options);
                }
            }
        }
    }

    public class FigmaViewRendererServiceOptions
    {
        //public bool IsToCodeProcessed { get; set; } = true;
        public bool IsToViewProcessed { get; set; } = true;
        public bool AreImageProcessed { get; set; } = true;
        public int StartPage { get; set; } = 0;
    }
}
