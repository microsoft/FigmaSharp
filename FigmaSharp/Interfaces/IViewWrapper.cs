﻿/* 
 * FigmaImageView.cs - NSImageView which stores it's associed Figma Id
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

using System.Collections.Generic;
using FigmaSharp.Models;

namespace FigmaSharp
{
    public interface IViewWrapper : IObjectWrapper
    {
        IViewWrapper Parent { get; }

        IReadOnlyList<IViewWrapper> Children { get; }

        string Identifier { get; set; }
        string NodeName { get; set; }
        bool Hidden { get; set; }

        float Width { get; set; }
        float Height { get; set; }

        FigmaRectangle Allocation { get; }

        void AddChild(IViewWrapper view);
        void CreateConstraints(FigmaNode current);

        void RemoveChild(IViewWrapper view);

        void ClearSubviews();

        void MakeFirstResponder();
        void SetPosition(float x, float y);
        void SetAllocation(float x, float y, float width, float height);
    }
}
