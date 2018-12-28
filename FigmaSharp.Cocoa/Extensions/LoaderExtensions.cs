using System;
using System.Collections.Generic;
using System.IO;
using AppKit;
using CoreGraphics;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace FigmaSharp
{
    public static class LoaderExtensions
    {
        //public static void LoadFigmaFromFilePath(this NSWindow window, string filePath, out List<IImageViewWrapper> figmaImageViews, int page = 0)
        //{
        //    figmaImageViews = new List<IImageViewWrapper>();
        //    var figmaResponse = FigmaApiHelper.GetFigmaDialogFromFilePath(filePath);
        //    if (page < 0 || page > figmaResponse.document.children.Length -1)
        //    {
        //        throw new IndexOutOfRangeException("page is not in range");
        //    }
        //    var canvas = figmaResponse.document.children[page];
        //    if (canvas.absoluteBoundingBox != null)
        //    {
        //        window.SetFrame(new CGRect(window.Frame.X, window.Frame.Y, canvas.absoluteBoundingBox.width, canvas.absoluteBoundingBox.height), true);
        //    }
        //    LoadFigma(window.ContentView, new FigmaFrameEntityResponse(filePath, figmaResponse), figmaImageViews);
        //}

        //public static void LoadFigmaFromUrlFile(this NSWindow window, string urlFile, out List<IImageViewWrapper> figmaImageViews, int page = 0)
        //{
        //    figmaImageViews = new List<IImageViewWrapper>();
        //    var figmaResponse = FigmaApiHelper.GetFigmaDialogFromUrlFile(urlFile);
        //    if (page < 0 || page > figmaResponse.document.children.Length)
        //    {
        //        throw new IndexOutOfRangeException("page is not in range");
        //    }
        //    var canvas = figmaResponse.document.children[page];
        //    var boundingBox = canvas.absoluteBoundingBox;
        //    window.SetFrame(new CGRect(window.Frame.X, window.Frame.Y, boundingBox.width, boundingBox.height), true);
        //    LoadFigma(window.ContentView, new FigmaFrameEntityResponse(urlFile, figmaResponse), figmaImageViews);
        //}

        //public static void LoadFigmaFromResource(this NSView contentView, string resource, out List<IImageViewWrapper> figmaImageViews, Assembly assembly = null)
        //{
        //    figmaImageViews = new List<IImageViewWrapper>();
        //    var template = FigmaApiHelper.GetManifestResource(assembly, resource);
        //    var figmaDialog = FigmaApiHelper.GetFigmaResponseFromContent(template);
        //    LoadFigmaFromFrameEntity(contentView, figmaDialog, figmaImageViews);
        //}

        //public static void LoadFigmaFromFilePath(this NSView contentView, string filePath, out List<IImageViewWrapper> figmaImageViews)
        //{
        //    figmaImageViews = new List<IImageViewWrapper>();
        //    var figmaDialog = FigmaApiHelper.GetFigmaDialogFromFilePath(filePath);
        //    LoadFigmaFromFrameEntity(contentView, figmaDialog, figmaImageViews);
        //}

        public static void LoadFigmaFromContent(this NSView contentView, string figmaContent, out List<IImageViewWrapper> figmaImageViews, int page = 0)
        {
            figmaImageViews = new List<IImageViewWrapper>();
            var response = FigmaApiHelper.GetFigmaResponseFromContent(figmaContent);
            LoadFigmaFromFrameEntity(contentView, response, figmaImageViews);
        }

        //public static void LoadFigmaFromUrlFile(this NSView contentView, string urlFile, out List<IImageViewWrapper> figmaImageViews)
        //{
        //    figmaImageViews = new List<IImageViewWrapper>();
        //    var figmaDialog = FigmaApiHelper.GetFigmaDialogFromUrlFile(urlFile);
        //    contentView.LoadFigmaFromFrameEntity(figmaDialog, figmaImageViews);
        //}

        public static void LoadFigmaFromFrameEntity(this NSView view, FigmaResponse figmaResponse, List<IImageViewWrapper> figmaImageViews, int page = 0)
        {
            if (figmaResponse != null)
            {
                LoadFigma(view, new FigmaFrameEntityResponse(figmaResponse, page), figmaImageViews);
            }
            else
            {
                var alert = new NSAlert();
                alert.MessageText = string.Format("You figma file does not have a view name:'{0}'");
                alert.AddButton("Close");
                alert.RunModal();
            }
        }

        public static void LoadFromLocalImageResources(this List<IImageViewWrapper> figmaImageViews, Assembly assembly = null)
        {
            for (int i = 0; i < figmaImageViews.Count; i++)
            {
                try
                {
                    var image = AppContext.Current.GetImageFromManifest(assembly, figmaImageViews[i].Data.imageRef);
                    figmaImageViews[i].SetImage(image);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static void LoadFromResourceImageDirectory(this List<IImageViewWrapper> figmaImageViews, string resourcesDirectory, string format = ".png")
        {
            for (int i = 0; i < figmaImageViews.Count; i++)
            {
                try
                {
                    string filePath = Path.Combine(resourcesDirectory, string.Concat(figmaImageViews[i].Data.imageRef, format));
                    if (!File.Exists(filePath))
                    {
                        throw new FileNotFoundException(filePath);
                    }
                    figmaImageViews[i].SetImage(AppContext.Current.GetImageFromFilePath(filePath));
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

        //public static void Load(this IEnumerable<IImageViewWrapper> figmaImageViews, string fileId)
        //{
        //    var ids = figmaImageViews.Select(s => s.Data.ID).ToArray();
        //    var images = FigmaApiHelper.GetFigmaImages(fileId, ids);

        //    if (images != null)
        //    {

        //        List<Task> downloadImageTaks = new List<Task>();
        //        foreach (var imageView in figmaImageViews)
        //        {

        //            Task.Run(() => {
        //                var url = images.images[imageView.Data.ID];
        //                Console.WriteLine($"Processing image - ID:[{imageView.Data.ID}] ImageRef:[{imageView.Data.imageRef}] Url:[{url}]");
        //                try
        //                {
        //                    var image = AppContext.Current.GetImage(url);
        //                    NSApplication.SharedApplication.InvokeOnMainThread(() => {
        //                        imageView.SetImage(image);
        //                    });
        //                    Console.WriteLine($"[SUCCESS] Processing image - ID:[{imageView.Data.ID}] ImageRef:[{imageView.Data.imageRef}] Url:[{url}]");
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine($"[ERROR] Processing image - ID:[{imageView.Data.ID}] ImageRef:[{imageView.Data.imageRef}] Url:[{url}]");
        //                    Console.WriteLine(ex);
        //                }

        //            });
        //        }
        //    }
        //}

        public static void LoadFigma(this NSView contentView, FigmaFrameEntityResponse frameEntityResponse, List<IImageViewWrapper> figmaImageViews = null)
        {
            //clean views from current container
            var views = contentView.Subviews;
            foreach (var item in views)
            {
                item.RemoveFromSuperview();
            }
            contentView.RemoveConstraints(contentView.Constraints);


            var canvas = frameEntityResponse.PageContent;

            //Figma doesn't calculate the bounds of our first level
            canvas.CalculateBounds();

            contentView.WantsLayer = true;
            var backgroundColor = canvas.backgroundColor.ToNSColor();
            contentView.Layer.BackgroundColor = backgroundColor.CGColor;

            var figmaView = canvas as FigmaNode;
            //var mainView = figmaView.ToViewWrapper(new ViewWrapper (contentView), figmaView);
            //if (mainView != null) {
            //    contentView.AddSubview(mainView.NativeObject as NSView);
            //}
        }
    }
}
