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
using System.Reflection;
using FigmaSharp.Converters;
using FigmaSharp.Helpers;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Web.PropertyConfigure;

namespace FigmaSharp.Web
{
    public class FigmaDelegate : IFigmaDelegate
    {
        static NodeConverter[] figmaViewConverters;
        static CodePropertyConfigureBase codePropertyConverter;
        static ViewPropertyConfigureBase viewPropertyConfigure;
        static ICodeNameService codeNameService;

        public bool IsVerticalAxisFlipped => false;

        public IImage GetImage (string url)
        {
            return null;
        }

		public IImage GetImageFromManifest (Assembly assembly, string imageRef)
        {
            return null;
        }

        public IImage GetImageFromFilePath(string filePath)
        {
            return null;
        }

        public IImageView GetImageView(IImage Image)
        {
            return null;
        }

        public NodeConverter[] GetFigmaConverters()
        {
            if (figmaViewConverters == null)
            {
                figmaViewConverters = new NodeConverter[] {
                    //new RegularPolygonConverter(),
                    //new TextConverter(),
                    //new LineConverter(),
                    //new RectangleVectorConverter(),
                    //new ElipseConverter(),
                    //new PointConverter(),
                    //new FrameConverter(),
                    //new VectorConverter(),
                };
            }
            return figmaViewConverters;
        } 

        public IView CreateEmptyView() => null;

        public string GetManifestResource(Assembly assembly, string file) =>
            WebApiHelper.GetManifestResource(assembly, file);

        public void BeginInvoke(Action handler) => handler();

        public CodePropertyConfigureBase GetCodePropertyConfigure()
        {
            if (codePropertyConverter == null)
                codePropertyConverter = new CodePropertyConfigure();
            return codePropertyConverter;
        }
        public ViewPropertyConfigureBase GetViewPropertyConfigure()
        {
            return null;
        }

        public ICodeNameService GetCodeNameService()
        {
            if (codeNameService == null)
                codeNameService = new CodeNameService();
            return codeNameService;
        }
    }
}
