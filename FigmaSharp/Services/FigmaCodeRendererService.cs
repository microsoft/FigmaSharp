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
			if (!dict.TryGetValue (type, out string iden)) {
				iden = "view";
			}
			if (!identifiers.TryGetValue (iden, out int data)) {
				identifiers.Add (iden, 0);
				return iden;
			}

			identifiers.Remove (iden);
			identifiers.Add (iden, ++data);
			return iden + data;
		}

		Dictionary<Type, string> dict = new Dictionary<Type, string> () {
			{ typeof (FigmaLine), "lineView" },
			{ typeof (FigmaStar), "startView" },
			{ typeof (FigmaElipse), "elipseView" },
			{ typeof (FigmaText), "textView" },
			{ typeof (FigmaPath), "pathView" },
			{ typeof (FigmaCanvas), "canvasView" },
			{ typeof (FigmaGroup), "groupView" },
			{ typeof (FigmaVector), "vectorView" },
			{ typeof (FigmaVectorEntity), "vectorEntityView" },
			{ typeof (FigmaFrameEntity), "frameEntityView" },
	};

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

        }
    }
}
