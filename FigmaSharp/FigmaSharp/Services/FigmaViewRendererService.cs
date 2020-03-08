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
using FigmaSharp.Views;
using FigmaSharp.Models;

namespace FigmaSharp.Services
{
    public class FigmaViewRendererService : IDisposable
    {
        public FigmaViewPropertySetterBase PropertySetter { get; }

        protected readonly List<FigmaViewConverter> DefaultConverters;
        public readonly List<FigmaViewConverter> CustomConverters;

        public List<ProcessedNode> NodesProcessed = new List<ProcessedNode>();
        public readonly List<ProcessedNode> ImageVectors = new List<ProcessedNode>();

        protected ProcessedNode firstProcesedNode;
        protected IFigmaFileProvider fileProvider;
        internal IFigmaFileProvider FileProvider => fileProvider;

        public FigmaViewRendererService (IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters = null, FigmaViewPropertySetterBase propertySetter = null)
        {
            this.fileProvider = figmaProvider;
            this.fileProvider.ImageLinksProcessed += FileProvider_ImageLinksProcessed;
            this.PropertySetter = propertySetter ?? AppContext.Current.GetPropertySetter();

            if (figmaViewConverters == null) {
                figmaViewConverters = AppContext.Current.GetFigmaConverters();
            }
          
            DefaultConverters = figmaViewConverters.Where(s => s.IsLayer)
                .ToList();
            CustomConverters = figmaViewConverters.Where(s => !s.IsLayer)
                .ToList();
        }

        #region Search

        public T FindViewStartsWith<T>(string name, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase) where T : IView
		{
			foreach (var node in NodesProcessed)
			{
				if (node.View is T && node.FigmaNode.name.StartsWith(name, stringComparison))
				{
					return (T)node.View;
				}
			}
			return default(T);
		}
		
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

        virtual internal bool IsFirstNode(FigmaNode currentNode)
        {
            return currentNode is FigmaCanvas || currentNode.Parent is FigmaCanvas;
        }

        public T FindViewByPath<T>(params string[] path) where T : IView
		{
			var node = fileProvider.FindByPath(path);
			if (node == null)
				return default(T);
			var processed = NodesProcessed.FirstOrDefault(s => s.FigmaNode == node);
			if (processed == null)
				return default(T);

			return (T) processed.View;
		}

