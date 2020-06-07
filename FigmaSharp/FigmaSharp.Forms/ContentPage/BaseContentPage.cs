using System;
using System.Collections.Generic;
using FigmaSharp;
using FigmaSharp.Models;
using System.Linq;
using System.Reflection;
using FigmaSharp.Converters;
using FigmaSharp.Services;

namespace Xamarin.Forms
{
    public abstract class BaseContentPage : ContentPage
    {
        public string FileName { get; private set; }

        protected NodeProvider FileProvider { get; set; }

        protected ViewRenderService RendererService { get; set; }

        protected List<NodeConverter> Converters => RendererService.CustomConverters;

        FigmaSharp.Views.IView contentView;
        public FigmaSharp.Views.IView ContentView
        {
            get => contentView;
            set
            {
                contentView = value;
                Content = contentView.NativeObject as AbsoluteLayout;
            }
        }

        public string StartNodeID => FileProvider.Nodes.OfType<FigmaCanvas>().FirstOrDefault()?.prototypeStartNodeID;

        public void LoadDocument(string fileName)
        {
            FileName = fileName;
            FileProvider.Load(fileName);
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

        public T FindNativeViewByName<T>(string name) where T : Xamarin.Forms.View
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
            ContentView = new FigmaSharp.Views.Forms.View();
            RendererService.RenderByNode<FigmaSharp.Views.IView>(node, ContentView, options);
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
