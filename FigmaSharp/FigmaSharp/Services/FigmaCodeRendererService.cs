using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FigmaSharp.Models;

namespace FigmaSharp.Services
{
	public class FigmaCodeRendererServiceOptions
	{
		public IColorConverter ColorConverter { get; set; }
		public bool RendersConstructorFirstElement { get; set; }
		public bool TranslateLabels { get; set; }
	}

	public class FigmaCodeRendererService
	{
		internal const string DefaultViewName = "view";

		internal IFigmaFileProvider figmaProvider;

		internal FigmaCodePropertyConverterBase codePropertyConverter;

		FigmaViewConverter[] figmaConverters;
		FigmaViewConverter[] customConverters;

		public FigmaCodeRendererService (IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters,
			FigmaCodePropertyConverterBase codePropertyConverter)
		{
			this.customConverters = figmaViewConverters.Where (s => !s.IsLayer).ToArray ();
			this.figmaConverters = figmaViewConverters.Where (s => s.IsLayer).ToArray (); ;
			this.figmaProvider = figmaProvider;
			this.codePropertyConverter = codePropertyConverter;
		}

		FigmaViewConverter GetConverter (FigmaCodeNode node, FigmaViewConverter[] converters)
		{
			foreach (var customViewConverter in converters) {
				if (customViewConverter.CanConvert (node.Node)) {
					return customViewConverter;
				}
			}
			return null;
		}

		internal FigmaCodeRendererServiceOptions CurrentRendererOptions { get; set; }
		internal FigmaCodeNode ParentMainNode { get; set; }
		internal FigmaCodeNode MainNode { get; set; }

		public bool IsMainNode (FigmaNode figmaNode) => MainNode != null && figmaNode == MainNode?.Node;


		readonly internal List<FigmaCodeNode> Nodes = new List<FigmaCodeNode>();

		public virtual void Clear ()
		{
			CurrentRendererOptions = null;
			ParentMainNode = null;
			MainNode = null;
		}

		public void GetCode (StringBuilder builder, FigmaCodeNode node, FigmaCodeNode parent = null, FigmaCodeRendererServiceOptions currentRendererOptions = null)
		{
			//in first level we clear all identifiers
			if (parent == null) {
				if (MainNode == null) {

					//clear all nodes
					Nodes.Clear();

					identifiers.Clear ();
					OnStartGetCode ();
					
					//we initialize
					CurrentRendererOptions = currentRendererOptions ?? new FigmaCodeRendererServiceOptions ();

					//we store our main node
					MainNode = node;
					ParentMainNode = parent;
				}
			}

			if (node != null)
				Nodes.Add(node);

			FigmaCodeNode calculatedParentNode = null;
			FigmaViewConverter converter = null;

			var isNodeSkipped = IsNodeSkipped (node);

			//on node skipped we don't render
			if (!isNodeSkipped) {
				//if (figmaProvider.RendersProperties (node)) {
				converter = GetConverter (node, customConverters);
				//bool navigateChild = true;
				if (converter == null) {
					converter = GetConverter (node, figmaConverters);
				}

				if (converter != null) {
					if (!node.HasName) {

						if (!TryGetCodeViewName (node, parent, converter, out string identifier)) {
							identifier = DefaultViewName;
						}

						//we store our name to don't generate dupplicates
						var lastIndex = GetLastInsertedIndex (identifier);
						if (lastIndex >= 0) {
							identifiers.Remove (identifier);
						}
						lastIndex++;

						node.Name = identifier;
						if (lastIndex > 0) {
							node.Name += lastIndex;
						}
						identifiers.Add (identifier, lastIndex);
					}

					builder.AppendLine();
					builder.AppendLine ($"// View:     {node.Name}");
					builder.AppendLine ($"// NodeName: {node.Node.name}");
					builder.AppendLine ($"// NodeType: {node.Node.type}");
					builder.AppendLine ($"// NodeId:   {node.Node.id}");

					OnPreConvertToCode (builder, node, parent, converter, codePropertyConverter);
					//we generate our code and replace node name

					var code = converter.ConvertToCode (node, parent, this);
					builder.AppendLineIfValue (code.Replace (Resources.Ids.Conversion.NameIdentifier, node.Name));
					OnPostConvertToCode (builder, node, parent, converter, codePropertyConverter);


					if (RendersAddChild(node, parent, this))
					{
						builder.AppendLineIfValue(codePropertyConverter.ConvertToCode(CodeProperties.AddChild, node, parent, this));
						OnChildAdded(builder, node, parent, converter, codePropertyConverter);
					}

					if (RendersSize(node, parent, this))
                    {
						builder.AppendLineIfValue(codePropertyConverter.ConvertToCode(CodeProperties.Frame, node, parent, this));
						OnFrameSet(builder, node, parent, converter, codePropertyConverter);
					}

					if (RendersConstraints(node, parent, this))
					{
						builder.AppendLineIfValue(codePropertyConverter.ConvertToCode(CodeProperties.Constraints, node, parent, this));
					}

					calculatedParentNode = node;
				} else {
					//without a converter we don't have any view created, we need to attach to the parent view
					calculatedParentNode = parent;
				}
			}

			//without converter we scan the children automatically
			var navigateChild = converter?.ScanChildren (node.Node) ?? true; 
			if (navigateChild && HasChildrenToRender (node)) {
				foreach (var item in GetChildrenToRender (node)) {
					var figmaNode = new FigmaCodeNode(item, parent: node);
					GetCode (builder, figmaNode, calculatedParentNode);
				}
			}

			if (MainNode == node) {
				//first loop
				Clear ();
			}
		}

