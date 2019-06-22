using System;
using System.Collections.Generic;
using System.Reflection;
using FigmaSharp.Converters;
using FigmaSharp.Forms.Converters;
using Xamarin.Forms;

namespace FigmaSharp.Forms
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

        public IViewWrapper CreateEmptyView() => new ViewWrapper(new AbsoluteLayout());

        public IImageViewWrapper GetImageView(IImageWrapper image)
        {
            var imageView = new ImageViewWrapper(new Image ());
            imageView.SetImage(image);
            return imageView;
        }

        public void BeginInvoke(Action handler) => Xamarin.Forms.Device.BeginInvokeOnMainThread(handler);

        static readonly FigmaCodePositionConverter positionConverter = new FigmaCodePositionConverter();
        static readonly FigmaCodeAddChildConverter addChildConverter = new FigmaCodeAddChildConverter();

        public FigmaCodePositionConverterBase GetPositionConverter() => positionConverter;
        public FigmaCodeAddChildConverterBase GetAddChildConverter() => addChildConverter;
    }
}
