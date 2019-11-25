﻿/* 
 * FigmaLineConverter.cs 
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
using FigmaSharp.Models;

namespace FigmaSharp.iOS.Converters
{
    public class FigmaCodePositionConverter : FigmaCodePositionConverterBase
    {
        public override string ConvertToCode(string parent, string name, FigmaNode current)
        {
            if (current is IAbsoluteBoundingBox absoluteBounding && current.Parent is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
            {
                var x = absoluteBounding.absoluteBoundingBox.x - parentAbsoluteBoundingBox.absoluteBoundingBox.x;

                var parentY = parentAbsoluteBoundingBox.absoluteBoundingBox.y + parentAbsoluteBoundingBox.absoluteBoundingBox.height;
                var actualY = absoluteBounding.absoluteBoundingBox.y + absoluteBounding.absoluteBoundingBox.height;
                var y = parentY - actualY;
                return string.Format("{0}.SetFrameOrigin(new {1}.{2}({3}, {4}));", name, nameof(CoreGraphics), nameof(CoreGraphics.CGPoint), x.ToDesignerString(), y.ToDesignerString());
            }
            return string.Empty;
        }
    }
}
