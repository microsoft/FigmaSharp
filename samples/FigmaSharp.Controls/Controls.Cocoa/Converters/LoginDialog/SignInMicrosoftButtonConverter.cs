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

using System;
using System.Linq;
using AppKit;
using FigmaSharp.Controls.Cocoa;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace LocalFile.Cocoa
{
    class SignInMicrosoftButtonConverter : NodeConverter
	{
		public const string SignInMicrosoftButtonName = "SignInMicrosoftButton";
		const string LogoImageName = "MSLogoImage";

		public override bool ScanChildren(FigmaNode currentNode) => false;

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.name == SignInMicrosoftButtonName;
		}

		public override IView ConvertTo(FigmaNode currentNode, ViewNode parent, RenderService rendererService)
		{
			string text = string.Empty;
			if (currentNode is IFigmaNodeContainer container)
			{
				var figmaText = container.children
					.OfType<FigmaText>()
					.FirstOrDefault();

				if (figmaText != null)
					text = figmaText.characters;
			}

			IView msLogoView = null;
			if (rendererService is ViewRenderService viewRendererService)
				msLogoView = viewRendererService.RenderByName<IView>(LogoImageName, null);
		
			var flatButton =  new FixedFlatButton(text, msLogoView.NativeObject as NSView);

			IButton button = TransitionHelper.CreateButtonFromFigmaNode (flatButton, currentNode);
			return button;
		}

		public override string ConvertToCode(CodeNode currentNode, CodeNode parentNode, CodeRenderService rendererService)
		{
			return string.Empty;
		}

        public override Type GetControlType(FigmaNode currentNode)
        {
            throw new NotImplementedException();
        }
    }
}

