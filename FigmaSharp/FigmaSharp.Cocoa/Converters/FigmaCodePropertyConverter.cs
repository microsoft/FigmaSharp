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

using FigmaSharp.Models;
using FigmaSharp.Services;
using AppKit;

namespace FigmaSharp.Cocoa.Converters
{
    public class FigmaCodePropertyConverter : FigmaCodePropertyConverterBase
    {
        public override string ConvertToCode (string propertyName, FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			if (propertyName == CodeProperties.AddChild) {
                return parentNode?.GetMethod (nameof (NSView.AddSubview), currentNode.Name);
			}
            if (propertyName == CodeProperties.Size) {

                if (currentNode.Node is IAbsoluteBoundingBox container) {

					if (currentNode.Node is FigmaLine line) {
                        var width = container.absoluteBoundingBox.Width == 0 ? 1 : container.absoluteBoundingBox.Width;
						var height = container.absoluteBoundingBox.Height == 0 ? 1 : container.absoluteBoundingBox.Height;
                        var size = typeof (CoreGraphics.CGSize).GetConstructor (new string[] { width.ToDesignerString (), height.ToDesignerString () });
                        return currentNode.GetMethod (nameof (NSView.SetFrameSize), size);
					}

                    var sizeConstructor = typeof (CoreGraphics.CGSize).GetConstructor (
                     container.absoluteBoundingBox.Width.ToDesignerString (),
                     container.absoluteBoundingBox.Height.ToDesignerString ());
                    return currentNode.GetMethod (nameof (NSView.SetFrameSize), sizeConstructor);

                }
                return string.Empty;
            }
            if (propertyName == CodeProperties.Position) {
                //first level has an special behaviour on positioning 
                if (currentNode.Node.Parent is FigmaCanvas)
                    return string.Empty;

                if (currentNode.Node is IAbsoluteBoundingBox absoluteBounding && currentNode.Node.Parent is IAbsoluteBoundingBox parentAbsoluteBoundingBox) {
                    var x = absoluteBounding.absoluteBoundingBox.X - parentAbsoluteBoundingBox.absoluteBoundingBox.X;

                    var parentY = parentAbsoluteBoundingBox.absoluteBoundingBox.Y + parentAbsoluteBoundingBox.absoluteBoundingBox.Height;
                    var actualY = absoluteBounding.absoluteBoundingBox.Y + absoluteBounding.absoluteBoundingBox.Height;
                    var y = parentY - actualY;

                    if (x != default || y != default) {
                        var pointConstructor = CodeGenerationExtensions.GetConstructor (
                         typeof (CoreGraphics.CGPoint),
                         x.ToDesignerString (),
                         y.ToDesignerString ()
                    );
                        return currentNode.GetMethod (nameof (AppKit.NSView.SetFrameOrigin), pointConstructor);
                    }
                }
                return string.Empty;
            }

            throw new System.NotImplementedException (propertyName);
		}
    }
}
