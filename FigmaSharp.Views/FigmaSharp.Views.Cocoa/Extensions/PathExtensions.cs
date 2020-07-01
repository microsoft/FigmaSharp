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
