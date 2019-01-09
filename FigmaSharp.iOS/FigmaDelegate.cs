using System;
using System.Collections.Generic;
using System.Reflection;
using FigmaSharp.Converters;
using Foundation;
using UIKit;

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

        public bool IsYAxisFlipped => false;

        static UIImage FromUrl(string uri)
        {
            using (var url = new NSUrl(uri))
            using (var data = NSData.FromUrl(url))
                return UIImage.LoadFromData(data);
        }

        public IImageWrapper GetImage (string url)
        {
           var image = FromUrl(url);
            return new ImageWrapper(image);
        }

        public IImageWrapper GetImageFromManifest (Assembly assembly, string imageRef)
        {
            var assemblyImage = FigmaViewsHelper.GetManifestImageResource(assembly, string.Format("{0}.png", imageRef));
            return new ImageWrapper (assemblyImage);
        }

        public IImageWrapper GetImageFromFilePath(string filePath)
        {
           var image = new UIImage(filePath);
           return new ImageWrapper(image);
        }

        public FigmaViewConverter[] GetFigmaConverters() => figmaViewConverters;

        public string GetFigmaFileContent(string file, string token) =>
             FigmaApiHelper.GetFigmaFileContent(file, token);

        public string GetManifestResource(Assembly assembly, string file) =>
            FigmaApiHelper.GetManifestResource(assembly, file);

        public FigmaResponse GetFigmaResponseFromContent(string template) =>
            FigmaApiHelper.GetFigmaResponseFromContent(template);

        public IViewWrapper CreateEmptyView() => new ViewWrapper();

        public IImageViewWrapper GetImageView(IImageWrapper image)
        {
            var imageView = new ImageViewWrapper(new UIImageView ());
            imageView.SetImage(image);
            return imageView;
        }

        public void BeginInvoke(Action handler) =>
            UIApplication.SharedApplication.InvokeOnMainThread(handler);

        public FigmaCodePositionConverter GetPositionConverter()
        {
            throw new NotImplementedException();
        }

        public FigmaCodeAddChildConverter GetAddChildConverter()
        {
            throw new NotImplementedException();
        }
    }
}
