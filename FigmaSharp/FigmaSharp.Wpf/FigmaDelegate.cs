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
using FigmaSharp.Views;
using FigmaSharp.Wpf.PropertyConverter;
using FigmaSharp.Views.Wpf;

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

        public IImage GetImage(string url)
        {
            ImageSource image = null;
            Application.Current.Dispatcher.Invoke(() => { image = GetFromUrl(url); });
            return new Views.Wpf.Image(image);
        }

        public IImage GetImageFromFilePath(string filePath)
        {
            BitmapImage source = null;
            Application.Current.Dispatcher.Invoke(() => { source = new BitmapImage(new Uri(filePath)); });
            return new Views.Wpf.Image(source);
        }

        public IImage GetImageFromManifest(Assembly assembly, string imageRef)
        {
            ImageSource assemblyImage = null;
            Application.Current.Dispatcher.Invoke(() => { assemblyImage = ViewsHelper.GetManifestImageResource(assembly, string.Format("{0}.png", imageRef)); });
            return new Views.Wpf.Image(assemblyImage);
        }
         
        public string GetManifestResource(Assembly assembly, string file) =>
            FigmaApiHelper.GetManifestResource (assembly, file);
         
        public IView CreateEmptyView() => new View (new Canvas ());

        public IImageView GetImageView(IImage image)
        {
            ImageView imageView = null;
            Application.Current.Dispatcher.Invoke(() => {
            //var picture = new CanvasImage();
            imageView = new ImageView();// picture);
                imageView.Image = image;
            });
          
            return imageView;
        }

        public void BeginInvoke(Action handler) => Application.Current.Dispatcher.Invoke(handler);

        static readonly FigmaCodePropertyConverterBase codePropertyConverter = new FigmaCodePropertyConverter();
        static readonly FigmaViewPropertySetterBase propertySetter = new FigmaViewPropertySetter();

        public FigmaCodePropertyConverterBase GetCodePropertyConverter() => codePropertyConverter;

        public string GetSvgData(string url)
        {
            return "";
        }

        public FigmaViewPropertySetterBase GetPropertySetter() => propertySetter;
    }
}
