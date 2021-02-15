using FigmaSharp.Models;
using System.Linq;

namespace FigmaSharp.Extensions
{
    internal static class ControlGroupNodeExtensions
    {
        internal const string controlGroupName = "name";
        internal const string controlGroupTarget = "target";

        internal const string controlRoleGroup = "group";
        internal const string controlGroupRole = "role";

        internal const string controlGroupNodeName = "ControlGroup";

        public static bool IsControlGroup(this FigmaNode figmaNode)
        {
            var controlGroupNode = figmaNode.GetControlGroupNode(); 
                
            if (controlGroupNode != null && controlGroupNode.TryGetChildPropertyValue(controlGroupRole, out var value) && value == controlRoleGroup)
            {
                return true;
            }
            return false;
        }

        public static bool isControlGroupEnabled(this FigmaNode figmaNode)
        {
            return GetControlGroupNode(figmaNode)?.visible ?? false;
        }

        public static FigmaNode GetControlGroupNode(this FigmaNode figmaNode)
        {
            return (figmaNode as IFigmaNodeContainer)?.children?.FirstOrDefault(s => s.GetNodeTypeName() == controlGroupNodeName);
        }

        public static bool TrySearchControlGroupName(this FigmaNode figmaNode, out string name)
        {
            var controlGroupNode = figmaNode.GetControlGroupNode();
            if (controlGroupNode != null && controlGroupNode.TryGetChildPropertyValue(controlGroupName, out name))
            {
                return true;
            }
            name = null;
            return false;
        }
        public static bool TrySearchControlGroupTarget(this FigmaNode figmaNode, out string target)
        {
            var controlGroupNode = figmaNode.GetControlGroupNode();
            if (controlGroupNode != null && controlGroupNode.TryGetChildPropertyValue(controlGroupTarget, out target))
            {
                return true;
            }
            target = null;
            return false;
        }
    }
}
