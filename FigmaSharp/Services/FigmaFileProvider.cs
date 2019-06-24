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

namespace FigmaSharp.Services
{
    public interface IFigmaFileProvider
    {
        bool NeedsImageLinks { get; }
        event EventHandler ImageLinksProcessed;
        List<FigmaNode> Nodes { get; }
        FigmaResponse Response { get; }
        void Load(string path);
        void Save(string filePath);
        string GetContentTemplate(string file);
        void OnStartImageLinkProcessing(Dictionary<FigmaVectorEntity, string> imageVectors, string file);
    }

    public class FigmaLocalFileProvider : FigmaFileProvider
    {
        public override string GetContentTemplate(string file)
        {
            return System.IO.File.ReadAllText(file);
        }

        public override void OnStartImageLinkProcessing(Dictionary<FigmaVectorEntity, string> imageVectors, string file)
        {
            //not needed in local files
        }
    }

    public class FigmaRemoteFileProvider : FigmaFileProvider
    {
        public override bool NeedsImageLinks => true;

        public override string GetContentTemplate(string file)
        {
            return AppContext.Current.GetFigmaFileContent(file, AppContext.Current.Token);
        }

        public override void OnStartImageLinkProcessing(Dictionary<FigmaVectorEntity, string> imageVectors, string file)
        {
            if (imageVectors.Count == 0)
            {
                return;
            }

            Task.Run(() => {
                //Remote files need get the real image url to get the file
                var vectorsIds = imageVectors.Select(s => s.Key.id);

                //TODO: figma url has a limited character in urls we fixed the limit to 10 ids's for each call
                var numberLoop = (vectorsIds.Count() / CallNumber) + 1;

                Console.WriteLine("Starting image link processing {0}.", numberLoop);
                for (int i = 0; i < numberLoop; i++)
                {
                    var vectors = vectorsIds.Skip(i * CallNumber).Take(CallNumber);
                    Console.WriteLine("[{0}/{1}] detected {2} ", i, numberLoop, vectors.Count());
                    var figmaImageResponse = FigmaApiHelper.GetFigmaImages(file, vectors);
                    if (figmaImageResponse != null)
                    {
                        string keys = string.Empty;
                        foreach (var imageResponse in figmaImageResponse.images)
                        {
                            var image = imageVectors.FirstOrDefault(s => s.Key.id == imageResponse.Key).Key;
                            imageVectors[image] = imageResponse.Value;
                            keys += imageResponse.Key + " ";

                        }
                        Console.WriteLine(keys);
                    }
                    Thread.Sleep(100);
                }

                Console.WriteLine("Ended image link processing");

                OnImageLinkProcessed();
            });
        }
        const int CallNumber = 200;
    }

    public class FigmaManifestFileProvider : FigmaFileProvider
    {
        public Assembly Assembly { get; set; }

        public override string GetContentTemplate(string file)
        {
            return AppContext.Current.GetManifestResource(Assembly, file);
        }

        public override void OnStartImageLinkProcessing(Dictionary<FigmaVectorEntity, string> imageVectors, string file)
        {
            //not needed in local files
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

        public void Load(string path)
        {
            ImageProcessed = false;
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
                Console.WriteLine($"Error reading remote resources. Ensure you added NewtonSoft nuget?");
                Console.WriteLine(ex);
            }
        }

        void ProcessNodeRecursively(FigmaNode node, FigmaNode parent)
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

        public abstract void OnStartImageLinkProcessing(Dictionary<FigmaVectorEntity, string> imageVectors, string file);

        public void Save(string filePath)
        {
            AppContext.Current.SetFigmaResponseFromContent(Response, filePath);
        }
    }
}
