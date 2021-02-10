// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

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

using FigmaSharp.Converters;
using FigmaSharp.Helpers;
using FigmaSharp.Models;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Views;
using FigmaSharp.Views.Wpf;
using FigmaSharp.Wpf.Converters;
using FigmaSharp.Wpf.PropertyConfigure;

namespace FigmaSharp.Wpf
{
    public class FigmaDelegate : IFigmaDelegate
    {
        static readonly NodeConverter[] figmaViewConverters = {
            //new RegularPolygonConverter (),
            //new Converters.PointConverter (),
            //new FrameConverter (),
            //new TextConverter (),
            //new Converters.VectorConverter (),
            //new RectangleVectorConverter (),
            //new ElipseConverter (),
            //new LineConverter (),
            //new InstanceConverter(),
            new ButtonConverter(),
            new TextBoxConverter(),
            new TextBlockConverter(),
            new CheckboxConverter(),
            new ComboBoxConverter()
        };

        static readonly NodeConverter[] figmaControlConverters =
        {
            new ButtonConverter()
        };

        public bool IsVerticalAxisFlipped => false;

        public FigmaDelegate()
        {
        }

        public NodeConverter[] GetFigmaConverters()
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
            WebApiHelper.GetManifestResource (assembly, file);
         
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

        static readonly CodePropertyConfigureBase codePropertyConverter = new CodePropertyConfigure();
        static readonly ViewPropertyConfigureBase propertySetter = new ViewPropertyConfigure();

        public CodePropertyConfigureBase GetCodePropertyConfigure() => codePropertyConverter;

        public string GetSvgData(string url)
        {
            return "";
        }

        public ViewPropertyConfigureBase GetViewPropertyConfigure() => propertySetter;
    }
}
