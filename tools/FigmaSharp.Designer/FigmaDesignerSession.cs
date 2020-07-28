﻿// Authors:
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace FigmaSharp.Designer
{
    public class FigmaDesignerSession
    {
        readonly ViewRenderService rendererService;
        readonly StoryboardLayoutManager distributionService;
        readonly INodeProvider fileProvider;

        public bool IsModified { get; internal set; }

        public FigmaFileResponse Response => fileProvider.Response;

        public List<ViewNode> ProcessedNodes => rendererService.NodesProcessed;

        public ViewNode[] MainViews => rendererService.NodesProcessed.Where (s => s.ParentView != null && s.ParentView.Node is FigmaCanvas).ToArray ();

        public FigmaDesignerSession(INodeProvider figmaFileProvider, ViewRenderService figmaViewRendererService, StoryboardLayoutManager figmaViewRendererDistributionService)
        {
            fileProvider = figmaFileProvider;
            rendererService = figmaViewRendererService;
            distributionService = figmaViewRendererDistributionService;
        }

        public event EventHandler ReloadFinished;

        string baseDirectory;

        public void Reload(IView contentView, string file, ViewRenderServiceOptions options)
        {
            try
            {
                rendererService.Start(file, contentView, options);
                distributionService.Run(contentView, rendererService);

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

        public async Task ReloadAsync (IView contentView, string file, ViewRenderServiceOptions options)
        {
            try
            {
                await rendererService.StartAsync (file, contentView, options);
                distributionService.Run(contentView, rendererService);

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

        public void Save(string fileName)
        {
            fileProvider.Save(fileName);
        }

        public event EventHandler ModifiedChanged;

        public IView GetViewWrapper(FigmaNode e)
        {
            var processed = ProcessedNodes.FirstOrDefault(s => s.Node == e);
            return processed?.View;
        }

        public FigmaNode GetModel(IView e)
        {
            var processed = ProcessedNodes.FirstOrDefault(s => s.View != null && s.View.NativeObject == e.NativeObject);
            return processed?.Node;
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
        bool DeleteNodeRecursively(FigmaNode current, FigmaNode parent, FigmaNode toDelete)
        {
            if (current == toDelete)
            {
                if (parent is FigmaDocument parentDocument)
                {
                    var index = Array.FindIndex(parentDocument.children, row => row == current);
                    parentDocument.children = RemoveAt<FigmaCanvas>(parentDocument.children, index);
                    return true;
                }
                else if (parent is IFigmaNodeContainer parentNodeContainer)
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