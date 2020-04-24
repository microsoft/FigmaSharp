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
using System.Text;
using System.Collections.Generic;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.NativeControls;

namespace FigmaSharp
{
	public class ShowContentClassMethod : ClassMethod
	{
        public IEnumerable<FigmaFrameEntity> figmaFrameEntities;

        public ShowContentClassMethod(IEnumerable<FigmaFrameEntity> figmaFrames)
		{
            this.figmaFrameEntities = figmaFrames;
        }

		internal override void Write(FigmaClassBase figmaClassBase, StringBuilder sb)
		{
			foreach (var frameEntity in figmaFrameEntities)
			{
                figmaClassBase.AppendLine(sb, frameEntity.id);
			}
		}
	}

	public class FigmaContainerBundleWindow : FigmaBundleWindow
	{
		public IEnumerable<FigmaFrameEntity> ReferencedWindows { get; private set; }

		public FigmaContainerBundleWindow(FigmaBundle figmaBundle, string viewName, FigmaNode figmaNode) : base(figmaBundle, viewName, figmaNode)
		{

		}

		protected override void OnGetPartialDesignerClass(FigmaPartialDesignerClass partialDesignerClass, FigmaCodeRendererService codeRendererService, bool translateLabels)
		{
			base.OnGetPartialDesignerClass(partialDesignerClass, codeRendererService, translateLabels);

			var fileProvider = codeRendererService.figmaProvider;
			var windows = GetReferencedWindows (fileProvider, FigmaNode);
            var contentClassMethod = new ShowContentClassMethod (windows);
            partialDesignerClass.Methods.Add (contentClassMethod);
        }

		static IEnumerable<FigmaFrameEntity> GetReferencedWindows (IFigmaFileProvider fileProvider, FigmaNode node)
		{
            foreach (var mainLayer in fileProvider.GetMainGeneratedLayers())
            {
                if (mainLayer.id == node.id)
                    continue;
                if (mainLayer is IFigmaNodeContainer container)
                {
                    foreach (var item in container.children)
                    {
                        if (item is FigmaInstance entity && entity.componentId == node.id)
                        {
                            yield return entity;
                            break;
                        }
                    }
                }
            }
        }

		protected override void OnGetPublicDesignerClass(FigmaPublicPartialClass publicPartialClass)
		{
			base.OnGetPublicDesignerClass(publicPartialClass);
		}
	}
}
