/* 
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
using System.Text;
using AppKit;

using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Native.Cocoa;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Cocoa.Converters
{
    public class FigmaLineConverter : FigmaLineConverterBase
    {
        public override IView ConvertTo(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var figmaLineView = new HorizontalBar ();
            var nativeView = (FNSBox)figmaLineView.NativeObject;
           
            nativeView.Configure (currentNode);
            return figmaLineView;
        }

        public override string ConvertToCode(FigmaNode currentNode, FigmaCodeRendererService rendererService)
        {
            StringBuilder builder = new StringBuilder();
            var name = Resources.Ids.Conversion.NameIdentifier;

            if (rendererService.NeedsRenderInstance (currentNode))
                builder.AppendLine($"var {name} = new {typeof(NSBox).FullName}();");

            builder.Configure(name, currentNode, false);

            if (currentNode is IAbsoluteBoundingBox container) {
                var width = container.absoluteBoundingBox.Width == 0 ? 1 : container.absoluteBoundingBox.Width;
                var height = container.absoluteBoundingBox.Height == 0 ? 1 : container.absoluteBoundingBox.Height;

                builder.AppendLine (string.Format ("{0}.SetFrameSize (new {1}({2}, {3}));",
                    name,
                    typeof (CoreGraphics.CGSize).FullName,
                    width.ToDesignerString (),
					height.ToDesignerString ()
                    ));
            }

            builder.AppendLine (string.Format ("{0}.BoxType = {1};", name, NSBoxType.NSBoxSeparator.GetFullName ()));

            return builder.ToString();
        }
    }
}
