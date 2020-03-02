/* 
 * CustomTextFieldConverter.cs
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
using System.Linq;
using FigmaSharp.Models;
using FigmaSharp.NativeControls;
using FigmaSharp.Views;

namespace FigmaSharp.Services
{
    public class NativeViewRenderingService : FigmaViewRendererService
	{
		public NativeViewRenderingService (IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters = null, FigmaViewPropertySetterBase propertySetter = null)
            : base (figmaProvider,
                  figmaViewConverters ?? NativeControlsContext.Current.GetConverters (),
                  propertySetter ?? NativeControlsContext.Current.GetViewPropertySetter()
                  )
		{

		}

		public override bool ProcessesImageFromNode (FigmaNode node)
		{
 			return node.IsImageNode () || node.IsFigmaImageViewNode ();
		}

		protected override bool NodeScansChildren (FigmaNode currentNode, CustomViewConverter converter, FigmaViewRendererServiceOptions options)
		{
			if (currentNode.IsFigmaImageViewNode ())
				return false;

			return base.NodeScansChildren (currentNode, converter, options);
		}

		protected override bool SkipsNode (FigmaNode currentNode, ProcessedNode parent, FigmaViewRendererServiceOptions options)
		{
			if (currentNode.IsDialog ()) {
				return true;
			}
			return false;
		}
    }
}
