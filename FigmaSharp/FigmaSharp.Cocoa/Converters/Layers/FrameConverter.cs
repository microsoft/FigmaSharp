/* 
 * FrameConverter.cs
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
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Views.Cocoa;

namespace FigmaSharp.Cocoa.Converters
{
    public class FrameConverter : FrameConverterBase
    {
        public override Type GetControlType(FigmaNode currentNode) => typeof(NSView);

        public override bool ScanChildren (FigmaNode currentNode)
            => true;

        public override IView ConvertToView (FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
        {
            IView view;
            if (rendererService.FileProvider.RendersAsImage (currentNode))
                view = new ImageView ();
			else
                view  = new View ();

            var currengroupView = view.NativeObject as NSView;
            var FigmaFrame = (FigmaFrame)currentNode;
            currengroupView.Configure(currentNode);

            currengroupView.AlphaValue = FigmaFrame.opacity;

			if (FigmaFrame.HasFills) {
                foreach (var fill in FigmaFrame.fills) {
					if (fill.type == "IMAGE") {
						//we need to add this to our service
                    } else if (fill.type == "SOLID") {
                       if (fill.visible) {
                            currengroupView.Layer.BackgroundColor = fill.color.ToCGColor ();
                        }
                    } else {
                        Console.WriteLine ($"NOT IMPLEMENTED FILL : {fill.type}");
					}
                    //currengroupView.Layer.Hidden = !fill.visible;
                }
            }
		
            return view;
        }

        public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
        {
            var FigmaFrame = (FigmaFrame)currentNode.Node;
            StringBuilder builder = new StringBuilder();

            var name = Resources.Ids.Conversion.NameIdentifier;

            if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
                builder.WriteConstructor(name, GetControlType(currentNode.Node), rendererService.NodeRendersVar(currentNode, parentNode));

            builder.Configure(FigmaFrame, currentNode.Name);
            return builder.ToString();
        }
    }
}
