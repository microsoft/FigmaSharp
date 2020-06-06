using System;
using System.Collections.Generic;
using System.Reflection;

using AppKit;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Native.Cocoa;
using FigmaSharp.Cocoa.Converters;
using FigmaSharp.Models;
using FigmaSharp.Helpers;
using FigmaSharp.Converters;

namespace FigmaSharp.Cocoa
{
    public class FigmaDelegate : IFigmaDelegate
    {
        static readonly LayerConverter[] figmaViewConverters = {
            new FigmaRegularPolygonConverter (),
            new FigmaTextConverter (),
            new FigmaLineConverter (),
            new FigmaRectangleVectorConverter (),
            new FigmaElipseConverter (),
            new FigmaVectorViewConverter (),
            new FigmaFrameConverter (),
            new FigmaVectorEntityConverter (),
        };

        static readonly CodePropertyNodeConfigureBase codePropertyConverter = new FigmaCodePropertyConverter ();
        static readonly ViewPropertyNodeConfigureBase propertySetter = new FigmaViewPropertySetter ();

        public bool IsVerticalAxisFlipped => false;

        public IImage GetImage (string url)
        {
            var image = new NSImage(new Foundation.NSUrl(url));
            return new Image(image);
        }

		public string GetSvgData(string url)
		{
			return "";
		}

		public IImage GetImageFromManifest (Assembly assembly, string imageRef)
        {
            var assemblyImage = ViewsHelper.GetManifestImageResource(assembly, string.Format("{0}.png", imageRef));
            return new Image (assemblyImage);
        }

        public IImage GetImageFromFilePath(string filePath)
        {
           var image = new NSImage(filePath);
           return new Image(image);
        }

        public IImageView GetImageView(IImage Image)
        {
            var wrapper = new ImageView();
            wrapper.Image = Image;
            return wrapper;
        }

        public LayerConverter[] GetFigmaConverters() => figmaViewConverters;

        public IView CreateEmptyView() => new View();

        public string GetManifestResource(Assembly assembly, string file) =>
            WebApiHelper.GetManifestResource(assembly, file);

        public void BeginInvoke(Action handler) => NSApplication.SharedApplication.InvokeOnMainThread(handler);

        public CodePropertyNodeConfigureBase GetCodePropertyConverter () => codePropertyConverter;
        public ViewPropertyNodeConfigureBase GetPropertySetter() => propertySetter;
    }
}
