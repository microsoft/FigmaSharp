
using FigmaSharp.Models;

namespace FigmaSharp.Services
{
    public class ContainerNode
    {
        public FigmaNode Node { get; private set; }

        public ContainerNode(FigmaNode figmaNode)
        {
            Node = figmaNode;
        }
    }
}
