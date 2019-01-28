using FigmaSharp.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

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

        public bool SupportsImageInvoke => true;

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

        public IViewWrapper CreateEmptyView() => new ViewWrapper ();

        public IImageViewWrapper GetImageView(IImageWrapper image)
        {
            var imageView = new ImageViewWrapper(new ImageTransparentControl());
            imageView.SetImage(image);
            return imageView;
        }

        public void BeginInvoke(Action handler)
        {
            //To define
        }

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
