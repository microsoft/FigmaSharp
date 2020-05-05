
using FigmaSharp.Models;

namespace FigmaSharp.Services
{
	public class FigmaCodeNode
	{
		public FigmaCodeNode (FigmaNode node, string name = null, bool isClass = false, FigmaCodeNode parent = null)
		{
			Node = node;
			Name = name;
			IsClass = isClass;
			Parent = parent;
		}

		public bool HasName => !string.IsNullOrEmpty (Name);

		public FigmaNode Node { get; private set; }

		public FigmaCodeNode Parent { get; private set; }

		public string Name { get; set; }

		public bool IsClass { get; private set; }
	}
}
