
using FigmaSharp.Models;

namespace FigmaSharp.Services
{
    public class CodeNode : ContainerNode
	{
		public CodeNode (FigmaNode node, string name = null, bool isClass = false, CodeNode parent = null) : base (node)
		{
			Name = name;
			IsClass = isClass;
			Parent = parent;
		}

		public bool HasName => !string.IsNullOrEmpty (Name);

		public CodeNode Parent { get; private set; }

		public string Name { get; set; }

		public bool IsClass { get; private set; }
	}
}
