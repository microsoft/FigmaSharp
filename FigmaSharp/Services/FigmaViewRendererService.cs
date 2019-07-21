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
using LiteForms;
using FigmaSharp.Models;

namespace FigmaSharp.Services
{
    public class FigmaRendererService
    {
        protected readonly FigmaViewConverter[] FigmaDefaultConverters;
        protected readonly FigmaViewConverter[] FigmaCustomConverters;

        public List<ProcessedNode> NodesProcessed = new List<ProcessedNode>();
        public readonly List<ProcessedNode> ImageVectors = new List<ProcessedNode>();

        protected IView container;
        protected IFigmaFileProvider fileProvider;

        public T FindViewByName<T>(string name) where T : IView
        {
            foreach (var node in NodesProcessed)
            {
                if (node.View is T && node.FigmaNode.name == name)
                {
                    return (T)node.View;
                }
            }
            return default(T);
        }

		public IEnumerable<T> FindViewsByName<T>(string name)
		{
			foreach (var node in NodesProcessed)
			{
				if (node.View.NativeObject is T && node.FigmaNode.name == name)
				{
					yield return (T)node.View.NativeObject;
				}
			}
		}

		public IView FindViewByName(string name)
        {
            foreach (var node in NodesProcessed)
            {
                if (node.FigmaNode.name == name)
                {
                    return node.View;
                }
            }
            return null;
        }

        public FigmaNode FindNodeByName (string name)
        {
            return fileProvider.Nodes.FirstOrDefault(s => s.name == name);
        }

        public ProcessedNode FindProcessedNodeByName(string name)
        {
            return NodesProcessed.FirstOrDefault(s => s.FigmaNode.name == name);
        }

