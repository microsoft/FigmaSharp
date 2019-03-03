using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FigmaSharp.Services
{
    public class FigmaCodeRendererService
    {
        protected FigmaFileService figmaFileService;
        FigmaCodePositionConverter codePositionConverter;
        FigmaCodeAddChildConverter codeAddChildConverter;

        public FigmaCodeRendererService(FigmaFileService figmaFileService)
        {
            this.figmaFileService = figmaFileService;
            codePositionConverter = AppContext.Current.GetPositionConverter();
            codeAddChildConverter = AppContext.Current.GetAddChildConverter();
        }

        public string GetCode(FigmaNode node, bool recursively = false)
        {
            var current = figmaFileService.NodesProcessed.FirstOrDefault(s => s.FigmaNode == node);
            if (current == null) return string.Empty;

			var name = TryAddIdentifier (node.GetType ());

			var builder = new StringBuilder();
            builder.AppendLine(current.Code.Replace("[NAME]", name));
            Recursively(builder, name, current);
            return builder.ToString ();
        }

		string TryAddIdentifier (Type type)
		{
			var iden = GetIdentifier (type);
			if (!identifiers.TryGetValue (iden, out int data)) {
				identifiers.Add (iden, 0);
				return iden;
			}

			identifiers.Remove (iden);
			identifiers.Add (iden, ++data);
			return iden + data;
		}

		string GetIdentifier (Type type)
		{
			if (type == typeof (FigmaLine)) {
				return "lineView";
			}
			if (type == typeof (FigmaStar)) {
				return "startView";
			}
			if (type == typeof (FigmaElipse)) {
				return "elipseView";
			}
			if (type == typeof (FigmaText)) {
				return "textView";
			}
			if (type == typeof (FigmaPath)) {
				return "pathView";
			}
			if (type == typeof (FigmaCanvas)) {
				return "canvasView";
			}
			if (type == typeof (FigmaGroup)) {
				return "groupView";
			}
			if (type == typeof (FigmaVector)) {
				return "vectorView";
			}
			if (type == typeof (FigmaVectorEntity)) {
				return "vectorEntityView";
			}
			if (type == typeof (FigmaFrameEntity)) {
				return "frameEntityView";
			}
			return "view";
		}

		Dictionary<string, int> identifiers = new Dictionary<string, int> ();

        public void Recursively (StringBuilder builder, string parent, ProcessedNode parentNode)
        {
            //we start to process all nodes
            var children = figmaFileService.NodesProcessed.Where(s => s.ParentView == parentNode);
            foreach (var child in children)
            {
				var name = TryAddIdentifier (child.FigmaNode.GetType ());
				builder.AppendLine(child.Code.Replace ("[NAME]", name));
                builder.AppendLine(codePositionConverter.ConvertToCode(name, parentNode, child));
                builder.AppendLine(codeAddChildConverter.ConvertToCode (parent, name, parentNode, child));
                Recursively(builder, name, child);
            }

            //if (children.Any())
            //{

            //    Recursively(processedNode, id, children);
            //}
        }
    }
}