        protected virtual bool RendersConstraints(FigmaCodeNode node, FigmaCodeNode parent, FigmaCodeRendererService figmaCodeRendererService)
        {
			return !((node != null && node == MainNode) || node.Node is FigmaCanvas || node.Node.Parent is FigmaCanvas);
		}

		protected virtual bool RendersSize(FigmaCodeNode node, FigmaCodeNode parent, FigmaCodeRendererService figmaCodeRendererService)
        {
			return true;
		}

		protected virtual bool RendersAddChild(FigmaCodeNode node, FigmaCodeNode parent, FigmaCodeRendererService figmaCodeRendererService)
        {
			return true;
        }

        protected virtual void OnStartGetCode ()
		{

		}

		protected virtual void OnPreConvertToCode (StringBuilder builder, FigmaCodeNode node, FigmaCodeNode parent, FigmaViewConverter converter, FigmaCodePropertyConverterBase codePropertyConverter)
		{
			
		}

		public bool NodeRendersVar (FigmaCodeNode currentNode, FigmaCodeNode parentNode)
        {
			if (currentNode.Node.GetNodeTypeName () == "mastercontent") {
				return false;
            }

			return !currentNode.Node.TryGetNodeCustomName(out var _);

		}

        protected virtual void OnPostConvertToCode (StringBuilder builder, FigmaCodeNode node, FigmaCodeNode parent, FigmaViewConverter converter, FigmaCodePropertyConverterBase codePropertyConverter)
		{

		}

		protected virtual void OnChildAdded (StringBuilder builder, FigmaCodeNode node, FigmaCodeNode parent, FigmaViewConverter converter, FigmaCodePropertyConverterBase codePropertyConverter)
		{

		}

		protected virtual void OnFrameSet (StringBuilder builder, FigmaCodeNode node, FigmaCodeNode parent, FigmaViewConverter converter, FigmaCodePropertyConverterBase codePropertyConverter)
		{

		}

		const string init = "Figma";
		const string end = "Converter";
		const string ViewIdentifier = "View";

		protected virtual bool TryGetCodeViewName (FigmaCodeNode node, FigmaCodeNode parent, FigmaViewConverter converter, out string identifier)
		{
			try {
				identifier = converter.GetType().Name;
				if (identifier.StartsWith (init)) {
					identifier = identifier.Substring (init.Length);
				}

				if (identifier.EndsWith (end)) {
					identifier = identifier.Substring (0, identifier.Length - end.Length);
				}

				identifier = char.ToLower (identifier[0]) + identifier.Substring (1) + ViewIdentifier;

				return true;
			} catch (Exception) {
				identifier = null;
				return false;
			}
		}

		internal int GetLastInsertedIndex (string identifier)
		{
			if (!identifiers.TryGetValue (identifier, out int data)) {
				return -1;
			}
			return data;
		}

		#region Rendering Iteration

		internal virtual bool NeedsRenderConstructor (FigmaCodeNode node, FigmaCodeNode parent)
		{
			if (parent != null
				&& IsMainNode (parent.Node)
				&& (CurrentRendererOptions?.RendersConstructorFirstElement ?? false)
				)
				return false;
			else {
				return true;
			}
		}

		internal virtual bool IsMainViewContainer (FigmaCodeNode node)
		{
			return true;
		}

		internal virtual FigmaNode[] GetChildrenToRender (FigmaCodeNode node)
		{
			if (node.Node is IFigmaNodeContainer nodeContainer) {
				return nodeContainer.children;
			}
			return new FigmaNode[0];
		}

		internal virtual bool HasChildrenToRender (FigmaCodeNode node)
		{
			return node.Node is IFigmaNodeContainer;
		}

		internal virtual bool IsNodeSkipped (FigmaCodeNode node)
		{
			return false;
		}

		#endregion

		Dictionary<string, int> identifiers = new Dictionary<string, int> ();
	}
}
