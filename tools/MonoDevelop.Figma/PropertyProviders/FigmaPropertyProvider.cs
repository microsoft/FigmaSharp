using FigmaSharp.Models;
using MonoDevelop.DesignerSupport;

namespace MonoDevelop.Figma
{
    public class FigmaPropertyProvider : IPropertyProvider
    {
        public bool SupportsObject(object obj)
        {
            return obj is FigmaNode;
        }

        public object CreateProvider(object obj)
        {
            return obj;
        }
    }
}