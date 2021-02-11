using FigmaSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FigmaSharp.Extensions
{
    internal static class AccessibilityNodeExtensions
    {
        internal const string a11yLabel = "label";
        internal const string a11yHelp = "help";
        internal const string a11yRole = "role";
        internal const string tooltip = "tooltip";

        internal const string a11yRoleGroup = "group";

        internal const string a11yNodeName = "a11y";

        public static bool IsA11Group(this FigmaNode figmaNode)
        {
            var a11node = figmaNode.GetA11Node(); 
                
            if (a11node != null && a11node.TryGetChildPropertyValue(a11yRole, out var value) && value == a11yRoleGroup)
            {
                return true;
            }
            return false;
        }

        public static bool isA11yEnabled(this FigmaNode figmaNode)
        {
            return GetA11Node(figmaNode)?.visible ?? false;
        }

        public static FigmaNode GetA11Node(this FigmaNode figmaNode)
        {
            return (figmaNode as IFigmaNodeContainer)?.children?.FirstOrDefault(s => s.GetNodeTypeName() == a11yNodeName);
        }

        public static bool TrySearchA11Label(this FigmaNode figmaNode, out string label)
        {
            var a11Node = figmaNode.GetA11Node();
            if (a11Node != null && a11Node.TryGetChildPropertyValue(a11yLabel, out label))
            {
                return true;
            }
            label = null;
            return false;
        }

        public static bool TrySearchA11Help(this FigmaNode figmaNode, out string label)
        {
            var a11Node = figmaNode.GetA11Node();
            if (a11Node != null && a11Node.TryGetChildPropertyValue(a11yHelp, out label))
            {
                return true;
            }
            label = null;
            return false;
        }

        public static bool TrySearchTooltip(this FigmaNode figmaNode, out string label)
        {
            var a11Node = figmaNode.GetA11Node();
            if(a11Node != null && a11Node.TryGetChildPropertyValue(tooltip, out label))
            {
                return true;
            }
            label = null;
            return false;
        }
    }
}
