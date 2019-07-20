using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.WinForms.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace FigmaSharp.WinForms
{
    public class FigmaDelegate : IFigmaDelegate
    {
        static readonly FigmaViewConverter[] figmaViewConverters = {
            new FigmaRegularPolygonConverter (),
            new FigmaVectorViewConverter (),
            new FigmaFrameEntityConverter (),
            new FigmaTextConverter (),
            new FigmaVectorEntityConverter (),
            new FigmaRectangleVectorConverter (),
            new FigmaElipseConverter (),
            new FigmaLineConverter ()
        };

        public bool IsVerticalAxisFlipped => false;

        public FigmaDelegate()
        {
        }

        public FigmaViewConverter[] GetFigmaConverters()
        {
            return figmaViewConverters;
        }

        Image GetFromUrl (string url)
        {
            try {
                var request = WebRequest.Create (url);

                using (var response = request.GetResponse ())
                using (var stream = response.GetResponseStream ()) {
                    return Image.FromStream(stream);
                }
            } catch (System.Exception ex) {
                Console.WriteLine (ex);
            }
            return null;
        }

        public IImageWrapper GetImage(string url)
        {
            var image = GetFromUrl (url);
            return new ImageWrapper (image);
        }

        public IImageWrapper GetImageFromFilePath(string filePath)
        {
            var image = Image.FromFile (filePath);
            return new ImageWrapper (image);
        }

        public IImageWrapper GetImageFromManifest(Assembly assembly, string imageRef)
        {
            var assemblyImage = FigmaViewsHelper.GetManifestImageResource (assembly, string.Format ("{0}.png", imageRef));
            return new ImageWrapper (assemblyImage);
        }

        public string GetFigmaFileContent(string file, string token) =>
             FigmaApiHelper.GetFigmaFileContent (file, token);

        public string GetManifestResource(Assembly assembly, string file) =>
            FigmaApiHelper.GetManifestResource (assembly, file);

        public FigmaResponse GetFigmaResponseFromContent(string template) =>
            FigmaApiHelper.GetFigmaResponseFromContent (template);

        public IViewWrapper CreateEmptyView() => new ViewWrapper (new TransparentControl ());

        public IImageViewWrapper GetImageView(IImageWrapper image)
        {
            var imageView = new ImageViewWrapper(new ImageTransparentControl());
            imageView.SetImage(image);
            return imageView;
        }

        public void BeginInvoke(Action handler)
        {
            handler();
        }

        static readonly FigmaCodePositionConverterBase positionConverter = new FigmaCodePositionConverter();
        static readonly FigmaCodeAddChildConverterBase addChildConverter = new FigmaCodeAddChildConverter();

        public FigmaCodePositionConverterBase GetPositionConverter() => positionConverter;

        public FigmaCodeAddChildConverterBase GetAddChildConverter() => addChildConverter;
    }
}
