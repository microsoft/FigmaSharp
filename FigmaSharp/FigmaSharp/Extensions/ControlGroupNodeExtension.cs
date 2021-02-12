using FigmaSharp.Models;
using System.Linq;

namespace FigmaSharp.Extensions
{
    internal static class ControlGroupNodeExtensions
    {
        internal const string controlGroupId = "id";

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

        public static bool TrySearchControlGroupId(this FigmaNode figmaNode, out string id)
        {
            var controlGroupNode = figmaNode.GetControlGroupNode();
            if (controlGroupNode != null && controlGroupNode.TryGetChildPropertyValue(controlGroupId, out id))
            {
                return true;
            }
            id = null;
            return false;
        }
    }
}
