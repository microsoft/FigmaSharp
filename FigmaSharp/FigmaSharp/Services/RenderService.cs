﻿/* 
 * ViewRenderService.cs
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
using System.Linq;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Views;

namespace FigmaSharp.Services
{
    public abstract class RenderService
    {
        protected readonly List<NodeConverter> DefaultConverters;
        public readonly List<NodeConverter> CustomConverters;
       
        public INodeProvider NodeProvider { get; }

        public RenderService(INodeProvider figmaProvider, NodeConverter[] figmaViewConverters)
        {
            this.NodeProvider = figmaProvider;
            DefaultConverters = figmaViewConverters.Where(s => s.IsLayer).ToList();
            CustomConverters = figmaViewConverters.Where(s => !s.IsLayer).ToList();
        }

        public FigmaNode FindNodeByName(string name)
        {
            return NodeProvider.Nodes.FirstOrDefault(s => s.name == name);
        }

        public FigmaNode FindNodeById(string id)
        {
            return NodeProvider.Nodes.FirstOrDefault(s => s.id == id);
        }

        FigmaNode GetRecursively(string name, FigmaNode figmaNode)
        {
            if (figmaNode.name == name)
            {
                return figmaNode;
            }

            if (figmaNode is IFigmaNodeContainer container)
            {
                foreach (var item in container.children)
                {
                    var node = GetRecursively(name, item);
                    if (node != null)
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        protected NodeConverter GetProcessedConverter(FigmaNode currentNode, IEnumerable<NodeConverter> customViewConverters)
        {
            foreach (var customViewConverter in customViewConverters)
            {
                if (customViewConverter.CanConvert(currentNode))
                {
                    return customViewConverter;
                }
            }
            return null;
        }
    }
}
