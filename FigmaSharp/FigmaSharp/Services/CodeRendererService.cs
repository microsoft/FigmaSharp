using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.PropertyConfigure;

namespace FigmaSharp.Services
{
	public class CodeRenderService :  RenderService
	{
		Dictionary<string, int> identifiers = new Dictionary<string, int>();

		const string init = "Figma";
		const string end = "Converter";
		const string ViewIdentifier = "View";

		internal const string DefaultViewName = "view";

		internal CodeNode MainNode { get; set; }
		internal CodeNode ParentMainNode { get; set; }

		internal INodeProvider figmaProvider;

		internal CodePropertyConfigureBase codePropertyConverter;
		readonly internal List<CodeNode> Nodes = new List<CodeNode>();

		internal CodeRenderServiceOptions CurrentRendererOptions { get; set; }

		public CodeRenderService (INodeProvider figmaProvider, NodeConverter[] figmaViewConverters,
			CodePropertyConfigureBase codePropertyConverter) : base (figmaProvider, figmaViewConverters)
		{
			this.codePropertyConverter = codePropertyConverter;
		}

		NodeConverter GetConverter (CodeNode node, List<NodeConverter> converters)
		{
			foreach (var customViewConverter in converters) {
				if (customViewConverter.CanConvert (node.Node)) {
					return customViewConverter;
				}
			}
			return null;
		}

		public bool IsMainNode (FigmaNode figmaNode) => MainNode != null && figmaNode == MainNode?.Node;

		public virtual void Clear ()
		{
			CurrentRendererOptions = null;
			ParentMainNode = null;
			MainNode = null;
		}

		public void GetCode (StringBuilder builder, CodeNode node, CodeNode parent = null, CodeRenderServiceOptions currentRendererOptions = null)
		{
			//in first level we clear all identifiers
			if (parent == null) {
				if (MainNode == null) {

					//clear all nodes
					Nodes.Clear();

					identifiers.Clear ();
					OnStartGetCode ();
					
					//we initialize
					CurrentRendererOptions = currentRendererOptions ?? new CodeRenderServiceOptions ();

					//we store our main node
					MainNode = node;
					ParentMainNode = parent;
				}
			}

			if (node != null)
				Nodes.Add(node);

			CodeNode calculatedParentNode = null;
			NodeConverter converter = null;

			var isNodeSkipped = IsNodeSkipped (node);

			//on node skipped we don't render
			if (!isNodeSkipped) {
				//if (figmaProvider.RendersProperties (node)) {
				converter = GetConverter (node, CustomConverters);
				//bool navigateChild = true;
				if (converter == null) {
					converter = GetConverter (node, DefaultConverters);
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

					builder.AppendLineIfValue(codePropertyConverter.ConvertToCode(PropertyNames.ViewInformation, node, parent, this));

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

        protected virtual bool RendersConstraints(CodeNode node, CodeNode parent, CodeRenderService figmaCodeRendererService)
        {
			return !((node != null && node == MainNode) || node.Node is FigmaCanvas || node.Node.Parent is FigmaCanvas);
		}

		protected virtual bool RendersSize(CodeNode node, CodeNode parent, CodeRenderService figmaCodeRendererService)
        {
			return true;
		}

		protected virtual bool RendersAddChild(CodeNode node, CodeNode parent, CodeRenderService figmaCodeRendererService)
        {
			return true;
        }

        protected virtual void OnStartGetCode ()
		{

		}

		protected virtual void OnPreConvertToCode (StringBuilder builder, CodeNode node, CodeNode parent, NodeConverter converter, CodePropertyConfigureBase codePropertyConverter)
		{
			
		}

		public virtual bool NodeRendersVar (CodeNode currentNode, CodeNode parentNode)
        {
			return !currentNode.Node.TryGetNodeCustomName(out var _);
		}

        protected virtual void OnPostConvertToCode (StringBuilder builder, CodeNode node, CodeNode parent, NodeConverter converter, CodePropertyConfigureBase codePropertyConverter)
		{

		}

		protected virtual void OnChildAdded (StringBuilder builder, CodeNode node, CodeNode parent, NodeConverter converter, CodePropertyConfigureBase codePropertyConverter)
		{

		}

		protected virtual void OnFrameSet (StringBuilder builder, CodeNode node, CodeNode parent, NodeConverter converter, CodePropertyConfigureBase codePropertyConverter)
		{

		}

		protected virtual bool TryGetCodeViewName (CodeNode node, CodeNode parent, NodeConverter converter, out string identifier)
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
	}
}
