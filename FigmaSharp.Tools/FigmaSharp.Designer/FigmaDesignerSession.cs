/* 
 * FigmaViewContent.cs 
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
using AppKit;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using FigmaSharp.Services;
using FigmaSharp.Converters;
using FigmaSharp.Cocoa;

namespace FigmaSharp.Designer
{
    public class FigmaDesignerSession
    {
        readonly FigmaViewRendererService fileService;
        readonly FigmaViewRendererDistributionService rendererService;
        readonly FigmaLocalFileProvider fileProvider;

        public bool IsModified { get; internal set; }

        public FigmaResponse Response => fileProvider.Response;

        public List<ProcessedNode> ProcessedNodes => fileService.NodesProcessed;
        public ProcessedNode[] MainViews => rendererService.MainViews;

        public FigmaDesignerSession(FigmaViewConverter[] figmaViewConverters)
        {
            fileProvider = new FigmaLocalFileProvider();
            fileService = new FigmaViewRendererService(fileProvider, figmaViewConverters);
            rendererService = new FigmaViewRendererDistributionService(fileService);
        }

        public event EventHandler ReloadFinished;

        public void Reload (FigmaViewRendererServiceOptions options)
        {
            fileService.Refresh(options);
            rendererService.Start();
            var resourcesDirectoryPath = Path.Combine(baseDirectory, "Resources");
            ReloadImages(resourcesDirectoryPath);
            ReloadFinished?.Invoke(this, EventArgs.Empty);
        }

        string baseDirectory;

        public void Reload(IViewWrapper contentView, string fileName, string baseDirector)
        {
            this.baseDirectory = baseDirectory;
            try
            {
                var resourcesDirectoryPath = Path.Combine(baseDirectory, "Resources");
                if (!Directory.Exists(resourcesDirectoryPath))
                {
                    throw new DirectoryNotFoundException(resourcesDirectoryPath);
                }
                fileService.Start(fileName, contentView);
                rendererService.Start();
                ReloadImages(resourcesDirectoryPath);

                ReloadFinished?.Invoke(this, EventArgs.Empty);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine("[FIGMA.RENDERER] Resource directory not found ({0}). Images will not load", ex.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void ReloadImages(string resourcesDirectory, string format = ".png")
        {
            Console.WriteLine($"Loading images..");

            var imageVectors = fileService.ImageVectors;
            if (imageVectors?.Count > 0)
            {
                foreach (var imageVector in imageVectors)
                {
                    try
                    {
                        var recoveredKey = FigmaResourceConverter.FromResource(imageVector.Node.id);
                        string filePath = Path.Combine(resourcesDirectory, string.Concat(recoveredKey, format));

                        if (!File.Exists(filePath))
                        {
                            throw new FileNotFoundException(filePath);
                        }

                        var processedNode = fileService.NodesProcessed.FirstOrDefault(s => s.FigmaNode == imageVector.Node);
                        var wrapper = processedNode.View as IImageViewWrapper;

                        var image = new ImageWrapper(new NSImage(filePath));
                        wrapper.SetImage(image);
                    }
                    catch (FileNotFoundException ex)
                    {
                        Console.WriteLine("[FIGMA.RENDERER] Resource '{0}' not found.", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    //FigmaImages.Add(wrapper);
                }
            }
        }

        public void Save(string fileName)
        {
            fileProvider.Save(fileName);
        }

        public event EventHandler ModifiedChanged;

        public IViewWrapper GetViewWrapper(FigmaNode e)
        {
            var processed = ProcessedNodes.FirstOrDefault(s => s.FigmaNode == e);
            return processed?.View;
        }

        public FigmaNode GetModel (IViewWrapper e)
        {
            var processed = ProcessedNodes.FirstOrDefault(s => s.View.NativeObject == e.NativeObject);
            return processed?.FigmaNode;
        }

        public void DeleteView(FigmaNode e)
        {
            foreach (var canvas in fileProvider.Response.document.children)
            {
                if (DeleteNodeRecursively(canvas, fileProvider.Response.document, e))
                {
                    return;
                }
            }
        }

        static T[] RemoveAt<T>(T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }
        bool DeleteNodeRecursively (FigmaNode current, FigmaNode parent, FigmaNode toDelete)
        {
            if (current == toDelete)
            {
                if (parent is FigmaDocument parentDocument)
                {
                    var index = Array.FindIndex(parentDocument.children, row => row == current);
                    parentDocument.children = RemoveAt<FigmaCanvas>(parentDocument.children, index);
                    return true;
                } else if (parent is IFigmaNodeContainer parentNodeContainer)
                {
                    var index = Array.FindIndex(parentNodeContainer.children, row => row == current);
                    parentNodeContainer.children = RemoveAt<FigmaNode>(parentNodeContainer.children, index);
                    return true;
                }
            }

            if (current is FigmaDocument document)
            {
                if (document.children != null)
                {
                    foreach (var item in document.children)
                    {
                        try
                        {
                            if (DeleteNodeRecursively(item, current, toDelete))
                            {
                                return true;
                            };
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                return false;
            }

            if (current is IFigmaNodeContainer container)
            {
                foreach (var item in container.children)
                {
                    try
                    {
                        if (DeleteNodeRecursively(item, current, toDelete))
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            return false;
        }
    }
}