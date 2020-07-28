// Authors:
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

using CoreGraphics;

namespace AppKit
{
    public static class CGPathExtensions
    {
        //TODO: we should move this to a shared place
        public static CGPath ToCGPath(this NSBezierPath path)
        {
            var numElements = path.ElementCount;
            if (numElements == 0)
            {
                return null;
            }

            CGPath result = new CGPath();
            bool didClosePath = true;


            for (int i = 0; i < numElements; i++)
            {
                CGPoint[] points;
                var element = path.ElementAt(i, out points);
                if (element == NSBezierPathElement.MoveTo)
                {
                    result.MoveToPoint(points[0].X, points[0].Y);
                }
                else if (element == NSBezierPathElement.LineTo)
                {
                    result.AddLineToPoint(points[0].X, points[0].Y);
                    didClosePath = false;

                }
                else if (element == NSBezierPathElement.CurveTo)
                {
                    result.AddCurveToPoint(points[0].X, points[0].Y,
                                            points[1].X, points[1].Y,
                                            points[2].X, points[2].Y);
                    didClosePath = false;
                }
                else if (element == NSBezierPathElement.ClosePath)
                {
                    result.CloseSubpath();
                }
            }

            // Be sure the path is closed or Quartz may not do valid hit detection.
            if (!didClosePath)
            {
                result.CloseSubpath();
            }
            return result;
        }
    }
}
