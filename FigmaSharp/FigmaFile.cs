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
using LiteForms;

namespace FigmaSharp
{
    /// <summary>
    /// Use FigmaFile to load a FigmaDocument from a .figma file bundled in your project.
    /// </summary>
    public class FigmaFile : IFigmaFile
    {
        /// <summary>
        /// Gets the figma images.
        /// </summary>
        /// <value>The figma images.</value>
        public List<IImageView> FigmaImages { get; private set; }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>The document.</value>
        public FigmaResponse Document => figmaLocalFileProvider.Response;

        /// <summary>
        /// Gets the content view.
        /// </summary>
        /// <value>The content view.</value>
        public IView ContentView { get; private set; }

        readonly FigmaFileRendererService fileService;
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
            FigmaImages = new List<IImageView>();

            var assembly = System.Reflection.Assembly.GetCallingAssembly();
            figmaLocalFileProvider = new FigmaManifestFileProvider(assembly);
            fileService = new FigmaFileRendererService (figmaLocalFileProvider, figmaViewConverters);
            rendererService = new FigmaViewRendererDistributionService(fileService);
        }

        /// <summary>
        /// Reload the specified includeImages.
        /// </summary>
        public void Reload ()
        {
            Console.WriteLine($"Loading views..");
            try
            {
                FigmaImages.Clear();
                fileService.Start(file, ContentView);
                rendererService.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading resource");
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        public void InitializeComponent ()
        {
            Reload ();
        }
    }
}
