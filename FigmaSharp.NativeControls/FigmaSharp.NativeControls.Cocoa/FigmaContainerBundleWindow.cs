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
using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.NativeControls;

namespace FigmaSharp
{
    public class ShowContentMethodCodeObject : ClassMethodCodeObject
    {
		readonly string contentViewName;
		readonly string selectedContentName;
		public string SelectedContentName => selectedContentName;

		readonly string argumentName;
		readonly string enumTypeName;
		public List<(string, Rectangle)> figmaFrameEntities;

        public ShowContentMethodCodeObject(List<(string, Rectangle)> figmaFrames, string name, string contentViewName, string enumTypeName) : base (name)
        {
			MethodModifier = CodeObjectModifier.Public;
			argumentName = "content";
			selectedContentName = "SelectedContent";
			figmaFrameEntities = figmaFrames;

			this.contentViewName = contentViewName;
			this.enumTypeName = enumTypeName;
		}

		public override void Write (FigmaClassBase figmaClassBase, StringBuilder sb)
		{
			figmaClassBase.AddTabLevel ();

			figmaClassBase.GenerateMethod (sb, Name, CodeObjectModifier.Protected,
				arguments: new List<(string, string)>() {(enumTypeName, argumentName) });

			figmaClassBase.AppendLine (sb, $"{contentViewName}?.{nameof(AppKit.NSView.RemoveFromSuperview)}();");

			figmaClassBase.AppendLine(sb, $"{selectedContentName} = {argumentName};");

			var frameName = "frame";
			figmaClassBase.AppendLine(sb, $"var {frameName} = {typeof (CoreGraphics.CGRect).FullName}.{nameof (CoreGraphics.CGRect.Empty)};");

			//switch
			figmaClassBase.AppendLine(sb, $"switch ({argumentName})");
			figmaClassBase.OpenBracket(sb);

			for (int i = 0; i < figmaFrameEntities.Count; i++)
			{
				var className = figmaFrameEntities[i].Item1;
				if (i > 0)
					figmaClassBase.RemoveTabLevel();

				figmaClassBase.AppendLine(sb, $"case {enumTypeName}.{className}:");
				figmaClassBase.AddTabLevel();
				figmaClassBase.AppendLine(sb, $"{contentViewName} = new {className}();");

				var rect = figmaFrameEntities[i].Item2.ToCGRect().ToDesignerString();
				figmaClassBase.AppendLine(sb, $"{frameName} = {contentViewName}.{nameof(AppKit.NSView.GetFrameForAlignmentRect)} ({rect});");
				figmaClassBase.AppendLine(sb, "break;");
			}

			figmaClassBase.RemoveTabLevel();
			figmaClassBase.CloseBracket (sb);

			//if case
			figmaClassBase.AppendLine(sb, string.Empty);

			figmaClassBase.AddTabLevel();
			figmaClassBase.AppendLine(sb, $"if ({contentViewName} != null)");
			figmaClassBase.OpenBracket(sb);
			figmaClassBase.AppendLine(sb, $"{nameof(AppKit.NSWindow.ContentView)}.{nameof(AppKit.NSView.AddSubview)}({contentViewName});");

			figmaClassBase.AppendLine(sb, $"{contentViewName}.{nameof(AppKit.NSView.Frame)} = {frameName};");

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

		Rectangle GetRectangle (FigmaFrameEntity currentNode)
        {
			var content = currentNode.FirstChild(s => s.IsNodeWindowContent());
			if (content is IAbsoluteBoundingBox absoluteBounding && currentNode is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
			{
				var x = absoluteBounding.absoluteBoundingBox.X - parentAbsoluteBoundingBox.absoluteBoundingBox.X;

				var parentY = parentAbsoluteBoundingBox.absoluteBoundingBox.Y + parentAbsoluteBoundingBox.absoluteBoundingBox.Height;
				var actualY = absoluteBounding.absoluteBoundingBox.Y + absoluteBounding.absoluteBoundingBox.Height;
				var y = parentY - actualY;

				return new Rectangle(x, y, absoluteBounding.absoluteBoundingBox.Width, absoluteBounding.absoluteBoundingBox.Height);
			}
			return default;
		}

		protected override void OnGetPartialDesignerClass(FigmaPartialDesignerClass partialDesignerClass, FigmaCodeRendererService codeRendererService, bool translateLabels)
		{
			base.OnGetPartialDesignerClass(partialDesignerClass, codeRendererService, translateLabels);

			var fileProvider = codeRendererService.figmaProvider;

			var windows = GetReferencedWindows (fileProvider, FigmaNode);

			var converter = codeRendererService.codePropertyConverter;

			var names = windows.OfType <FigmaFrameEntity> ()
				.Select(s =>  (s.GetClassName(), GetRectangle(s)))
				.ToList ();

			var enumName = "Content";
			var enumCodeObject = new EnumCodeObject(enumName, names);
			partialDesignerClass.Methods.Add(enumCodeObject);

			var currentContent = "currentContent";
			var contentName = "ShowContent";

			var contentClassMethod = new ShowContentMethodCodeObject (names, contentName, currentContent, enumName);
            partialDesignerClass.Methods.Add (contentClassMethod);
			partialDesignerClass.PrivateMembers.Add ((typeof(AppKit.NSView).FullName, currentContent));
			partialDesignerClass.PrivateMembers.Add ((enumName, contentClassMethod.SelectedContentName));
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
