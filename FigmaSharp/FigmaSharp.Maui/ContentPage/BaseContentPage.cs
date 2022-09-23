// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FigmaSharp;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;

namespace Microsoft.Maui.Controls
{
    public abstract class BaseContentPage : ContentPage
    {
        public string FileName { get; private set; }

        protected NodeProvider FileProvider { get; set; }

        protected ViewRenderService RendererService { get; set; }

        protected List<NodeConverter> Converters => RendererService.CustomConverters;

        FigmaSharp.Views.IView view;
        public FigmaSharp.Views.IView ContentView
        {
            get => view;
            set
            {
                this.view = value;
                Content = value.NativeObject as View;
            }
        }

        public string StartNodeID => FileProvider.Nodes.OfType<FigmaCanvas>().FirstOrDefault()?.prototypeStartNodeID;

        public Task LoadDocumentAsync(string fileName)
        {
            FileName = fileName;
            return FileProvider.LoadAsync(fileName);
        }

        public void AddConverter(params NodeConverter[] converters)
        {
            RendererService.CustomConverters.AddRange(converters);
        }

        public void RemoveConverter(params NodeConverter[] converters)
        {
            foreach (var item in converters)
                RendererService.CustomConverters.Remove(item);
        }
      
        public abstract void InitializeFigmaComponent();
        protected abstract NodeConverter[] GetFigmaViewConverters();

        protected void InternalInitializeComponent()
        {
            try
            {
                GetType().GetMethod("InitializeComponent", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(this, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public T FindNativeViewByName<T>(string name) where T : View
        {
            return RendererService.FindNativeViewByName<T>(name);
        }

        public void RenderByPath(params string[] path) =>
            RenderByPath(new ViewRenderServiceOptions (), path);

        public void RenderByPath(ViewRenderServiceOptions options, params string[] path)
        {
            var mainScreen = RendererService.RenderByPath<FigmaSharp.Views.IView>(options, null, path);
            ContentView = mainScreen;
        }

        public void RenderByName(string name, ViewRenderServiceOptions options)
        {
            var mainScreen = RendererService.RenderByName<FigmaSharp.Views.IView>(name, null, options);
            if (mainScreen == null) return;
            ContentView = mainScreen;
        }

        public void RenderByName(string name) =>
            RenderByNode(FileProvider.FindByName(name));

        public void RenderByNode(FigmaNode node, ViewRenderServiceOptions options)
        {
            var contentView = RendererService.RenderByNode<FigmaSharp.Views.IView>(node, null, options);
            ContentView = contentView;
        }

        public void RenderByNode(FigmaNode node) =>
            RenderByNode(node, new ViewRenderServiceOptions());

        public void RenderByNodeId(string nodeId, ViewRenderServiceOptions options)
        {
            var selectedNode = RendererService.FindNodeById(nodeId);
            if (selectedNode == null) return;
            RenderByNode(selectedNode, options);
        }

        public void RenderByNodeId(string nodeId) =>
            RenderByNodeId(nodeId, new ViewRenderServiceOptions());
    }
}
