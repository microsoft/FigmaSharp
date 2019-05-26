﻿/* 
 * FigmaElipseConverter.cs 
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
using System.Text;
using AppKit;
using FigmaSharp.Converters;

namespace FigmaSharp.Cocoa.Converters
{
    public class FigmaElipseConverter : FigmaElipseConverterBase
    {
        public override IViewWrapper ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
            var elipseView = new NSView();// { TranslatesAutoresizingMaskIntoConstraints = false };
            var elipse = (FigmaElipse)currentNode;
            elipseView.Configure(elipse);
            return new ViewWrapper(elipseView);
        }

        public override string ConvertToCode(FigmaNode currentNode)
        {
            StringBuilder builder = new StringBuilder();
            var name = "[NAME]";
            builder.AppendLine($"var {name} = new {nameof (NSView)}();");

            builder.Configure(name, currentNode);
            return builder.ToString();
        }
    }
}
