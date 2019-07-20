using System;
using System.Collections.Generic;
using System.Reflection;
using FigmaSharp.Converters;
using FigmaSharp.Forms.Converters;
using FigmaSharp.Models;
using FigmaSharp.Views;
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

        public bool IsVerticalAxisFlipped => false;

        public IImage GetImage (string url)
        {
           var image = ImageSource.FromUri (new Uri (url));
            return new FigmaSharp.Views.Forms.Image(image);
        }

        public IImage GetImageFromManifest (Assembly assembly, string imageRef)
        {
            var assemblyImage = FigmaViewsHelper.GetManifestImageResource(assembly, string.Format("{0}.png", imageRef));
            return new FigmaSharp.Views.Forms.Image (assemblyImage);
        }

        public IImage GetImageFromFilePath(string filePath)
        {
           var image = ImageSource.FromFile(filePath);
           return new FigmaSharp.Views.Forms.Image(image);
        }

        public FigmaViewConverter[] GetFigmaConverters() => figmaViewConverters;

        public string GetFigmaFileContent(string file, string token) =>
             FigmaApiHelper.GetFigmaFileContent(file, token);

        public string GetManifestResource(Assembly assembly, string file) =>
            FigmaApiHelper.GetManifestResource(assembly, file);

        public FigmaResponse GetFigmaResponseFromContent(string template) =>
            FigmaApiHelper.GetFigmaResponseFromContent(template);

        public IView CreateEmptyView() => new FigmaSharp.Views.Forms.View(new AbsoluteLayout());

        public IImageView GetImageView(IImage image)
        {
            var imageView = new FigmaSharp.Views.Forms.ImageView(new Image ());
			imageView.Image = image;
            return imageView;
        }

        public void BeginInvoke(Action handler) => Xamarin.Forms.Device.BeginInvokeOnMainThread(handler);

        static readonly FigmaCodePositionConverter positionConverter = new FigmaCodePositionConverter();
        static readonly FigmaCodeAddChildConverter addChildConverter = new FigmaCodeAddChildConverter();

        public FigmaCodePositionConverterBase GetPositionConverter() => positionConverter;
        public FigmaCodeAddChildConverterBase GetAddChildConverter() => addChildConverter;

		public string GetSvgData(string url)
		{
			return "";
		}
	}
}
