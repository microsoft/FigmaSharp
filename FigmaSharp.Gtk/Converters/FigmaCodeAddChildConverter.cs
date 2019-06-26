namespace FigmaSharp.GtkSharp
{
    internal class FigmaCodeAddChildConverter : FigmaCodeAddChildConverterBase
    {
        public override string ConvertToCode(string parent, string current, FigmaNode currentNode)
        {
            return string.Format("{0}.Add({1});", parent, current);
        }
    }
}