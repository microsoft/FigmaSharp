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
using System.Globalization;
using System.Xml.Linq;
using AppKit;
using FigmaSharp.Views.Graphics;

namespace FigmaSharp.Views.Cocoa.Graphics
{
    public static class XExtensions
    {
        public static double GetWidth(this XElement element)
            => Convert.ToDouble(element.Attribute("width")?.Value);
        public static double GetHeight(this XElement element)
         => Convert.ToDouble(element.Attribute("height")?.Value);
        public static string GetViewBox(this XElement element)
        => element.Attribute("viewBox")?.Value;

        //public static NSColor GetFill(this XElement element)
        //{
        //    return ConvertToNSColor(element.Attribute("fill")?.Value);
        //}

        //public static NSColor GetStroke(this XElement element)
        //{
        //    return ConvertToNSColor(element.Attribute("stroke")?.Value);
        //}

        public static int GetStrokeWidth(this XElement element)
        => Convert.ToInt32(element.Attribute("stroke-width")?.Value);

        //NSColor gg ()
        //{
        //    NSString* hexString = [NSString stringWithFormat: @"%02X%02X%02X",
        //    (int)(color.redComponent * 0xFF), (int)(color.greenComponent * 0xFF),
        //    (int)(color.blueComponent * 0xFF)];
        //}

        static NSColor FromString(string colorcode)
        {
            colorcode = colorcode.TrimStart('#');

            NSColor col; // from System.Drawing or System.Windows.Media
            if (colorcode.Length == 6)
                col = NSColor.FromCalibratedRgba( // hardcoded opaque
                            int.Parse(colorcode.Substring(0, 2), NumberStyles.HexNumber),
                            int.Parse(colorcode.Substring(2, 2), NumberStyles.HexNumber),
                            int.Parse(colorcode.Substring(4, 2), NumberStyles.HexNumber), 255);
            else // assuming length of 8
                col = NSColor.FromCalibratedRgba(
                            int.Parse(colorcode.Substring(2, 2), NumberStyles.HexNumber),
                            int.Parse(colorcode.Substring(4, 2), NumberStyles.HexNumber),
                            int.Parse(colorcode.Substring(6, 2), NumberStyles.HexNumber),
                             int.Parse(colorcode.Substring(0, 2), NumberStyles.HexNumber));
            return col;
        }

        static bool TryGetUrl (string url, out string value)
        {
            url = url.Trim();
            if (url.StartsWith("url("))
            {
                url = url.Substring("url(".Length);
                url = url.Substring(1, url.Length - 2);
                value = url;
                return true;
            }
            value = "";
            return false;
        }

        public static bool TryGetLinearColor (RectanglePath data, Views.Graphics.Svg context, out LinearGradient gradient)
        {
            string Id = data.Id;
            if (TryGetUrl (data.Fill,out var url) )
            {
                Id = url;
            }
            var definition = context.Definitions.FirstOrDefault(s => s.Id == url);
            if (definition is LinearGradient linearGradient)
            {
                gradient = linearGradient;
                return true;
            }
            gradient = null;
            return false;
        }

        public static bool TryConvertToNSColor (string data, out NSColor color)
        {
            color = NSColor.Clear;
            if (string.IsNullOrEmpty(data))
            {
                return false;
            }

            if (data != "none")
            {
                if (data == "black")
                {
                    color = NSColor.Black;
                    return true;
                }
                if (data == "white")
                {
                    color = NSColor.White;
                    return true;
                }
            }

            try
            {
                if (data.StartsWith ("#"))
                {
                    data = data.TrimStart('#');
                    if (data.Length == 6)
                    {
                        color = NSColor.FromRgb(
                                    int.Parse(data.Substring(0, 2), NumberStyles.HexNumber),
                                    int.Parse(data.Substring(2, 2), NumberStyles.HexNumber),
                                    int.Parse(data.Substring(4, 2), NumberStyles.HexNumber));
                        return true;
                    }
                    else if (data.Length == 8)
                    {// assuming length of 8
                        color = NSColor.FromRgba(
                                        int.Parse(data.Substring(2, 2), NumberStyles.HexNumber),
                                        int.Parse(data.Substring(4, 2), NumberStyles.HexNumber),
                                        int.Parse(data.Substring(6, 2), NumberStyles.HexNumber),
                                        int.Parse(data.Substring(0, 2), NumberStyles.HexNumber));
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return true;
        }
    }
}
