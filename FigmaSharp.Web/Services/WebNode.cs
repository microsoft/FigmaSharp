using FigmaSharp.Models;

namespace FigmaSharp.Web.Services
{
    public class WebNode
    {
        public WebNode(FigmaNode node, string name = null, bool isClass = false, WebNode parent = null)
        {
            Node = node;
            Name = name;
            IsClass = isClass;
            Parent = parent;
        }

        public bool HasName => !string.IsNullOrEmpty(Name);

        public FigmaNode Node { get; private set; }

        public WebNode Parent { get; private set; }

        public string Name { get; set; }

        public bool IsClass { get; private set; }
    }
}
