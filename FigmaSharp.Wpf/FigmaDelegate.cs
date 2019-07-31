using FigmaSharp.Converters;
using FigmaSharp.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using FigmaSharp.Models;

namespace FigmaSharp.Wpf
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

        ImageSource GetFromUrl (string url)
        {
            try {

                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.UriSource = new Uri(url);
                imageSource.EndInit();
                return imageSource;

            } catch (System.Exception ex) {
                Console.WriteLine (ex);
            }
            return null;
        }

        public IImageWrapper GetImage(string url)
        {
            ImageSource image = null;
            Application.Current.Dispatcher.Invoke(() => { image = GetFromUrl(url); });
            return new ImageWrapper (image);
        }

        public IImageWrapper GetImageFromFilePath(string filePath)
        {
            BitmapImage source = null;
            Application.Current.Dispatcher.Invoke(() => { source = new BitmapImage(new Uri(filePath)); });
            return new ImageWrapper (source);
        }

        public IImageWrapper GetImageFromManifest(Assembly assembly, string imageRef)
        {
            ImageSource assemblyImage = null;
            Application.Current.Dispatcher.Invoke(() => { assemblyImage = FigmaViewsHelper.GetManifestImageResource(assembly, string.Format("{0}.png", imageRef)); });
            return new ImageWrapper (assemblyImage);
        }

        public string GetFigmaFileContent(string file, string token) =>
             FigmaApiHelper.GetFigmaFileContent (file, token);

        public string GetManifestResource(Assembly assembly, string file) =>
            FigmaApiHelper.GetManifestResource (assembly, file);

        public FigmaResponse GetFigmaResponseFromContent(string template) =>
            FigmaApiHelper.GetFigmaResponseFromContent (template);

        public IViewWrapper CreateEmptyView() => new ViewWrapper (new Canvas ());

        public IImageViewWrapper GetImageView(IImageWrapper image)
        {
            ImageViewWrapper imageView = null;
            Application.Current.Dispatcher.Invoke(() => {
                var picture = new CanvasImage();
                imageView = new ImageViewWrapper(picture);
                imageView.SetImage(image);
            });
          
            return imageView;
        }

        public void BeginInvoke(Action handler) => Application.Current.Dispatcher.Invoke(handler);

        static readonly FigmaCodePositionConverterBase positionConverter = new FigmaCodePositionConverter();
        static readonly FigmaCodeAddChildConverterBase addChildConverter = new FigmaCodeAddChildConverter();

        public FigmaCodePositionConverterBase GetPositionConverter() => positionConverter;

        public FigmaCodeAddChildConverterBase GetAddChildConverter() => addChildConverter;
    }
}
