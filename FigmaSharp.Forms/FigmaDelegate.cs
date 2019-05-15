using System;
using System.Collections.Generic;
using System.Reflection;
using FigmaSharp.Converters;
using Xamarin.Forms;

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

        public IImageWrapper GetImage (string url)
        {
           var image = ImageSource.FromUri (new Uri (url));
            return new ImageWrapper(image);
        }

        public IImageWrapper GetImageFromManifest (Assembly assembly, string imageRef)
        {
            var assemblyImage = FigmaViewsHelper.GetManifestImageResource(assembly, string.Format("{0}.png", imageRef));
            return new ImageWrapper (assemblyImage);
        }

        public IImageWrapper GetImageFromFilePath(string filePath)
        {
           var image = ImageSource.FromFile(filePath);
           return new ImageWrapper(image);
        }

        public FigmaViewConverter[] GetFigmaConverters() => figmaViewConverters;

        public string GetFigmaFileContent(string file, string token) =>
             FigmaApiHelper.GetFigmaFileContent(file, token);

        public string GetManifestResource(Assembly assembly, string file) =>
            FigmaApiHelper.GetManifestResource(assembly, file);

        public FigmaResponse GetFigmaResponseFromContent(string template) =>
            FigmaApiHelper.GetFigmaResponseFromContent(template);

        public IViewWrapper CreateEmptyView() => new ViewWrapper(new EmptyView());

        public IImageViewWrapper GetImageView(IImageWrapper image)
        {
            var imageView = new ImageViewWrapper(new Image ());
            imageView.SetImage(image);
            return imageView;
        }

        public void BeginInvoke(Action handler) => Xamarin.Forms.Device.BeginInvokeOnMainThread(handler);

        public FigmaCodePositionConverter GetPositionConverter()
        {
            throw new NotImplementedException();
        }

        public FigmaCodeAddChildConverter GetAddChildConverter()
        {
            throw new NotImplementedException();
        }
    }

    public class EmptyView : View
    {

    }
}
