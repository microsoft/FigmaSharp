using FigmaSharp.Models;

namespace FigmaSharp.GtkSharp
{
    internal class FigmaCodePositionConverter : FigmaCodePositionConverterBase
    {
        public override string ConvertToCode(string parent, string name, FigmaNode current)
        {
            if (current is IAbsoluteBoundingBox absoluteBounding && current.Parent is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
            {
                var x = (int) (absoluteBounding.absoluteBoundingBox.x - parentAbsoluteBoundingBox.absoluteBoundingBox.x);
                var parentY = parentAbsoluteBoundingBox.absoluteBoundingBox.y + parentAbsoluteBoundingBox.absoluteBoundingBox.height;
                var actualY = absoluteBounding.absoluteBoundingBox.y + absoluteBounding.absoluteBoundingBox.height;
                var y = (int) (parentY - actualY);
                return string.Format("{0}.Move({1},{2},{3});", parent, name, x.ToDesignerString(), y.ToDesignerString());
            }
            return string.Empty;
        }
    }
}