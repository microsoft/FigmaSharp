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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FigmaSharp.Controls.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Services;

namespace FigmaSharp
{
    public class ShowContentMethodCodeObject : ClassMethodCodeObject
    {
		readonly string contentViewName;
		readonly string selectedContentName;
		public string SelectedContentName => selectedContentName;

		readonly string argumentName;
		readonly string enumTypeName;
		public List<FigmaFrame> figmaFrameEntities;

		CodeNode parentNode;
		CodeRenderService rendererService;

		const int DefaultWindowBarHeight = 22;

		public ShowContentMethodCodeObject(List<FigmaFrame> figmaFrames, string name, string contentViewName, string enumTypeName, CodeNode parentNode, CodeRenderService figmaRendererService) : base (name)
        {
			MethodModifier = CodeObjectModifierType.Public;
			argumentName = "content";
			selectedContentName = "SelectedContent";
			figmaFrameEntities = figmaFrames;

			this.rendererService = figmaRendererService;
			this.parentNode = parentNode;
			this.contentViewName = contentViewName;
			this.enumTypeName = enumTypeName;
		}

		public override void Write (FigmaClassBase figmaClassBase, StringBuilder sb)
		{
			figmaClassBase.AddTabLevel ();

			figmaClassBase.GenerateMethod (sb, Name, CodeObjectModifierType.Protected,
				arguments: new List<(string, string)>() {(enumTypeName, argumentName) });

			figmaClassBase.AppendLine (sb, $"{contentViewName}?.{nameof(AppKit.NSView.RemoveFromSuperview)}();");

			figmaClassBase.AppendLine(sb, $"{selectedContentName} = {argumentName};");

			var frameName = "frame";
			figmaClassBase.AppendLine(sb, $"var {frameName} = {typeof (CoreGraphics.CGRect).FullName}.{nameof (CoreGraphics.CGRect.Empty)};");

			for (int i = 0; i < figmaFrameEntities.Count; i++)
			{
				string ifcase = "if";
				if (i > 0)
					ifcase = "else " + ifcase;

				var className = figmaFrameEntities[i].GetClassName();

				figmaClassBase.AppendLine(sb, $"{ifcase} ({argumentName} == {enumTypeName}.{className}) {{");
				figmaClassBase.AddTabLevel();
				figmaClassBase.AppendLine(sb, $"{contentViewName} = new {className}();");
				figmaClassBase.AppendLine(sb, $"{nameof(AppKit.NSWindow.ContentView)}.{nameof(AppKit.NSView.AddSubview)}({contentViewName});");

				//var leftConstraintStringValue = CodeGenerationHelpers.GetLeftConstraintEqualToAnchor(
				//	contentViewName, figmaFrameEntities[i].Item2.Left, nameof(AppKit.NSWindow.ContentView));
				//figmaClassBase.AppendLine(sb, $"{figmaFrameEntities[i].Item2}.{nameof (AppKit.NSLayoutConstraint.Active)} = {true.ToDesignerString ()};");

				//var topConstraintStringValue = CodeGenerationHelpers.GetLeftConstraintEqualToAnchor(
				//	contentViewName, figmaFrameEntities[i].Item2.Left, nameof(AppKit.NSWindow.ContentView));
				//figmaClassBase.AppendLine(sb, $"{figmaFrameEntities[i].Item2}.{nameof(AppKit.NSLayoutConstraint.Active)} = {true.ToDesignerString()};");

				var parentNode = new CodeNode(figmaFrameEntities[i], nameof(AppKit.NSWindow.ContentView));
				var nodeContent = figmaFrameEntities[i].children.FirstOrDefault(s => s.IsNodeWindowContent());

				//hack:
				var oldboundingBox = figmaFrameEntities[i].absoluteBoundingBox;
				figmaFrameEntities[i].absoluteBoundingBox = new Rectangle(oldboundingBox.X, oldboundingBox.Y + DefaultWindowBarHeight, oldboundingBox.Width, oldboundingBox.Height - DefaultWindowBarHeight);

				var codeNode = new CodeNode(nodeContent, contentViewName, parent: parentNode);

				var converter = rendererService.GetNodeConverter(codeNode);
				var frameCode = rendererService.codePropertyConverter.ConvertToCode(PropertyNames.Frame, codeNode, parentNode, converter, rendererService);

				foreach (var line in frameCode.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
					figmaClassBase.AppendLine(sb, line);

				var contraintsCode = rendererService.codePropertyConverter.ConvertToCode(PropertyNames.Constraints, codeNode, parentNode, converter, rendererService);
                foreach (var line in contraintsCode.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
					figmaClassBase.AppendLine(sb, line);

				figmaFrameEntities[i].absoluteBoundingBox = oldboundingBox;

				figmaClassBase.RemoveTabLevel();

				figmaClassBase.CloseBracket(sb,removeTabIndex: false);
			}
			figmaClassBase.RemoveTabLevel();

			figmaClassBase.CloseBracket(sb);
		}
	}

	public class FigmaContainerBundleWindow : FigmaBundleWindow
	{
		public IEnumerable<FigmaFrame> ReferencedWindows { get; private set; }

		public FigmaContainerBundleWindow(FigmaBundle figmaBundle, string viewName, FigmaNode figmaNode) : base(figmaBundle, viewName, figmaNode)
		{

		}

		Rectangle GetRectangle (FigmaFrame currentNode)
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

		protected override void OnGetPartialDesignerClass(FigmaPartialDesignerClass partialDesignerClass, CodeRenderService codeRendererService, bool translateLabels)
		{
			base.OnGetPartialDesignerClass(partialDesignerClass, codeRendererService, translateLabels);

			var fileProvider = codeRendererService.NodeProvider;

			var windows = GetReferencedWindows (fileProvider, FigmaNode);

			var converter = codeRendererService.codePropertyConverter;

			var names = windows.OfType <FigmaFrame> ()
				.ToList ();

			var enumName = "Content";
			var enumCodeObject = new EnumCodeObject(enumName, names);
			partialDesignerClass.Methods.Add(enumCodeObject);

			var currentContent = "currentContent";
			var contentName = "ShowContent";

			var figmaCode = new CodeNode(FigmaNode);
			var contentClassMethod = new ShowContentMethodCodeObject (names, contentName, currentContent, enumName, figmaCode, codeRendererService);
            partialDesignerClass.Methods.Add (contentClassMethod);
			partialDesignerClass.PrivateMembers.Add ((typeof(AppKit.NSView).FullName, currentContent));
			partialDesignerClass.PrivateMembers.Add ((enumName, contentClassMethod.SelectedContentName));
		}

		static IEnumerable<FigmaNode> GetReferencedWindows (INodeProvider fileProvider, FigmaNode node)
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
