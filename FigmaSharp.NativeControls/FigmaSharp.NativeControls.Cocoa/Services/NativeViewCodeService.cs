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
using System.Text;

using FigmaSharp.Cocoa;
using FigmaSharp.Models;
using FigmaSharp.NativeControls;

namespace FigmaSharp.Services
{
    public class NativeViewCodeService : FigmaCodeRendererService
	{
		public NativeViewCodeService (IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters, FigmaCodePropertyConverterBase codePropertyConverter) : base (figmaProvider, figmaViewConverters, codePropertyConverter)
		{

		}

		protected override void OnPostConvertToCode (StringBuilder builder, FigmaCodeNode node, FigmaCodeNode parent, FigmaViewConverter converter, FigmaCodePropertyConverterBase codePropertyConverter)
		{
			bool hasAccessibility = false;
			if (node.Node.IsA11Group ()) {
				var fullRoleName = $"{typeof(AppKit.NSAccessibilityRoles).FullName}.{nameof (AppKit.NSAccessibilityRoles.GroupRole)}";
				new AppKit.NSView ().AccessibilityRole = AppKit.NSAccessibilityRoles.GroupRole;
				builder.WriteEquality (node.Name, nameof (AppKit.NSView.AccessibilityRole), fullRoleName);
				hasAccessibility = true;
			}
			if (node.Node.TrySearchA11Label (out var label)) {
				builder.WriteEquality (node.Name, nameof(AppKit.NSView.AccessibilityLabel), label, inQuotes: true);
				hasAccessibility = true;
			}
			if (node.Node.TrySearchA11Help (out var help)) {
				builder.WriteEquality (node.Name, nameof (AppKit.NSView.AccessibilityHelp), help, inQuotes: true);
				hasAccessibility = true;
			}

			if (hasAccessibility)
				builder.AppendLine ();
		}

		bool TrySearchParameter (FigmaNode node, string parameter, out string value)
		{
			value = node.name;
			try {
				var index = value.IndexOf (parameter);
				if (index > -1 && index < value.Length) {
					value = value.Substring (index + parameter.Length);
					index = value.IndexOf ("\"");
					if (index > -1 && index < value.Length) {
						value = value.Substring (0, index);
						return true;
					}
				}
			} catch (Exception) {
			}
			value = null;
			return false;
		}

		#region Rendering

		internal override bool IsNodeSkipped (FigmaCodeNode node)
		{
			if (node.Node.IsDialogParentContainer ())
				return true;
			if (node.Node.IsWindowContent ())
				return true;
			return false;
		}

		internal override bool IsMainViewContainer (FigmaCodeNode node)
		{
			return node.Node.IsWindowContent ();
		}

		internal override FigmaNode[] GetChildrenToRender (FigmaCodeNode node)
		{
			if (node.Node is FigmaBoolean) {
				return new FigmaNode[0];
			}

			if (node.Node.IsDialogParentContainer (NativeControlType.WindowStandard)) {
				if (node.Node is IFigmaNodeContainer nodeContainer) {
					var item = nodeContainer.children.FirstOrDefault (s => s.IsNodeWindowContent ());
					if (item != null && item is IFigmaNodeContainer children) {
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

		internal override bool HasChildrenToRender (FigmaCodeNode node)
		{
			if (node.Node.IsDialogParentContainer ()) {
				return true;
			}
			return base.HasChildrenToRender (node);
		}

		protected override bool TryGetCodeViewName (FigmaCodeNode node, FigmaCodeNode parent, out string identifier)
		{
			if (node.Node.TryGetCodeViewName (out identifier)) {
				return true;
			}
			return base.TryGetCodeViewName (node, parent, out identifier);
		}

		#endregion
	}
}
