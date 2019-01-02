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
    public abstract class FigmaFileService
    {
        readonly public List<CustomViewConverter> CustomViewConverters = new List<CustomViewConverter>();
        readonly FigmaViewConverter[] FigmaDefaultConverters;

        public List<ProcessedNode> NodesProcessed = new List<ProcessedNode>();

        public readonly Dictionary<FigmaVectorEntity, string> ImageVectors = new Dictionary<FigmaVectorEntity, string> ();

        public FigmaResponse Response { get; private set; }

        public string File { get; private set; }
        public int Page { get; private set; }
        public bool ProcessImages { get; private set; }

        public FigmaFileService()
        {
            FigmaDefaultConverters = AppContext.Current.GetFigmaConverters();
        }

        public async Task StartAsync(string file)
        {
            await Task.Run(() =>
            {
                Start(file);
            });
        }

        public void Start(string file, int page = 0, bool processImages = true)
        {
            Console.WriteLine("[FigmaRemoteFileService] Starting service process..");
            Console.WriteLine($"Reading {file} from resources..");

            ImageVectors.Clear();
            NodesProcessed.Clear();
            ProcessImages = processImages;
            Page = page;
            File = file;

            try
            {
                var template = GetContentTemplate(file);
                Response = AppContext.Current.GetFigmaResponseFromContent(template);

                Console.WriteLine($"Reading successfull");

                Console.WriteLine($"Loading views for page {page}..");

                var canvas = Response.document.children[page];
                foreach (var item in canvas.children)
                    GenerateViewsRecursively(item, null);

                //Images
                OnStartImageProcessing(ImageVectors, file);

                Console.WriteLine("View generation finished.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading resource");
                Console.WriteLine(ex);
            }
        }

        protected abstract void OnStartImageProcessing(Dictionary<FigmaVectorEntity, string> imageVectors, string file);

        protected abstract string GetContentTemplate(string file);

        ProcessedNode GetProcessedNode(FigmaNode currentNode, IEnumerable<CustomViewConverter> customViewConverters, ProcessedNode parent)
        {
            foreach (var customViewConverter in customViewConverters)
            {
                if (customViewConverter.CanConvert(currentNode))
                {
                    var currentView = customViewConverter.ConvertTo(currentNode, parent);
                    var currentElement = new ProcessedNode() { FigmaNode = currentNode, View = currentView, ParentView = parent };
                    return currentElement;
                }

            }
            return null;
        }

        //TODO: This 
        void GenerateViewsRecursively(FigmaNode currentNode, ProcessedNode parent)
        {
            Console.WriteLine("[{0}({1})] Processing {2}..", currentNode?.id, currentNode?.name, currentNode?.GetType());

            var currentProcessedNode = GetProcessedNode(currentNode, CustomViewConverters, parent);

            if (currentProcessedNode == null)
            {
                currentProcessedNode = GetProcessedNode(currentNode, FigmaDefaultConverters, parent);
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

            if (currentNode is IFigmaNodeContainer nodeContainer)
            {
                foreach (var item in nodeContainer.children)
                {
                    GenerateViewsRecursively(item, currentProcessedNode);
                }
            }
        }
    }

    public class FigmaManifestResourceFileService : FigmaFileService
    {
        protected override string GetContentTemplate(string file)
        {
            return AppContext.Current.GetManifestResource(null, file);
        }

        protected override void OnStartImageProcessing(Dictionary<FigmaVectorEntity, string> imageVectors, string file)
        {
          //not needed in local files
        }
    }

    public class FigmaLocalFileService : FigmaFileService
    {
        protected override string GetContentTemplate(string file)
        {
            return System.IO.File.ReadAllText(file);
        }

        protected override void OnStartImageProcessing(Dictionary<FigmaVectorEntity, string> imageVectors, string file)
        {
            //not needed in local files
        }
    }

    public class FigmaRemoteFileService : FigmaFileService
    {
        protected override string GetContentTemplate(string file)
        {
            return AppContext.Current.GetFigmaFileContent(file, AppContext.Current.Token);
        }

        protected override void OnStartImageProcessing(Dictionary<FigmaVectorEntity, string> imageVectors, string file)
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
}
