/* 
 * FigmaDelegate.cs 
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Reflection;

using AppKit;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;
using FigmaSharp.Views.Native.Cocoa;
using FigmaSharp.Cocoa.Converters;
using FigmaSharp.Models;
using FigmaSharp.Helpers;
using FigmaSharp.Converters;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Cocoa.PropertyConfigure;

namespace FigmaSharp.Cocoa
{
    public class FigmaDelegate : IFigmaDelegate
    {
        static readonly NodeConverter[] figmaViewConverters = {
            new FigmaRegularPolygonConverter (),
            new FigmaTextConverter (),
            new FigmaLineConverter (),
            new FigmaRectangleVectorConverter (),
            new FigmaElipseConverter (),
            new FigmaVectorViewConverter (),
            new FigmaFrameConverter (),
            new FigmaVectorEntityConverter (),
        };

        static readonly CodePropertyConfigureBase codePropertyConverter = new CodePropertyConfigure ();
        static readonly ViewPropertyConfigureBase propertySetter = new ViewPropertyConfigure ();

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

        public CodePropertyConfigureBase GetCodePropertyConverter () => codePropertyConverter;
        public ViewPropertyConfigureBase GetPropertySetter() => propertySetter;
    }
}
