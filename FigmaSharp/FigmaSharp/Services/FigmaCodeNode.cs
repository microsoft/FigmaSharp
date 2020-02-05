
using FigmaSharp.Models;

namespace FigmaSharp.Services
{
	public class FigmaCodeNode
	{
		public FigmaCodeNode (FigmaNode node, string name = null, bool isClass = false)
		{
			Node = node;
			Name = name;
			IsClass = isClass;
		}

		public bool HasName => !string.IsNullOrEmpty (Name);

		public FigmaNode Node { get; set; }
		public string Name { get; set; }

		public bool IsClass { get; set; }
	}
}
