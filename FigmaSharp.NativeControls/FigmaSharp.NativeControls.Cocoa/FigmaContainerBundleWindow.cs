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
using System.Linq;

using System.Collections.Generic;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.NativeControls;

namespace FigmaSharp
{
	public class ShowContentClassMethod : ClassMethod
	{
		readonly string masterContentViewName;
		readonly string contentViewName;
		readonly string argumentName;

		public IEnumerable<FigmaNode> figmaFrameEntities;

        public ShowContentClassMethod(IEnumerable<FigmaNode> figmaFrames, string masterContentViewName, string contentViewName)
        {
            MethodModifier = MethodModifier.Public;
			MethodName = "ShowContent";
			argumentName = "content";

			figmaFrameEntities = figmaFrames;

			this.masterContentViewName = masterContentViewName;
            this.contentViewName = contentViewName;
        }

		public override void Write (FigmaClassBase figmaClassBase, StringBuilder sb)
		{
			figmaClassBase.AddTabLevel ();

			figmaClassBase.GenerateMethod (sb, MethodName, MethodModifier.Protected,
				arguments: new List<(string, string)>() {(typeof (string).FullName, argumentName) });

			figmaClassBase.AppendLine (sb, $"{contentViewName}?.{nameof(AppKit.NSView.RemoveFromSuperview)}();");

			for (int i = 0; i < figmaFrameEntities.Count(); i++)
			{
				var frameEntity = figmaFrameEntities.ElementAt(i);
				var className = frameEntity.GetClassName();

				string ifCase = i == 0 ? "if" : "else if";
				ifCase = $"{ifCase} ({argumentName} == \"{className}\")";
				figmaClassBase.AppendLine(sb, ifCase);
				figmaClassBase.AppendLine(sb, $"{contentViewName} = new {className}();");
			}

			figmaClassBase.AppendLine(sb, $"if ({argumentName} != null)");
			figmaClassBase.OpenBracket(sb);
			figmaClassBase.AppendLine(sb, $"{masterContentViewName}.{nameof(AppKit.NSView.AddSubview)}({contentViewName});");
			figmaClassBase.AppendLine(sb, $"{contentViewName}.{nameof (AppKit.NSView.Frame)} = {masterContentViewName}.{nameof(AppKit.NSView.Bounds)};");

			figmaClassBase.RemoveTabLevel();

			figmaClassBase.CloseBracket(sb);

			figmaClassBase.CloseBracket(sb);
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

			var currentContent = "currentContent";

			var contentClassMethod = new ShowContentClassMethod (windows, "masterContent", currentContent);
            partialDesignerClass.Methods.Add (contentClassMethod);
			partialDesignerClass.PrivateMembers.Add((typeof(AppKit.NSView), currentContent));
		}

		static IEnumerable<FigmaNode> GetReferencedWindows (IFigmaFileProvider fileProvider, FigmaNode node)
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
                            yield return mainLayer;
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
