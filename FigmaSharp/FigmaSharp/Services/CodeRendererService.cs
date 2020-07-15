using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.PropertyConfigure;

namespace FigmaSharp.Services
{
	public class CodeRenderService : RenderService
	{
		internal const string DefaultViewName = "view";

		internal CodePropertyConfigureBase codePropertyConverter;

		public CodeRenderService (INodeProvider figmaProvider, NodeConverter[] nodeConverters,
			CodePropertyConfigureBase codePropertyConverter, ITranslationService translationService = null) : base (figmaProvider, nodeConverters)
		{
			this.codePropertyConverter = codePropertyConverter;
			this.TranslationService = translationService;
		}

		NodeConverter GetConverter (CodeNode node, List<NodeConverter> converters)
		{
			foreach (var customViewConverter in converters) {
				if (customViewConverter.CanCodeConvert (node.Node)) {
					return customViewConverter;
				}
			}
			return null;
		}

		public ITranslationService TranslationService { get; internal set; }
		public CodeRenderServiceOptions Options { get; internal set; }
		internal CodeNode ParentMainNode { get; set; }
		internal CodeNode MainNode { get; set; }

		public bool IsMainNode (FigmaNode figmaNode) => MainNode != null && figmaNode == MainNode?.Node;

		readonly internal List<CodeNode> Nodes = new List<CodeNode>();

		public virtual void Clear ()
		{
			Options = null;
			ParentMainNode = null;
			MainNode = null;
		}

		public void GetCode (StringBuilder builder, CodeNode node, CodeNode parent = null, CodeRenderServiceOptions currentRendererOptions = null, ITranslationService translateService = null)
		{
			//in first level we clear all identifiers
			if (parent == null) {
				if (MainNode == null) {

					//clear all nodes
					Nodes.Clear();

					identifiers.Clear ();
					OnStartGetCode ();
					
					//we initialize
					Options = currentRendererOptions ?? new CodeRenderServiceOptions ();

					TranslationService = translateService ?? TranslationService ?? new DefaultTranslationService();

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
				converter = GetNodeConverter(node);

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

					if (Options.ShowComments)
                    {
						builder.AppendLine();
						builder.AppendLine($"// View:     {node.Name}");
						builder.AppendLine($"// NodeName: {node.Node.name}");
						builder.AppendLine($"// NodeType: {node.Node.type}");
						builder.AppendLine($"// NodeId:   {node.Node.id}");
					}

					OnPreConvertToCode (builder, node, parent, converter, codePropertyConverter);
					//we generate our code and replace node name

					var code = converter.ConvertToCode (node, parent, this);
					builder.AppendLineIfValue (code.Replace (Resources.Ids.Conversion.NameIdentifier, node.Name));
					OnPostConvertToCode (builder, node, parent, converter, codePropertyConverter);

					//TODO: this could be removed to converters base
					if (Options.ShowAddChild && RendersAddChild(node, parent, this))
					{
						builder.AppendLineIfValue(codePropertyConverter.ConvertToCode(PropertyNames.AddChild, node, parent, converter, this));
						OnChildAdded(builder, node, parent, converter, codePropertyConverter);
					}

					if (Options.ShowSize && RendersSize(node, parent, this))
                    {
						builder.AppendLineIfValue(codePropertyConverter.ConvertToCode(PropertyNames.Frame, node, parent, converter, this));
						OnFrameSet(builder, node, parent, converter, codePropertyConverter);
					}

					if (Options.ShowConstraints && RendersConstraints(node, parent, this))
					{
						builder.AppendLineIfValue(codePropertyConverter.ConvertToCode(PropertyNames.Constraints, node, parent, converter, this));
					}

					calculatedParentNode = node;
				} else {
					//without a converter we don't have any view created, we need to attach to the parent view
					calculatedParentNode = parent;
				}
			}

			//without converter we scan the children automatically
			var navigateChild = Options.ScanChildren && (converter?.ScanChildren (node.Node) ?? true); 
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

		public NodeConverter GetNodeConverter (CodeNode node)
        {
			var converter = GetConverter(node, CustomConverters);
			if (converter == null)
				converter = GetConverter(node, DefaultConverters);
			return converter;
		}

        internal virtual bool IsFlexibleVertical(CodeNode nodeNode, NodeConverter converter)
        {
			return converter.IsFlexibleVertical(nodeNode.Node);
		}

        internal virtual bool IsFlexibleHorizontal(CodeNode nodeNode, NodeConverter converter)
        {
			return converter.IsFlexibleHorizontal (nodeNode.Node);
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

        internal string GetTranslatedText(string text)
        {
            if (Options.TranslateLabels) {
				return TranslationService.GetTranslatedStringText(text);
			}
			return text;
		}

        public bool NodeRendersVar (CodeNode currentNode, CodeNode parentNode)
        {
			if (currentNode.Node.GetNodeTypeName () == "mastercontent") {
				return false;
            }

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

		const string init = "Figma";
		const string end = "Converter";
		const string ViewIdentifier = "View";

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
				&& (Options?.RendersConstructorFirstElement ?? false)
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
