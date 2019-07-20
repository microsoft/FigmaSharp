using System;
using System.Collections.Generic;
using FigmaSharp;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views.Forms;
using System.Linq;
using System.Reflection;

namespace Xamarin.Forms
{
    public class FigmaRemoteContentPage : FigmaContentPage
    {
        public override void InitializeFigmaComponent()
        {
            InternalInitializeComponent();
            FileProvider = new FigmaRemoteFileProvider();
            RendererService = new FigmaViewRendererService(FileProvider, GetFigmaViewConverters ());
        }

        protected override FigmaViewConverter[] GetFigmaViewConverters()
        {
            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();
            return converters;
        }
    }

    public class FigmaAssemblyResourceContentPage : FigmaContentPage
    {
        public override void InitializeFigmaComponent()
        {
            InternalInitializeComponent();
            FileProvider = new FigmaManifestFileProvider(Assembly.GetCallingAssembly ());
            RendererService = new FigmaViewRendererService(FileProvider, GetFigmaViewConverters());
        }

        protected override FigmaViewConverter[] GetFigmaViewConverters()
        {
            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();
            return converters;
        }
    }

    public class FigmaLocalContentPage : FigmaContentPage
    {
        public override void InitializeFigmaComponent()
        {
            InternalInitializeComponent();
            FileProvider = new FigmaLocalFileProvider("Resources");
            RendererService = new FigmaViewRendererService(FileProvider, GetFigmaViewConverters());
        }

        protected override FigmaViewConverter[] GetFigmaViewConverters()
        {
            var converters = FigmaSharp.AppContext.Current.GetFigmaConverters();
            return converters;
        }
    }

    public abstract class FigmaContentPage : ContentPage
    {
        public string FileName { get; private set; }

        protected FigmaFileProvider FileProvider { get; set; }

        protected FigmaViewRendererService RendererService { get; set; }

        protected List<FigmaViewConverter> Converters => RendererService.CustomConverters;

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

        public void AddConverter(params FigmaViewConverter[] converters)
        {
            RendererService.CustomConverters.AddRange(converters);
        }

        public void RemoveConverter(params FigmaViewConverter[] converters)
        {
            foreach (var item in converters)
                RendererService.CustomConverters.Remove(item);
        }
      
        public abstract void InitializeFigmaComponent();
        protected abstract FigmaViewConverter[] GetFigmaViewConverters();

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
            RenderByPath(new FigmaViewRendererServiceOptions(), path);

        public void RenderByPath(FigmaViewRendererServiceOptions options, params string[] path)
        {
            var mainScreen = RendererService.RenderByPath<FigmaSharp.Views.IView>(options, path);
            ContentView = mainScreen;
        }

        public void RenderByName(string name, FigmaViewRendererServiceOptions options)
        {
            var mainScreen = RendererService.RenderByName<FigmaSharp.Views.IView>(name, options);
            if (mainScreen == null) return;
            ContentView = mainScreen;
        }

        public void RenderByName(string name) =>
            RenderByName(name, new FigmaViewRendererServiceOptions());

        public void RenderByNode(FigmaNode node, FigmaViewRendererServiceOptions options)
        {
            var mainScreen = RendererService.RenderByNode<FigmaSharp.Views.IView>(node, options);
            if (mainScreen == null) return;
            ContentView = mainScreen;
        }

        public void RenderByNode(FigmaNode node) =>
            RenderByNode(node, new FigmaViewRendererServiceOptions());

        public void RenderByNodeId(string nodeId, FigmaViewRendererServiceOptions options)
        {
            var selectedNode = RendererService.FindNodeById(nodeId);
            if (selectedNode == null) return;
            RenderByNode(selectedNode, options);
        }

        public void RenderByNodeId(string nodeId) =>
            RenderByNodeId(nodeId, new FigmaViewRendererServiceOptions());
    }
}
