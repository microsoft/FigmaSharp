
using FigmaSharp.Models;

namespace FigmaSharp.Services
{
	public class CodeNode
	{
		public CodeNode (FigmaNode node, string name = null, bool isClass = false, CodeNode parent = null)
		{
			Node = node;
			Name = name;
			IsClass = isClass;
			Parent = parent;
		}

		public bool HasName => !string.IsNullOrEmpty (Name);

		public FigmaNode Node { get; private set; }

		public CodeNode Parent { get; private set; }

		public string Name { get; set; }

		public bool IsClass { get; private set; }
	}
}
