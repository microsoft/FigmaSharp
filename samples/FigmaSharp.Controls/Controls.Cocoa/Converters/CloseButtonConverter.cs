/* 
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

using FigmaSharp;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace LocalFile.Cocoa
{
	class CloseButtonConverter : ViewConverter
	{
		public const string CloseButtonId = "window-close";

		public override bool CanConvert(FigmaNode currentNode) => currentNode.name == CloseButtonId;

		public override IView ConvertTo(FigmaNode currentNode, ViewNode parent, FigmaRendererService rendererService)
		{
			var button = FigmaSharp.Controls.Cocoa.TransitionHelper.CreateImageButtonFromFigmaNode(currentNode);
			//button.Border = false;
			return button;
		}
		public override bool ScanChildren(FigmaNode currentNode) => false;
		public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, FigmaCodeRendererService rendererService) => string.Empty;

		public override System.Type GetControlType(FigmaNode currentNode) => typeof(AppKit.NSView);
	}
}

