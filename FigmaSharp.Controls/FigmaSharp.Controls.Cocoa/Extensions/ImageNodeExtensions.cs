// Authors:
//   jmedrano <josmed@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
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
using System.Linq;
using FigmaSharp.Models;

namespace FigmaSharp.Controls
{
    static class ImageNodeExtensions
    {
        const string imageNodeName = "!image";

        internal static bool HasNodeImageName(this FigmaNode node) => node.name.StartsWith(imageNodeName);

        internal static bool IsSingleImageViewNode(this FigmaNode node)
            => HasNodeImageName(node)
            && node is IFigmaNodeContainer container
            && !container.children.Any(s => Enum.GetNames(typeof(CocoaThemes)).Contains(s.name));

        internal static bool IsThemedImageViewNode(this FigmaNode node, out CocoaThemes theme)
        {
            if (node.Parent != null && node.Parent.HasNodeImageName())
                if (Enum.TryParse(node.name, true, out theme))
                    return true;
            theme = default;
            return false;
        }

        internal static bool IsImageViewNode(this FigmaNode node)
        {
            //images without defining any theme renders as image
            if (node.IsSingleImageViewNode())
                return true;
            return IsThemedImageViewNode(node, out _);
        }
    }
}
