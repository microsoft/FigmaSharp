using System;
using System.Collections.Generic;
using System.IO;
using CoreGraphics;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using UIKit;

namespace FigmaSharp
{
    public static class LoaderExtensions
    {
        public static void LoadFigmaFromFilePath(this UIWindow window, string filePath, out List<IImageViewWrapper> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<IImageViewWrapper>();
            var figmaDialog = FigmaApiHelper.GetFigmaDialogFromFilePath(filePath, viewName, nodeName);
            var boundingBox = figmaDialog.absoluteBoundingBox;
            //if (boundingBox != null)
            //{
            //    window.SetFrame(new CGRect(window.Frame.X, window.Frame.Y, boundingBox.width, boundingBox.height), true);
            //}

            var currentView = window.RootViewController.View;
            LoadFigma(currentView, new FigmaFrameEntityResponse(filePath, figmaDialog), figmaImageViews);
        }

        public static void LoadFigmaFromUrlFile(this UIWindow window, string urlFile, out List<IImageViewWrapper> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<IImageViewWrapper>();
            var figmaDialog = FigmaApiHelper.GetFigmaDialogFromUrlFile(urlFile, viewName, nodeName);
            var boundingBox = figmaDialog.absoluteBoundingBox;
            var currentView = window.RootViewController.View;
            //window.SetFrame(new CGRect(window.Frame.X, window.Frame.Y, boundingBox.width, boundingBox.height), true);
            LoadFigma(currentView, new FigmaFrameEntityResponse(urlFile, figmaDialog), figmaImageViews);
        }

        public static void LoadFigmaFromResource(this UIView contentView, string resource, out List<IImageViewWrapper> figmaImageViews, Assembly assembly = null, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<IImageViewWrapper>();
            var template = FigmaApiHelper.GetManifestResource(assembly, resource);
            var figmaDialog = FigmaApiHelper.GetFigmaDialogFromContent(template, viewName, nodeName);
            LoadFigmaFromFrameEntity(contentView, figmaDialog, figmaImageViews, viewName, nodeName);
        }

        public static void LoadFigmaFromFilePath(this UIView contentView, string filePath, out List<IImageViewWrapper> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<IImageViewWrapper>();
            var figmaDialog = FigmaApiHelper.GetFigmaDialogFromFilePath(filePath, viewName, nodeName);
            LoadFigmaFromFrameEntity(contentView, figmaDialog, figmaImageViews, viewName, nodeName);
        }

        public static void LoadFigmaFromContent(this UIView contentView, string figmaContent, out List<IImageViewWrapper> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<IImageViewWrapper>();
            var figmaDialog = FigmaApiHelper.GetFigmaDialogFromContent(figmaContent, viewName, nodeName);
            LoadFigmaFromFrameEntity(contentView, figmaDialog, figmaImageViews, viewName, nodeName);
        }

        public static void LoadFigmaFromUrlFile(this UIView contentView, string urlFile, out List<IImageViewWrapper> figmaImageViews, string viewName = null, string nodeName = null)
        {
            figmaImageViews = new List<IImageViewWrapper>();
            var figmaDialog = FigmaApiHelper.GetFigmaDialogFromUrlFile(urlFile, viewName, nodeName);
            LoadFigmaFromFrameEntity(contentView, figmaDialog, figmaImageViews, viewName, nodeName);
        }

        public static void LoadFigmaFromFrameEntity(this UIView view, IFigmaDocumentContainer figmaView, List<IImageViewWrapper> figmaImageViews, string figmaFileName, string viewName = null, string nodeName = null)
        {
            if (figmaView != null)
            {
                LoadFigma(view, new FigmaFrameEntityResponse(figmaFileName, figmaView), figmaImageViews);
            }
            else
            {
                var alert = new UIAlertView();
                alert.Message = string.Format("You figma file does not have a view name:'{0}'", viewName);
                if (nodeName != null)
                {
                    alert.Message += string.Format(" or node name: '{0}'", nodeName);
                }
                alert.AddButton("Close");
                alert.Show();
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

        public static void Load(this IEnumerable<IImageViewWrapper> figmaImageViews, string fileId)
        {
            var ids = figmaImageViews.Select(s => s.Data.ID).ToArray();
            var images = FigmaApiHelper.GetFigmaImages(fileId, ids);

            if (images != null)
            {

                List<Task> downloadImageTaks = new List<Task>();
                foreach (var imageView in figmaImageViews)
                {

                    Task.Run(() => {
                        var url = images.images[imageView.Data.ID];
                        Console.WriteLine($"Processing image - ID:[{imageView.Data.ID}] ImageRef:[{imageView.Data.imageRef}] Url:[{url}]");
                        try
                        {
                            var image = AppContext.Current.GetImage(url);
                            UIApplication.SharedApplication.InvokeOnMainThread(() => {
                                imageView.SetImage(image);
                            });
                            Console.WriteLine($"[SUCCESS] Processing image - ID:[{imageView.Data.ID}] ImageRef:[{imageView.Data.imageRef}] Url:[{url}]");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ERROR] Processing image - ID:[{imageView.Data.ID}] ImageRef:[{imageView.Data.imageRef}] Url:[{url}]");
                            Console.WriteLine(ex);
                        }

                    });
                }
            }
        }

        public static void LoadFigma(this UIView contentView, FigmaFrameEntityResponse frameEntityResponse, List<IImageViewWrapper> figmaImageViews = null)
        {
            //clean views from current container
            var views = contentView.Subviews;
            foreach (var item in views)
            {
                item.RemoveFromSuperview();
            }
            contentView.RemoveConstraints(contentView.Constraints);

            //Figma doesn't calculate the bounds of our first level
            frameEntityResponse.FigmaMainNode.CalculateBounds();

            //contentView.WantsLayer = true;
            var backgroundColor = frameEntityResponse.FigmaMainNode.backgroundColor.ToUIColor();
            contentView.Layer.BackgroundColor = backgroundColor.CGColor;

            var figmaView = frameEntityResponse.FigmaMainNode as FigmaNode;
            //var mainView = figmaView.ToViewWrapper(new ViewWrapper (contentView), figmaView);
            //if (mainView != null) {
            //    contentView.AddSubview(mainView.NativeObject as NSView);
            //}
        }
    }
}
