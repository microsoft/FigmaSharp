namespace FigmaSharp
{
    internal class GtkFigmaCodePositionConverter : FigmaCodePositionConverter
    {
        public override string ConvertToCode(string name, ProcessedNode parentNode, ProcessedNode current)
        {
            if (current.FigmaNode is IAbsoluteBoundingBox absoluteBounding && parentNode.FigmaNode is IAbsoluteBoundingBox parentAbsoluteBoundingBox)
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