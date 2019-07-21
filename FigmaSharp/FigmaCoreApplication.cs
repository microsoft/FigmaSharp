/* 
 * FigmaEnvirontment.cs 
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
using LiteForms;
using FigmaSharp.Models;

namespace FigmaSharp
{

    /// <summary>
    /// An AppContext contains configuration (such as the authentication token)
    /// and code converters for the current application.
    /// </summary>
    public class AppContext : IFigmaDelegate
    {
        IFigmaDelegate figmaDelegate;

        public bool IsConfigured => !string.IsNullOrEmpty(Token);

        internal string Token { get; set; }

        AppContext()
        {

        }

        public void Configuration(IFigmaDelegate currentDelegate, string token)
        {
            SetAccessToken(token);
            figmaDelegate = currentDelegate;
        }

        public void SetAccessToken(string token)
        {
            Token = token;
        }

        public void BeginInvoke(Action handler) => figmaDelegate.BeginInvoke(handler);

        public IView CreateEmptyView() => figmaDelegate.CreateEmptyView();

        public FigmaViewConverter[] GetFigmaConverters() => figmaDelegate.GetFigmaConverters();

        public IImage GetImage(string url) => figmaDelegate.GetImage(url);

        public IImage GetImageFromFilePath(string filePath) =>
            figmaDelegate.GetImageFromFilePath(filePath);

        public FigmaResponse GetFigmaResponseFromContent(string template) =>
              FigmaApiHelper.GetFigmaResponseFromContent(template);

        public void SetFigmaResponseFromContent(FigmaResponse figmaResponse, string filePath) =>
             FigmaApiHelper.SetFigmaResponseFromContent(figmaResponse, filePath);

        public IImage GetImageFromManifest(Assembly assembly, string imageRef) =>
            figmaDelegate.GetImageFromManifest(assembly, imageRef);

        public string GetFigmaFileContent(string file, string token) =>
            figmaDelegate.GetFigmaFileContent(file, token);

        public string GetManifestResource(Assembly assembly, string file) =>
            figmaDelegate.GetManifestResource(assembly, file);

        public IImageView GetImageView(IImage image)
        {
            return figmaDelegate.GetImageView(image);
        }

        public FigmaCodePositionConverterBase GetPositionConverter()
        {
            return figmaDelegate.GetPositionConverter ();
        }

        public FigmaCodeAddChildConverterBase GetAddChildConverter()
        {
            return figmaDelegate.GetAddChildConverter();
        }

        #region Static

        static AppContext current;

        /// <summary>
        /// The shared AppContext for the application.
        /// </summary>
        /// <value>The current.</value>
        public static AppContext Current
        {
            get
            {
                if (current == null)
                {
                    current = new AppContext();
                }
                return current;
            }
        }

        public bool IsVerticalAxisFlipped => figmaDelegate.IsVerticalAxisFlipped;

        #endregion
    }
}
