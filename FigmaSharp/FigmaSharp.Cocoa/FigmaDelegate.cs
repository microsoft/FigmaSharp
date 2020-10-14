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

using AppKit;

using FigmaSharp.Cocoa.Converters;
using FigmaSharp.Cocoa.PropertyConfigure;
using FigmaSharp.Converters;
using FigmaSharp.Helpers;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Cocoa
{
    public class FigmaDelegate : IFigmaDelegate
    {
        static readonly NodeConverter[] figmaViewConverters = {
            new StackViewConverter (),
            new RegularPolygonConverter (),
            new TextConverter (),
            new LineConverter (),
            new RectangleVectorConverter (),
            new ElipseConverter (),
            new PointConverter (),
            new FrameConverter (),
            new VectorConverter (),
        };

        static readonly CodePropertyConfigureBase codePropertyConverter = new CodePropertyConfigure ();
        static readonly ViewPropertyConfigureBase propertySetter = new ViewPropertyConfigure ();
        static readonly IColorService defaultColorService = new Services.ColorService ();

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

        public NodeConverter[] GetFigmaConverters() => figmaViewConverters;

        public IView CreateEmptyView() => new View();

        public string GetManifestResource(Assembly assembly, string file) =>
            WebApiHelper.GetManifestResource(assembly, file);

        public void BeginInvoke(Action handler) => NSApplication.SharedApplication.InvokeOnMainThread(handler);

        public CodePropertyConfigureBase GetCodePropertyConfigure () => codePropertyConverter;
        public ViewPropertyConfigureBase GetViewPropertyConfigure () => propertySetter;
        public IColorService GetDefaultColorService() => defaultColorService;
    }
}
