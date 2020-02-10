using System;
using FigmaSharp;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.Services;
using System.Linq;
using FigmaSharp.NativeControls.Cocoa;
using FigmaSharp.NativeControls;
using System.Text;
using FigmaSharp.Cocoa;

namespace FigmaSharp.Services
{
	public class NativeViewCodeService : FigmaCodeRendererService
	{
		public NativeViewCodeService (IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters, FigmaCodePropertyConverterBase codePropertyConverter) : base (figmaProvider, figmaViewConverters, codePropertyConverter)
		{

		}

		const string a11yLabel = "a11y-label:\"";
		const string a11yHelp = "a11y-help:\"";
		const string a11yGroup = "a11y-group";

		string GetAccessibilityTitleName (FigmaNode node)
		{
			return nameof (AppKit.NSView.AccessibilityTitle);
			//if (node.TryGetNativeControlType (out var nativeControlType)) {
			//	switch (nativeControlType) {
			//		case NativeControlType.Button:
			//		case NativeControlType.CheckBox:
			//		case NativeControlType.PopupButton:
			//			return nameof (AppKit.NSView.AccessibilityTitle);
			//	}
			//}
			//return nameof (AppKit.NSView.AccessibilityLabel);
		}

		protected override void OnPostConvertToCode (StringBuilder builder, FigmaCodeNode node, FigmaCodeNode parent, FigmaViewConverter converter, FigmaCodePropertyConverterBase codePropertyConverter)
		{
			bool hasAccessibility = false;
			if (node.Node.name.Contains (a11yGroup)) {
				var fullRoleName = $"{typeof(AppKit.NSAccessibilityRoles).FullName}.{nameof (AppKit.NSAccessibilityRoles.GroupRole)}";
				new AppKit.NSView ().AccessibilityRole = AppKit.NSAccessibilityRoles.GroupRole;
				builder.WriteEquality (node.Name, nameof (AppKit.NSView.AccessibilityRole), fullRoleName);
				hasAccessibility = true;
			}
			if (TrySearchParameter (node.Node, a11yLabel, out var label)) {
				builder.WriteEquality (node.Name, GetAccessibilityTitleName (node.Node), label, inQuotes: true);
				hasAccessibility = true;
			}
			if (TrySearchParameter (node.Node, a11yHelp, out var help)) {
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
			if (node.Node.TryGetNodeCustomName (out identifier)) {
				return true;
			}
			return base.TryGetCodeViewName (node, parent, out identifier);
		}

		#endregion
	}
}