		public IEnumerable<T> FindViewsStartsWith<T>(string name, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
		{
			foreach (var node in NodesProcessed)
			{
				if (node.View is T && node.FigmaNode.name.StartsWith(name, stringComparison))
				{
					yield return (T)node.View;
				}
			}
		}

		public IEnumerable<T> FindViewsByName<T>(string name)
		{
			foreach (var node in NodesProcessed)
			{
				if (node.View is T && node.FigmaNode.name == name)
				{
					yield return (T)node.View;
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

		public FigmaNode FindNodeById(string id)
		{
			return fileProvider.Nodes.FirstOrDefault(s => s.id == id);
		}

		public ProcessedNode FindProcessedNodeByName(string name)
        {
            return NodesProcessed.FirstOrDefault(s => s.FigmaNode.name == name);
        }

		public ProcessedNode FindProcessedNodeById(string Id)
		{
			return NodesProcessed.FirstOrDefault(s => s.FigmaNode.id == Id);
		}

        //TODO: Move like extended method
		FigmaNode GetRecursively (string name, FigmaNode figmaNode)
        {
            if (figmaNode.name == name) {
                return figmaNode;
            }

            if (figmaNode is IFigmaNodeContainer container) {
                foreach (var item in container.children) {
                    var node = GetRecursively(name, item);
                    if (node != null)
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        #endregion

		#region Render Initialization

		public void RenderInWindow(IWindow mainWindow, FigmaViewRendererServiceOptions options = null)
        {
            var allCanvas = fileProvider.Nodes
                .OfType<FigmaCanvas>()
                .ToArray();
            if (allCanvas.Length == 0)
            {
                return;
            }

            var startPage = options != null ? options.StartPage : 0;
            var canvas = allCanvas[startPage];
            if (canvas != null)
            {
                canvas.absoluteBoundingBox = canvas.GetCurrentBounds();
                RenderInWindow(mainWindow, canvas, options);
            }
        }

        public void RenderInWindow(IWindow mainWindow, string nodeName, FigmaViewRendererServiceOptions options = null)
        {
            var node = fileProvider.Nodes
                .FirstOrDefault(s => s.name == nodeName || (s.TryGetNodeCustomName(out string name) && name == nodeName));

            if (node == null)
                throw new Exception($"nodename {nodeName} not found");
            RenderInWindow(mainWindow, node, options);
        }

        public virtual void RenderInWindow(IWindow mainWindow, FigmaNode node, FigmaViewRendererServiceOptions options = null)
        {
            if (node is IAbsoluteBoundingBox bounNode)
            {
                mainWindow.Size = new Size(bounNode.absoluteBoundingBox.Width, bounNode.absoluteBoundingBox.Height);
            }

            if (options == null)
            {
                options = new FigmaViewRendererServiceOptions();
            }

            StartFromNode(node, mainWindow.Content, options);

            var processedNode = FindProcessedNodeById(node.id);
            RecursivelyConfigureViews(processedNode, options);
        }

        void RecursivelyConfigureViews(ProcessedNode parentNode, FigmaViewRendererServiceOptions options)
        {
            var children = NodesProcessed.Where(s => s.ParentView == parentNode);
            foreach (var child in children)
            {
                if (child.View == null)
                {
                    Console.WriteLine("Node {0} has no view to process... skipping", child.FigmaNode);
                    continue;
                }

                PropertySetter.Configure(CodeProperties.AddChild, child.View, child.FigmaNode, parentNode.View, parentNode.FigmaNode, this);

                PropertySetter.Configure(CodeProperties.Frame, child.View, child.FigmaNode, parentNode.View, parentNode.FigmaNode, this);

                if (!IsFirstNode(child.FigmaNode) || options.ConstraintsInMainViews)
                {
                    PropertySetter.Configure(CodeProperties.Constraints, child.View, child.FigmaNode, parentNode.View, parentNode.FigmaNode, this);
                }

                RecursivelyConfigureViews(child, options);
            }
        }

        //public void Start (IView container, FigmaViewRendererServiceOptions options = null)
        //{
        //    if (options == null) {
        //        options = new FigmaViewRendererServiceOptions();
        //    }


        //    this.container = container;

        //    try
        //    {
        //        if (options.LoadFileProvider)
        //            fileProvider.Load(fileProvider.File);

        //        //we generate all the processed nodes
        //        Refresh(options);

        //        //we render only if there is a canvas and GenerateViews is enabled
        //        //var canvas = NodesProcessed.FirstOrDefault(s => s.FigmaNode is FigmaCanvas);
        //        if (firstNode != null && options.ConfigureViews)
        //        {
        //            RecursivelyConfigureViews(canvas, options);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error reading resource");
        //        Console.WriteLine(ex);
        //    }
        //}

        public void Start(IView contentView, string figmaName = null, FigmaViewRendererServiceOptions options = null)
        {
            FigmaCanvas canvas;
            if (options.StartPage >= 0 && options.StartPage <= fileProvider.Response.document.children.Length) {
                canvas = fileProvider.Response.document.children[options.StartPage];
            }
            else {
                canvas = fileProvider.Response.document.children.FirstOrDefault();
            }

            StartFromNode(canvas, contentView, options);
        }

        public void StartFromNode(FigmaNode figmaNode, IView contentView, FigmaViewRendererServiceOptions options)
        {
            firstProcesedNode = new ProcessedNode() { FigmaNode = figmaNode, View = contentView };

            if (figmaNode is FigmaCanvas figmaCanvas) {
                //in canvas we want calculate the bounds size
                figmaCanvas.absoluteBoundingBox = figmaCanvas.GetCurrentBounds();
            }

            foreach (var item in GetNodeChildren(figmaNode, null, options)) {
                GenerateViewsRecursively(item, firstProcesedNode, options);
            }

            try {
                //Images
                if (options.AreImageProcessed) {
                    foreach (var processedNode in NodesProcessed) {
                        if (ProcessesImageFromNode(processedNode.FigmaNode)) {
                            ImageVectors.Add(processedNode);
                        }
                    }

                    fileProvider.OnStartImageLinkProcessing(ImageVectors);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading resource");
                Console.WriteLine(ex);
            }
        }

        public void Refresh(FigmaViewRendererServiceOptions options)
        {
            if (firstProcesedNode == null) {
                throw new Exception("cannot refresh without a previous renderer");
            }

            //on refresh we want clear results
            ImageVectors.Clear();
            NodesProcessed.Clear();

            Console.WriteLine($"Reading successfull");

            StartFromNode(firstProcesedNode.FigmaNode, firstProcesedNode.View, options);
        }

        #endregion

        #region Rendering Methods

        public T RenderByFullPath<T>(IView parent, FigmaViewRendererServiceOptions options, string path) where T : IView
        {
            FigmaNode node = fileProvider.FindByPath(path);
            if (node == null)
                return default(T);
            return (T)RenderByNode(node, parent, options);
        }

        public T RenderByPath<T>(FigmaViewRendererServiceOptions options, IView parent, params string[] path) where T : IView
        {
            FigmaNode node = fileProvider.FindByPath(path);
            if (node == null)
                return default(T);
            return (T)RenderByNode(node, parent, options);
        }

        FigmaNode firstNode;

        public IView RenderByNode(FigmaNode node, IView parent, FigmaViewRendererServiceOptions options = null)
        {
            if (options == null)
                options = new FigmaViewRendererServiceOptions();

            firstNode = node;

            StartFromNode(node, parent, options);
            var processedNode = FindProcessedNodeById(node.id);
            RecursivelyConfigureViews(processedNode, options);

            firstNode = null;

            return processedNode.View;
        }

        public T RenderByNode<T>(FigmaNode node, IView parent, FigmaViewRendererServiceOptions options = null) where T : IView
        {
            return (T)RenderByNode<T>(node, parent, options);
        }

        public T RenderByName<T>(string figmaName, IView parent, FigmaViewRendererServiceOptions options = null) where T : IView
        {
            var node = FindNodeByName(figmaName);
            if (node == null)
                return default(T);
            return (T)RenderByNode(node, parent, options);
        }

        #endregion

        public virtual bool ProcessesImageFromNode (FigmaNode node)
        {
           return node.IsImageNode ();
        }

        void FileProvider_ImageLinksProcessed(object sender, EventArgs e)
        {
            Console.WriteLine($"Image Links ended.");
        }

        protected CustomViewConverter GetProcessedConverter(FigmaNode currentNode, IEnumerable<CustomViewConverter> customViewConverters)
        {
            foreach (var customViewConverter in customViewConverters) {
                if (customViewConverter.CanConvert(currentNode)) {
                    return customViewConverter;
                }
            }
            return null;
        }

		protected virtual bool SkipsNode (FigmaNode currentNode, ProcessedNode parent, FigmaViewRendererServiceOptions options)
		{
            if (options != null && options.ToIgnore != null && options.ToIgnore.Contains(currentNode))
                return true;
            return  false;
		}

        protected void GenerateViewsRecursively(FigmaNode currentNode, ProcessedNode parent, FigmaViewRendererServiceOptions options)
        {
            Console.WriteLine("[{0}.{1}] Processing {2}..", currentNode?.id, currentNode?.name, currentNode?.GetType());

            if (SkipsNode (currentNode, parent, options))
                return;

            var converter = GetProcessedConverter(currentNode, CustomConverters);

            if (converter == null)
            {
                converter = GetProcessedConverter(currentNode, DefaultConverters);
            }

            ProcessedNode currentProcessedNode = null;
            if (converter != null)
            {
                var currentView = options.IsToViewProcessed ? converter.ConvertTo(currentNode, parent, this) : null;
                currentProcessedNode = new ProcessedNode() { FigmaNode = currentNode, View = currentView, ParentView = parent };
                NodesProcessed.Add(currentProcessedNode);
            }
            else
            {
                Console.WriteLine("[{1}.{2}] There is no Converter for this type: {0}", currentNode.GetType(), currentNode.id, currentNode.name);
            }

            if (NodeScansChildren (currentNode, converter, options))
            {
                foreach (var item in GetNodeChildren (currentNode, parent?.FigmaNode, options)) {
                    GenerateViewsRecursively(item, currentProcessedNode ?? parent, options);
                }
            }
        }

        protected virtual IEnumerable<FigmaNode> GetNodeChildren (FigmaNode currentNode, FigmaNode parentNode, FigmaViewRendererServiceOptions options)
		{
            if (currentNode is IFigmaNodeContainer nodeContainer)
			{
                return nodeContainer.children;
            }
            return Enumerable.Empty<FigmaNode>();
        }

        protected virtual bool NodeScansChildren (FigmaNode currentNode, CustomViewConverter converter, FigmaViewRendererServiceOptions options)
		{
            if (converter == null)
                return false;

			if (!converter.ScanChildren (currentNode)) {
                return false;
			}

			if (!options.ScanChildrenFromFigmaInstances && (currentNode is FigmaInstance || currentNode is FigmaComponentEntity)) {
                return false;
			}

            return true;
        }

		public void Dispose()
		{
			if (this.fileProvider != null) {
                this.fileProvider.ImageLinksProcessed -= FileProvider_ImageLinksProcessed;
            }
		}
	}

    public class FigmaViewRendererServiceOptions
    {
        public bool IsToViewProcessed { get; set; } = true;
        public bool AreImageProcessed { get; set; } = true;
        public int StartPage { get; set; } = 0;

        public FigmaNode[] ToIgnore { get; set; }
        public bool LoadFileProvider { get; set; } = true;

        public bool ConfigureViews { get; set; } = true;

        public bool FrameInMainViews { get; set; } = false;
        public bool ConstraintsInMainViews { get; set; } = false;

        public bool SkipProcessFirstNode { get; set; } = false;

        /// <summary>
        /// Allows configure in rederer process all children subviews from FigmaInstances (Components)
        /// </summary>
        public bool ScanChildrenFromFigmaInstances { get; set; } = true;
    }
}
