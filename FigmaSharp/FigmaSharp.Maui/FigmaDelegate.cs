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
using System.Reflection;
using System.Threading.Tasks;
using FigmaSharp.Converters;
using FigmaSharp.Maui.Converters;
using FigmaSharp.Maui.PropertyConfigure;
using FigmaSharp.Helpers;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Views;
using Microsoft.Maui.Controls;

namespace FigmaSharp.Maui
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
            return new Views.Maui.Image(image);
        }

        public IImage GetImageFromManifest (Assembly assembly, string imageRef)
        {
            var assemblyImage = ViewsHelper.GetManifestImageResource(assembly, string.Format("{0}.png", imageRef));
            return new Views.Maui.Image (assemblyImage);
        }

        public IImage GetImageFromFilePath(string filePath)
        {
           var image = ImageSource.FromFile(filePath);
           return new Views.Maui.Image(image);
        }

        public NodeConverter[] GetFigmaConverters() => figmaViewConverters;

        public Task<string> GetFigmaFileContentAsync(string file, string token) =>
            AppContext.Api.GetContentFileAsync (new FigmaFileQuery (file, token));

        public string GetManifestResource(Assembly assembly, string file) =>
            WebApiHelper.GetManifestResource(assembly, file);

        public IView CreateEmptyView() => new Views.Maui.View(new AbsoluteLayout());

        public IImageView GetImageView(IImage image)
        {
            var imageView = new FigmaSharp.Views.Maui.ImageView(new Microsoft.Maui.Controls.Image ());
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