        FigmaNode GetRecursively (string name, FigmaNode figmaNode)
        {
            if (figmaNode.name == name)
            {
                return figmaNode;
            }

            if (figmaNode is IFigmaNodeContainer container)
            {
                foreach (var item in container.children)
                {
                    var node = GetRecursively(name, item);
                    if (node != null)
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        public FigmaRendererService(IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters)
        {
            this.fileProvider = figmaProvider;
            FigmaDefaultConverters = figmaViewConverters.Where(s => s.IsLayer).ToArray();
            FigmaCustomConverters = figmaViewConverters.Where(s => !s.IsLayer).ToArray();
        }

        public void ProcessFromNode (FigmaNode figmaNode, IView View, FigmaViewRendererServiceOptions options)
        {
            try
            {
                //in canvas we want calculate the bounds size
                if (figmaNode is FigmaCanvas canvas)
                {
                    //var canvas = fileProvider.Response.document.children.FirstOrDefault();
                    var processedParentView = new ProcessedNode() { FigmaNode = figmaNode, View = View };
                    NodesProcessed.Add(processedParentView);

                    Rectangle contentRect = Rectangle.Zero;
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

                    if (figmaNode is IFigmaNodeContainer container)
                    {
                        foreach (var item in container.children)
                            GenerateViewsRecursively(item, processedParentView, options);
                    }
                } else
                {
                    GenerateViewsRecursively(figmaNode, null, options);
                }

                //Images
                if (options.AreImageProcessed)
                {
                    foreach (var processedNode in NodesProcessed)
                    {
                        if (processedNode.FigmaNode is IFigmaImage figmaImage)
                        {
                            //TODO: this should be replaced by svg
                            if (figmaImage.HasImage())
                            {
                                ImageVectors.Add(processedNode);
                            }
                        }
                    }

                    fileProvider.ImageLinksProcessed += FileProvider_ImageLinksProcessed;
                    fileProvider.OnStartImageLinkProcessing(ImageVectors);
                }

                Console.WriteLine("View generation finished.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading resource");
                Console.WriteLine(ex);
            }
        }

        public void Refresh(FigmaViewRendererServiceOptions options)
        {
            //on refresh we want clear results
            ImageVectors.Clear();
            NodesProcessed.Clear();

            Console.WriteLine($"Reading successfull");

            var canvas = fileProvider.Response.document.children.FirstOrDefault();
            ProcessFromNode (canvas, container, options);
        }

        private void FileProvider_ImageLinksProcessed(object sender, EventArgs e)
        {
            Console.WriteLine($"Image Links ended.");
        }

        protected CustomViewConverter GetProcessedConverter(FigmaNode currentNode, IEnumerable<CustomViewConverter> customViewConverters, ProcessedNode parent, FigmaViewRendererServiceOptions options)
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
        protected void GenerateViewsRecursively(FigmaNode currentNode, ProcessedNode parent, FigmaViewRendererServiceOptions options)
        {
            Console.WriteLine("[{0}.{1}] Processing {2}..", currentNode?.id, currentNode?.name, currentNode?.GetType());

            bool scanChildren = true;

            var converter = GetProcessedConverter(currentNode, FigmaCustomConverters, parent, options);

            if (converter == null)
            {
                converter = GetProcessedConverter(currentNode, FigmaDefaultConverters, parent, options);
            }

            ProcessedNode currentProcessedNode = null;
            if (converter != null)
            {
                var currentView = options.IsToViewProcessed ? converter.ConvertTo(currentNode, parent) : null;
                currentProcessedNode = new ProcessedNode() { FigmaNode = currentNode, View = currentView, ParentView = parent };
                NodesProcessed.Add(currentProcessedNode);

                //TODO: this need to be improved, handles special cases for native controls
                scanChildren = currentNode is FigmaInstance && options.ScanChildrenFromFigmaInstances ? true : converter.ScanChildren(currentNode);
            }
            else
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

    public class FigmaFileRendererService : FigmaRendererService
    {
        public FigmaFileRendererService(IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters) : base (figmaProvider, figmaViewConverters)
        {
        }

        public Task StartAsync(string file, IView container) => StartAsync(file, container, new FigmaViewRendererServiceOptions());
        public Task StartAsync(string file, IView container, FigmaViewRendererServiceOptions options) => Task.Run(() => Start(file, container, options: options));

        public void Start(string file, IView container) =>
            Start(file, container, new FigmaViewRendererServiceOptions());

        public void Start(string file, IView container, FigmaViewRendererServiceOptions options)
        {
            Console.WriteLine("[FigmaRemoteFileService] Starting service process..");
            Console.WriteLine($"Reading {file} from resources..");

            this.container = container;

            try
            {
                if (options.LoadFileProvider)
                {
                    fileProvider.Load(file);
                }
                Refresh(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading resource");
                Console.WriteLine(ex);
            }
        }
    }

    public class FigmaViewRendererService : FigmaRendererService
    {
        public FigmaViewRendererService(IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters) : base(figmaProvider, figmaViewConverters)
        {
        }

        public T RenderByName<T>(string figmaName) where T : IView
        {
            return RenderByName <T>(figmaName, new FigmaViewRendererServiceOptions());
        }

        public T RenderByName<T>(string figmaName, FigmaViewRendererServiceOptions options) where T: IView
        {
            var node = FindNodeByName(figmaName);
            if (node == null)
                return default (T);
            ProcessFromNode (node, null, options);

            var processedNode = FindProcessedNodeByName(figmaName);
            Recursively(processedNode);
            return (T) processedNode.View;
        }

        void Recursively(ProcessedNode parentNode)
        {
            var children = NodesProcessed.Where(s => s.ParentView == parentNode);
            foreach (var child in children)
            {
                if (child.FigmaNode is IAbsoluteBoundingBox absoluteBounding && parentNode.FigmaNode is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
                {
                    parentNode.View.AddChild(child.View);

                    var x = Math.Max(absoluteBounding.absoluteBoundingBox.X - parentAbsoluteBoundingBox.absoluteBoundingBox.X, 0);
                    float y;
                    if (AppContext.Current.IsVerticalAxisFlipped)
                    {
                        var parentY = parentAbsoluteBoundingBox.absoluteBoundingBox.Y + parentAbsoluteBoundingBox.absoluteBoundingBox.Height;
                        var actualY = absoluteBounding.absoluteBoundingBox.Y + absoluteBounding.absoluteBoundingBox.Height;
                        y = parentY - actualY;
                    }
                    else
                    {
                        y = absoluteBounding.absoluteBoundingBox.Y - parentAbsoluteBoundingBox.absoluteBoundingBox.Y;
                    }

                    child.View.SetAllocation(x, y, Math.Max(absoluteBounding.absoluteBoundingBox.Width, 1), Math.Max(1, absoluteBounding.absoluteBoundingBox.Height));
                }

                Recursively(child);
            }
        }

        public void Start(string figmaName, IView container, FigmaViewRendererServiceOptions options)
        {
            Console.WriteLine("[FigmaViewRenderer] Starting process..");
            Console.WriteLine($"Reading {figmaName} from resources..");

            this.container = container;

            try
            {
                if (options.LoadFileProvider)
                    fileProvider.Load(fileProvider.File);
                Refresh(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading resource");
                Console.WriteLine(ex);
            }
        }
    }

    public class FigmaViewRendererServiceOptions
    {
        public bool IsToViewProcessed { get; set; } = true;
        public bool AreImageProcessed { get; set; } = true;
        public int StartPage { get; set; } = 0;

        public bool LoadFileProvider { get; set; } = true;

        /// <summary>
        /// Allows configure in rederer process all children subviews from FigmaInstances (Components)
        /// </summary>
        public bool ScanChildrenFromFigmaInstances { get; set; } = true;
    }
}
