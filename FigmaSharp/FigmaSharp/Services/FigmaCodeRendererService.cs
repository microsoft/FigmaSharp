using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FigmaSharp.Converters;
using FigmaSharp.Models;

namespace FigmaSharp.Services
{
	public class FigmaCodeRendererServiceOptions
	{
		public bool RendersConstructorFirstElement { get; set; }
		public bool TranslateLabels { get; set; }
	}

	public class FigmaCodeRendererService
	{
		internal const string DefaultViewName = "view";

		internal IFigmaFileProvider figmaProvider;

		internal CodePropertyNodeConfigureBase codePropertyConverter;

		LayerConverter[] figmaConverters;
		LayerConverter[] customConverters;

		public FigmaCodeRendererService (IFigmaFileProvider figmaProvider, LayerConverter[] figmaViewConverters,
			CodePropertyNodeConfigureBase codePropertyConverter)
		{
			this.customConverters = figmaViewConverters.Where (s => !s.IsLayer).ToArray ();
			this.figmaConverters = figmaViewConverters.Where (s => s.IsLayer).ToArray (); ;
			this.figmaProvider = figmaProvider;
			this.codePropertyConverter = codePropertyConverter;
		}

		LayerConverter GetConverter (CodeNode node, LayerConverter[] converters)
		{
			foreach (var customViewConverter in converters) {
				if (customViewConverter.CanConvert (node.Node)) {
					return customViewConverter;
				}
			}
			return null;
		}

		internal FigmaCodeRendererServiceOptions CurrentRendererOptions { get; set; }
		internal CodeNode ParentMainNode { get; set; }
		internal CodeNode MainNode { get; set; }

		public bool IsMainNode (FigmaNode figmaNode) => MainNode != null && figmaNode == MainNode?.Node;


		readonly internal List<CodeNode> Nodes = new List<CodeNode>();

		public virtual void Clear ()
		{
			CurrentRendererOptions = null;
			ParentMainNode = null;
			MainNode = null;
		}

		public void GetCode (StringBuilder builder, CodeNode node, CodeNode parent = null, FigmaCodeRendererServiceOptions currentRendererOptions = null)
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

			CodeNode calculatedParentNode = null;
			LayerConverter converter = null;

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
						builder.AppendLineIfValue(codePropertyConverter.ConvertToCode(PropertyNames.AddChild, node, parent, this));
						OnChildAdded(builder, node, parent, converter, codePropertyConverter);
					}

					if (RendersSize(node, parent, this))
                    {
						builder.AppendLineIfValue(codePropertyConverter.ConvertToCode(PropertyNames.Frame, node, parent, this));
						OnFrameSet(builder, node, parent, converter, codePropertyConverter);
					}

					if (RendersConstraints(node, parent, this))
					{
						builder.AppendLineIfValue(codePropertyConverter.ConvertToCode(PropertyNames.Constraints, node, parent, this));
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
					var figmaNode = new CodeNode(item, parent: node);
					GetCode (builder, figmaNode, calculatedParentNode);
				}
			}

			if (MainNode == node) {
				//first loop
				Clear ();
			}
		}

        protected virtual bool RendersConstraints(CodeNode node, CodeNode parent, FigmaCodeRendererService figmaCodeRendererService)
        {
			return !((node != null && node == MainNode) || node.Node is FigmaCanvas || node.Node.Parent is FigmaCanvas);
		}

		protected virtual bool RendersSize(CodeNode node, CodeNode parent, FigmaCodeRendererService figmaCodeRendererService)
        {
			return true;
		}

		protected virtual bool RendersAddChild(CodeNode node, CodeNode parent, FigmaCodeRendererService figmaCodeRendererService)
        {
			return true;
        }

        protected virtual void OnStartGetCode ()
		{

		}

		protected virtual void OnPreConvertToCode (StringBuilder builder, CodeNode node, CodeNode parent, LayerConverter converter, CodePropertyNodeConfigureBase codePropertyConverter)
		{
			
		}

		public bool NodeRendersVar (CodeNode currentNode, CodeNode parentNode)
        {
			if (currentNode.Node.GetNodeTypeName () == "mastercontent") {
				return false;
            }

			return !currentNode.Node.TryGetNodeCustomName(out var _);

		}

        protected virtual void OnPostConvertToCode (StringBuilder builder, CodeNode node, CodeNode parent, LayerConverter converter, CodePropertyNodeConfigureBase codePropertyConverter)
		{

		}

		protected virtual void OnChildAdded (StringBuilder builder, CodeNode node, CodeNode parent, LayerConverter converter, CodePropertyNodeConfigureBase codePropertyConverter)
		{

		}

		protected virtual void OnFrameSet (StringBuilder builder, CodeNode node, CodeNode parent, LayerConverter converter, CodePropertyNodeConfigureBase codePropertyConverter)
		{

		}

		const string init = "Figma";
		const string end = "Converter";
		const string ViewIdentifier = "View";

		protected virtual bool TryGetCodeViewName (CodeNode node, CodeNode parent, LayerConverter converter, out string identifier)
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

		internal virtual bool NeedsRenderConstructor (CodeNode node, CodeNode parent)
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

		internal virtual bool IsMainViewContainer (CodeNode node)
		{
			return true;
		}

		internal virtual FigmaNode[] GetChildrenToRender (CodeNode node)
		{
			if (node.Node is IFigmaNodeContainer nodeContainer) {
				return nodeContainer.children;
			}
			return new FigmaNode[0];
		}

		internal virtual bool HasChildrenToRender (CodeNode node)
		{
			return node.Node is IFigmaNodeContainer;
		}

		internal virtual bool IsNodeSkipped (CodeNode node)
		{
			return false;
		}

		#endregion

		Dictionary<string, int> identifiers = new Dictionary<string, int> ();
	}
}
