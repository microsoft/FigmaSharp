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

using System.Collections.Generic;

namespace FigmaSharp.Controls
{
    public class ControlImageNodeRequest : IImageNodeRequest
    {
        public string ResourceId => Node.id;
        public string Url { get; set; }

        public Models.FigmaNode Node { get; }

        public List<ImageScale> Scales { get; } = new List<ImageScale>();

        public ControlImageNodeRequest(Models.FigmaNode node)
        {
            this.Node = node;
        }

        public string GetOutputFileName(float scale)
        {
            string scaleName = scale == 1 ? "" : $"@{scale:N0}x";

            //we want try set custom name if not id
            string customName;

            if (Node.IsThemedImageViewNode(out var theme))
            {
                Node.Parent.TryGetNodeCustomName(out customName);
                switch (theme)
                {
                    case CocoaThemes.Dark:
                        return string.Format("{0}~dark{1}", customName, scaleName);
                    case CocoaThemes.DarkHC:
                        return string.Format("{0}~contrast~dark{1}", customName, scaleName);
                    case CocoaThemes.Light:
                        return string.Format("{0}{1}", customName, scaleName);
                    case CocoaThemes.LightHC:
                        return string.Format("{0}~contrast{1}", customName, scaleName);
                }
            }

            Node.TryGetNodeCustomName(out customName);
            return string.Format("{0}{1}", customName, scaleName);
        }
    }
}
