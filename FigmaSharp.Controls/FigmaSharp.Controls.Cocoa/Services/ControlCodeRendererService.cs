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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Services;

namespace FigmaSharp.Controls.Cocoa.Services
{
    public class NativeViewCodeService : CodeRenderService
    {
		public List<(string memberType, string name)> PrivateMembers = new List<(string memberType, string name)>();

		public NativeViewCodeService(INodeProvider figmaProvider, NodeConverter[] figmaViewConverters = null, CodePropertyConfigureBase codePropertyConverter = null, ITranslationService translationService = null) : base(figmaProvider, figmaViewConverters ?? FigmaControlsContext.Current.GetConverters(true),
			codePropertyConverter ?? FigmaControlsContext.Current.GetCodePropertyConverter(), translationService)
		{

		}

		internal override bool NeedsRenderConstructor(CodeNode node, CodeNode parent)
		{
			if (parent != null && IsMainNode(parent.Node) && (Options?.RendersConstructorFirstElement ?? false))
				return false;
			return true;
		}

		#region Rendering

		internal override bool IsNodeSkipped (CodeNode node)
		{
            if (node.Node is FigmaInstance nodeInstance)
            {
                if (NodeProvider.TryGetMainInstance(nodeInstance, out var maincomponent) && maincomponent.Parent is FigmaCanvas)
                {
					return true;
				}
            }

            if (node.Node.Parent is FigmaCanvas && node.Node is FigmaFrame) {
                return true;
            }

            if (node.Node.IsDialogParentContainer())
				return true;

			if (node.Node.IsWindowContent())
				return true;

			if (node.Node.IsParentMainContainerContent ())
				return true;

			return false;
		}

		internal override bool IsMainViewContainer (CodeNode node)
		{
			return node.Node.IsWindowContent ();
		}

		internal override FigmaNode[] GetChildrenToRender (CodeNode node)
		{
			if (node.Node is FigmaBoolean) {
				return new FigmaNode[0];
			}

			if (node.Node.IsDialogParentContainer (FigmaControlType.Window)) {
				if (node.Node is IFigmaNodeContainer nodeContainer) {
					var item = nodeContainer.children.FirstOrDefault (s => s.IsNodeWindowContent ());
					if (item != null && item is IFigmaNodeContainer children) {
						//instance of a component is not code rendered (we create an additional class)
						if (node.Node is FigmaInstance)
							return new FigmaNode[0];
						return children.children;
					} else {
						var instance = node.Node.GetDialogInstanceFromParentContainer ();
						//render all children nodes except instance
						var nodes = nodeContainer.children.Except (new FigmaNode[] { instance })
							.ToArray ();
						return nodes;
					}
				}
			}
			return base.GetChildrenToRender (node);
		}

		internal override bool HasChildrenToRender (CodeNode node)
		{
			if (node.Node is FigmaInstance nodeInstance)
			{
				if (NodeProvider.TryGetMainInstance(nodeInstance, out _))
					return true;
			}

			if (node.Node.IsDialogParentContainer ()) {
				return true;
			}
			return base.HasChildrenToRender (node);
		}

		protected override bool TryGetCodeViewName (CodeNode node, CodeNode parent, NodeConverter converter, out string identifier)
		{
			if (node.Node.TryGetCodeViewName (out identifier)) {
				return true;
			}
			return base.TryGetCodeViewName (node, parent, converter, out identifier);
		}

		protected override void OnStartGetCode()
		{
			PrivateMembers.Clear();
		}

		public override void Clear()
		{
			base.Clear();
		}

		protected override void OnPostConvertToCode (StringBuilder builder, CodeNode node, CodeNode parent, NodeConverter converter, CodePropertyConfigureBase codePropertyConverter)
		{
			if (!NodeRendersVar (node, parent)) {
				if (node.Node.TryGetNodeCustomName (out string name)) {
					var controlType = converter.GetControlType(node.Node);
					PrivateMembers.Add ((controlType.FullName, name));
				}
			}
			base.OnPostConvertToCode (builder, node, parent, converter, codePropertyConverter);
		}


        protected override bool RendersAddChild(CodeNode node, CodeNode parent, CodeRenderService figmaCodeRendererService)
        {
			return true;
		}

        protected override bool RendersSize(CodeNode node, CodeNode parent, CodeRenderService figmaCodeRendererService)
        {
			return true;
		}

        protected override bool RendersConstraints(CodeNode node, CodeNode parent, CodeRenderService rendererService)
        {
			if (node.Node.IsDialogParentContainer())
				return false;
			if (node.Node.IsNodeWindowContent())
				return false;
			return base.RendersConstraints(node, parent, rendererService);
		}

		const string IsFlexibleHorizontalParameter = "isFlexibleHorizontal";
		const string IsFlexibleVerticalParameter = "isFlexibleVertical";

		internal override bool IsFlexibleHorizontal(CodeNode codeNode, NodeConverter converter)
        {
			if (codeNode.Node.TryGetAttributeValue(IsFlexibleHorizontalParameter, out var value) && value == "true")
				return true;
            return base.IsFlexibleHorizontal(codeNode, converter);
		}
	
		internal override bool IsFlexibleVertical(CodeNode codeNode, NodeConverter converter)
        {
			if (codeNode.Node.TryGetAttributeValue(IsFlexibleVerticalParameter, out var value) && value == "true")
				return true;
			return base.IsFlexibleVertical(codeNode, converter);
        }

        #endregion
    }
}
