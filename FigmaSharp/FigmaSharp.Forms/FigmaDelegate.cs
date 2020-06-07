using System;
using System.Collections.Generic;
using System.Reflection;
using FigmaSharp.Converters;
using FigmaSharp.Forms.Converters;
using FigmaSharp.Forms.PropertyConfigure;
using FigmaSharp.Helpers;
using FigmaSharp.Models;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Views;
using Xamarin.Forms;

namespace FigmaSharp.Forms
{
    public class FigmaDelegate : IFigmaDelegate
    {
        static readonly NodeConverter[] figmaViewConverters = {
            new PointConverter (),
            new FrameConverter (),
            new TextConverter (),
            new VectorConverter (),
            new RectangleVectorConverter (),
            new ElipseConverter (),
            new LineConverter ()
        };

        public bool IsVerticalAxisFlipped => false;

        public IImage GetImage (string url)
        {
           var image = ImageSource.FromUri (new Uri (url));
            return new Views.Forms.Image(image);
        }

        public IImage GetImageFromManifest (Assembly assembly, string imageRef)
        {
            var assemblyImage = ViewsHelper.GetManifestImageResource(assembly, string.Format("{0}.png", imageRef));
            return new Views.Forms.Image (assemblyImage);
        }

        public IImage GetImageFromFilePath(string filePath)
        {
           var image = ImageSource.FromFile(filePath);
           return new Views.Forms.Image(image);
        }

        public NodeConverter[] GetFigmaConverters() => figmaViewConverters;

        public string GetFigmaFileContent(string file, string token) =>
            AppContext.Api.GetContentFile (new FigmaFileQuery (file, token));

        public string GetManifestResource(Assembly assembly, string file) =>
            WebApiHelper.GetManifestResource(assembly, file);

        public IView CreateEmptyView() => new Views.Forms.View(new AbsoluteLayout());

        public IImageView GetImageView(IImage image)
        {
            var imageView = new FigmaSharp.Views.Forms.ImageView(new Image ());
			imageView.Image = image;
            return imageView;
        }

        public void BeginInvoke(Action handler) => Device.BeginInvokeOnMainThread(handler);

        static readonly CodePropertyConfigureBase positionConverter = new CodePropertyConfigure();
        static readonly ViewPropertyConfigureBase addChildConverter = new ViewPropertyConfigure();

        public CodePropertyConfigureBase GetCodePropertyConfigure() => positionConverter;
        public ViewPropertyConfigureBase GetViewPropertyConfigure() => addChildConverter;

		public string GetSvgData(string url)
		{
			return "";
		}
	}
}
