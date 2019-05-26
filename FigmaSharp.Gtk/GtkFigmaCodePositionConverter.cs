namespace FigmaSharp.GtkSharp
{
    internal class GtkFigmaCodePositionConverter : FigmaCodePositionConverter
    {
        public override string ConvertToCode(string name, FigmaNode current)
        {
            if (current is IAbsoluteBoundingBox absoluteBounding && current.Parent is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
            {
                var x = absoluteBounding.absoluteBoundingBox.x - parentAbsoluteBoundingBox.absoluteBoundingBox.x;

                var parentY = parentAbsoluteBoundingBox.absoluteBoundingBox.y + parentAbsoluteBoundingBox.absoluteBoundingBox.height;
                var actualY = absoluteBounding.absoluteBoundingBox.y + absoluteBounding.absoluteBoundingBox.height;
                var y = parentY - actualY;
                return "";
            }
            return string.Empty;
        }
    }
}