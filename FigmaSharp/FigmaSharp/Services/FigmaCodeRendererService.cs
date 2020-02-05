using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FigmaSharp.Models;

namespace FigmaSharp.Services
{
	public class FigmaCodeRendererServiceOptions
	{
		public bool RendersConstructorFirstElement { get; set; }
	}

	public class FigmaCodeRendererService
	{
		internal const string DefaultViewName = "view";

		internal IFigmaFileProvider figmaProvider;

		FigmaCodePropertyConverterBase codePropertyConverter;

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

		public void Clear ()
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
					identifiers.Clear ();

					//we initialize
					CurrentRendererOptions = currentRendererOptions ?? new FigmaCodeRendererServiceOptions ();

					//we store our main node
					MainNode = node;
					ParentMainNode = parent;
				}
			} else {
				builder.AppendLine ();
			}

			FigmaCodeNode calculatedParentNode = null;
			FigmaViewConverter converter = null;

			var isNodeSkipped = figmaProvider.IsNodeSkipped (node);

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
						if (!converter.TryGetCodeViewName (node, parent, this, out string identifier)) {
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

					builder.AppendLine ($"// View: {node.Name} NodeName: {node.Node.name} NodeType: {node.Node.type} NodeId: {node.Node.id}");
					builder.AppendLine ();

					//we generate our code and replace node name
					var code = converter.ConvertToCode (node, parent, this);
					builder.AppendLineIfValue (code.Replace (Resources.Ids.Conversion.NameIdentifier, node.Name));

					builder.AppendLineIfValue (codePropertyConverter.ConvertToCode (CodeProperties.AddChild, node, parent, this));
					builder.AppendLineIfValue (codePropertyConverter.ConvertToCode (CodeProperties.Frame, node, parent, this));
					calculatedParentNode = node;
				} else {
					//without a converter we don't have any view created, we need to attach to the parent view
					calculatedParentNode = parent;
				}
			}

			//without converter we scan the children automatically
			var navigateChild = converter?.ScanChildren (node.Node) ?? true; 
			if (navigateChild && figmaProvider.HasChildrenToRender (node)) {
				foreach (var item in figmaProvider.GetChildrenToRender (node)) {
					GetCode (builder, new FigmaCodeNode (item, null), calculatedParentNode);
				}
			}

			if (MainNode == node) {
				//first loop
				Clear ();
			}
		}

		internal int GetLastInsertedIndex (string identifier)
		{
			if (!identifiers.TryGetValue (identifier, out int data)) {
				return -1;
			}
			return data;
		}

		Dictionary<string, int> identifiers = new Dictionary<string, int> ();
	}
}
