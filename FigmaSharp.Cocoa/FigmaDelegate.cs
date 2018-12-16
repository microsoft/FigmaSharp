using System;
using System.Collections.Generic;
using System.Reflection;
using AppKit;
using FigmaSharp.Converters;

namespace FigmaSharp
{
    public class FigmaDelegate : IFigmaDelegate
    {
        static readonly FigmaViewConverter[] figmaViewConverters = {
            new FigmaVectorViewConverter (),
            new FigmaFrameEntityConverter (),
            new FigmaTextConverter (),
            new FigmaVectorEntityConverter (),
            new FigmaRectangleVectorConverter (),
            new FigmaElipseConverter (),
            new FigmaLineConverter ()
        };

        public IImageWrapper GetImage (string url)
        {
            var image = new NSImage(new Foundation.NSUrl(url));
            return new ImageWrapper(image);
        }

        public IImageWrapper GetImageFromManifest (Assembly assembly, string imageRef)
        {
            var assemblyImage = FigmaViewsHelper.GetManifestImageResource(assembly, string.Format("{0}.png", imageRef));
            return new ImageWrapper (assemblyImage);
        }

        public IImageWrapper GetImageFromFilePath(string filePath)
        {
           var image = new NSImage(filePath);
           return new ImageWrapper(image);
        }

        public IImageViewWrapper GetImageView(FigmaPaint figmaPaint)
        {
            return new ImageViewWrapper(new NSImageView())
            {
                 Data = figmaPaint
            };
       }

        public FigmaViewConverter[] GetFigmaConverters() => figmaViewConverters;

        public void LoadFigmaFromFrameEntity(IViewWrapper contentView, IFigmaDocumentContainer document, List<IImageViewWrapper> figmaImages, string figmaFileName) =>
            (contentView.NativeObject as NSView).LoadFigmaFromFrameEntity(document, figmaImages, figmaFileName);

        public string GetFigmaFileContent(string file, string token) =>
             FigmaApiHelper.GetFigmaFileContent(file, token);

        public IViewWrapper CreateEmptyView() => new ViewWrapper();

        public IFigmaDocumentContainer GetFigmaDialogFromContent(string template) =>
            FigmaApiHelper.GetFigmaDialogFromContent(template);

        public string GetManifestResource(Assembly assembly, string file) =>
            FigmaApiHelper.GetManifestResource(assembly, file);

    }
}
