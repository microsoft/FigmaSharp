using FigmaSharp.Converters;
using FigmaSharp.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FigmaSharp.Wpf
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

        public FigmaDelegate()
        {
        }

        public FigmaViewConverter[] GetFigmaConverters()
        {
            return figmaViewConverters;
        }

        ImageSource GetFromUrl (string url)
        {
            try {
                var request = WebRequest.Create (url);

                using (var response = request.GetResponse ())
                using (var stream = response.GetResponseStream ()) {

                    var imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.StreamSource = stream;
                    imageSource.EndInit();

                    return imageSource;
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
            var source = new BitmapImage(new Uri(filePath));
            return new ImageWrapper (source);
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
            var imageView = new ImageViewWrapper(new Image());
            imageView.SetImage(image);
            return imageView;
        }

        public void BeginInvoke(Action handler)
        {
            //To define
        }

        public FigmaCodePositionConverterBase GetPositionConverter()
        {
            throw new NotImplementedException();
        }

        public FigmaCodeAddChildConverterBase GetAddChildConverter()
        {
            throw new NotImplementedException();
        }
    }
}
