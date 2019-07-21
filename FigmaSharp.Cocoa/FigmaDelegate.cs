using System;
using System.Collections.Generic;
using System.Reflection;

using AppKit;
using LiteForms;
using LiteForms.Cocoa;
using FigmaSharp.Cocoa.Converters;
using FigmaSharp.Models;

namespace FigmaSharp.Cocoa
{
    public class FigmaDelegate : IFigmaDelegate
    {
        static readonly FigmaViewConverter[] figmaViewConverters = {
            new FigmaRegularPolygonConverter (),
            new FigmaVectorViewConverter (),
            new FigmaFrameEntityConverter (),
            new FigmaTextConverter (),
            new FigmaVectorEntityConverter (),
            new RectangleVectorConverter (),
            new FigmaElipseConverter (),
            new FigmaLineConverter ()
        };

        static readonly FigmaCodePositionConverterBase positionConverter = new FigmaCodePositionConverter();
        static readonly FigmaCodeAddChildConverterBase addChildConverter = new FigmaCodeAddChildConverter();

        public bool IsVerticalAxisFlipped => true;

        public IImage GetImage (string url)
        {
            var image = new NSImage(new Foundation.NSUrl(url));
            return new Image(image);
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
            var wrapper = new ImageView(new NSImageView());
            wrapper.SetImage(Image);
            return wrapper;
        }

        public FigmaViewConverter[] GetFigmaConverters() => figmaViewConverters;

        public string GetFigmaFileContent(string file, string token) =>
             FigmaApiHelper.GetFigmaFileContent(file, token);

        public IView CreateEmptyView() => new View();

        public FigmaResponse GetFigmaResponseFromContent(string template) =>
            FigmaApiHelper.GetFigmaResponseFromContent(template);

        public string GetManifestResource(Assembly assembly, string file) =>
            FigmaApiHelper.GetManifestResource(assembly, file);

        public void BeginInvoke(Action handler) => NSApplication.SharedApplication.InvokeOnMainThread(handler);

        public FigmaCodePositionConverterBase GetPositionConverter() => positionConverter;

        public FigmaCodeAddChildConverterBase GetAddChildConverter() => addChildConverter;
    }
}
