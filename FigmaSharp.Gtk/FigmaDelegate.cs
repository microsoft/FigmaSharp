using System;
using System.Collections.Generic;
using System.Reflection;
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

        static readonly FigmaCodePositionConverter positionConverter = new GtkFigmaCodePositionConverter();
        static readonly FigmaCodeAddChildConverter addChildConverter = new GtkFigmaCodeAddChildConverter();

        public bool IsYAxisFlipped => false;

        public IImageWrapper GetImage (string url)
        {
            var image = new Gdk.Pixbuf(url);
            return new ImageWrapper(image);
        }

        public IImageWrapper GetImageFromManifest (Assembly assembly, string imageRef)
        {
            var assemblyImage = FigmaViewsHelper.GetManifestImageResource(assembly, string.Format("{0}.png", imageRef));
            return new ImageWrapper (assemblyImage);
        }

        public IImageWrapper GetImageFromFilePath(string filePath)
        {
            var image = new Gdk.Pixbuf(filePath);
            return new ImageWrapper(image);
        }

        public IImageViewWrapper GetImageView(IImageWrapper imageWrapper)
        {
            var pixelBuf = (Gdk.Pixbuf)imageWrapper.NativeObject;
            var image = new Gtk.Image(pixelBuf);
            var fixedView = new Gtk.Fixed();
            fixedView.Put(image, 0, 0);
            var wrapper = new ImageViewWrapper(image, fixedView);
            return wrapper;
        }

        public FigmaViewConverter[] GetFigmaConverters() => figmaViewConverters;

        public string GetFigmaFileContent(string file, string token) =>
             FigmaApiHelper.GetFigmaFileContent(file, token);

        public IViewWrapper CreateEmptyView ()
        {
                var fixedView = new Gtk.Fixed();
                return new ViewWrapper(fixedView, fixedView);

        }

        public FigmaResponse GetFigmaResponseFromContent(string template) =>
            FigmaApiHelper.GetFigmaResponseFromContent(template);

        public string GetManifestResource(Assembly assembly, string file) =>
            FigmaApiHelper.GetManifestResource(assembly, file);

        public void BeginInvoke(Action handler) => Gtk.Application.InvokeAction(handler);

        public FigmaCodePositionConverter GetPositionConverter() => positionConverter;

        public FigmaCodeAddChildConverter GetAddChildConverter() => addChildConverter;
    }
}
