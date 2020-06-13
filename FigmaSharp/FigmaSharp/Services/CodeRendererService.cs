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
		internal CodeNode MainNode { get; set; }
		internal CodeNode ParentMainNode { get; set; }

		internal INodeProvider NodeProvider;
		internal ICodeNameService NameService;
		internal CodePropertyConfigureBase PropertyConfigure;

		readonly internal List<CodeNode> Nodes = new List<CodeNode>();

		internal CodeRenderServiceOptions CurrentRendererOptions { get; set; }

		public CodeRenderService (INodeProvider figmaProvider, NodeConverter[] figmaViewConverters,
			CodePropertyConfigureBase codePropertyConverter, ICodeNameService codeNameService = null) : base (figmaProvider, figmaViewConverters)
		{
			this.NameService = codeNameService ?? AppContext.Current.GetCodeNameService();
			this.PropertyConfigure = codePropertyConverter;
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
					NameService.Clear();
					
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
				converter = CustomConverters.FirstOrDefault(s => s.CanConvert(node.Node));
				//bool navigateChild = true;
				if (converter == null) {
					converter = DefaultConverters.FirstOrDefault(s => s.CanConvert(node.Node));
				}

				if (converter != null) {
					if (!NameService.NodeHasName(node)) {
						node.Name = NameService.GenerateName(node, parent, converter);
					}

					builder.AppendLineIfValue(PropertyConfigure.ConvertToCode(PropertyNames.ViewInformation, node, parent, this));

					OnPreConvertToCode (builder, node, parent, converter, PropertyConfigure);
					//we generate our code and replace node name

					var code = converter.ConvertToCode (node, parent, this);
					builder.AppendLineIfValue (code.Replace (Resources.Ids.Conversion.NameIdentifier, node.Name));
					OnPostConvertToCode (builder, node, parent, converter, PropertyConfigure);

					if (RendersAddChild(node, parent, this))
					{
						builder.AppendLineIfValue(PropertyConfigure.ConvertToCode(PropertyNames.AddChild, node, parent, this));
						OnChildAdded(builder, node, parent, converter, PropertyConfigure);
					}

					if (RendersSize(node, parent, this))
                    {
						builder.AppendLineIfValue(PropertyConfigure.ConvertToCode(PropertyNames.Frame, node, parent, this));
						OnFrameSet(builder, node, parent, converter, PropertyConfigure);
					}

					if (RendersConstraints(node, parent, this))
					{
						builder.AppendLineIfValue(PropertyConfigure.ConvertToCode(PropertyNames.Constraints, node, parent, this));
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
