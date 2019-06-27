/* 
 * FigmaFile.cs 
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
using System.Linq;
using System.Collections.Generic;

using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;

namespace FigmaSharp
{
    /// <summary>
    /// Figma file.
    /// </summary>
    public class FigmaFile : IFigmaFile
    {
        /// <summary>
        /// Gets the figma images.
        /// </summary>
        /// <value>The figma images.</value>
        public List<IImageViewWrapper> FigmaImages { get; private set; }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>The document.</value>
        public FigmaResponse Document => figmaLocalFileProvider.Response;

        /// <summary>
        /// Gets the content view.
        /// </summary>
        /// <value>The content view.</value>
        public IViewWrapper ContentView { get; private set; }

        readonly FigmaViewRendererService fileService;
        readonly FigmaViewRendererDistributionService rendererService;
        readonly FigmaManifestFileProvider figmaLocalFileProvider;

        string file;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FigmaSharp.FigmaFile"/> class.
        /// </summary>
        /// <param name="file">File.</param>
        /// <param name="figmaViewConverters">Figma view converters.</param>
        public FigmaFile (string file, FigmaViewConverter[] figmaViewConverters)
        {
            this.file = file;

            ContentView = AppContext.Current.CreateEmptyView();
            FigmaImages = new List<IImageViewWrapper>();

            var assembly = System.Reflection.Assembly.GetCallingAssembly();
            figmaLocalFileProvider = new FigmaManifestFileProvider() { Assembly = assembly };
            fileService = new FigmaViewRendererService (figmaLocalFileProvider, figmaViewConverters);
            rendererService = new FigmaViewRendererDistributionService(fileService);
        }

        /// <summary>
        /// Reload the specified includeImages.
        /// </summary>
        /// <param name="includeImages">If set to <c>true</c> include images.</param>
        public void Reload (bool includeImages = false)
        {
            Console.WriteLine($"Loading views..");
            try
            {
                FigmaImages.Clear();
                fileService.Start(file, ContentView);
                rendererService.Start();

                ReloadImages();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading resource");
                Console.WriteLine(ex);
            }

            if (includeImages)
            {
                ReloadImages();
            }
        }

        /// <summary>
        /// Reloads the images.
        /// </summary>
        public void ReloadImages ()
        {
            Console.WriteLine($"Loading images..");

            var imageVectors = fileService.ImageVectors;
            if (imageVectors?.Count > 0)
            {
                foreach (var imageVector in imageVectors)
                {
                    var recoveredKey = FigmaResourceConverter.FromResource(imageVector.Node.id);
                    var image = AppContext.Current.GetImageFromManifest(figmaLocalFileProvider.Assembly, recoveredKey);

                    var processedNode = fileService.NodesProcessed.FirstOrDefault(s => s.FigmaNode == imageVector.Node);
                    var wrapper = processedNode.View as IImageViewWrapper;
                    wrapper.SetImage(image);
                    FigmaImages.Add(wrapper);
                }
            }
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        public void InitializeComponent ()
        {
            Reload(true);
        }
    }
}
