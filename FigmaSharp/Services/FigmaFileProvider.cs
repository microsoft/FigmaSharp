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
        void OnStartImageLinkProcessing(List<ImageProcessed> imageVectors, string file);
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

        public override void OnStartImageLinkProcessing(List<ImageProcessed> imageVectors, string file)
        {
            //not needed in local files
            Console.WriteLine($"Loading images..");
           
            if (imageVectors.Count > 0)
            {
                foreach (var vector in imageVectors)
                {
                    try
                    {
                        var recoveredKey = FigmaResourceConverter.FromResource(vector.Node.id);
                        string filePath = Path.Combine(ResourcesDirectory, string.Concat(recoveredKey, ImageFormat));

                        if (!System.IO.File.Exists(filePath))
                        {
                            throw new FileNotFoundException(filePath);
                        }
                        vector.Url = filePath;
                        vector.Image = AppContext.Current.GetImageFromFilePath(filePath);
                        vector.ViewWrapper?.SetImage(vector.Image);
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

        public override void OnStartImageLinkProcessing(List<ImageProcessed> imageVectors, string file)
        {
            if (imageVectors.Count == 0)
            {
                return;
            }

            Task.Run(() => {
                //Remote files need get the real image url to get the file
                //var vectorsIds = imageVectors.Select(s => s.Node.id);

                var totalImages = imageVectors.Count();
                //TODO: figma url has a limited character in urls we fixed the limit to 10 ids's for each call
                var numberLoop = (totalImages / CallNumber) + 1;

                Console.WriteLine("Detected a total of {0} possible images.  ", totalImages);
                for (int i = 0; i < numberLoop; i++)
                {
                    var vectors = imageVectors.Skip(i * CallNumber).Take(CallNumber);
                    Console.WriteLine("[{0}/{1}] Processing Images ... {2} ", i, numberLoop, vectors.Count());
                    var figmaImageResponse = FigmaApiHelper.GetFigmaImages(file, vectors.Select(s => s.Node.id));
                    if (figmaImageResponse != null)
                    {
                        foreach (var imageResponse in figmaImageResponse.images)
                        {
                            var vector = vectors.FirstOrDefault(s => s.Node.id == imageResponse.Key);
                            Console.Write("[{0}:{1}:{2}] {3}...", vector.Node.GetType (), vector.Node.id, vector.Node.name, imageResponse.Value);
                            if (imageResponse.Value != null)
                            {
                                vector.Url = imageResponse.Value;
                                vector.Image = AppContext.Current.GetImage(vector.Url);

                                AppContext.Current.BeginInvoke(() =>
                                {
                                    vector.ViewWrapper?.SetImage(vector.Image);
                                });
                                Console.Write("OK \n");
                            } else
                            {
                                Console.Write("NULL IMAGE \n");
                            }
                            //Thread.Sleep(30);
                        }
                    }
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

        public override void OnStartImageLinkProcessing(List<ImageProcessed> imageVectors, string file)
        {
            Console.WriteLine($"Loading images..");

            if (imageVectors.Count > 0)
            {
                foreach (var vector in imageVectors)
                {
                    var recoveredKey = FigmaResourceConverter.FromResource(vector.Node.id);
                    var image = AppContext.Current.GetImageFromManifest(Assembly, recoveredKey);
                    vector.Image = image;
                    vector.Url = recoveredKey;
                    vector.ViewWrapper?.SetImage(vector.Image);
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

                var template = GetContentTemplate(file);

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

        public abstract void OnStartImageLinkProcessing(List<ImageProcessed> imageVectors, string file);

        public void Save(string filePath)
        {
            AppContext.Current.SetFigmaResponseFromContent(Response, filePath);
        }
    }
}
