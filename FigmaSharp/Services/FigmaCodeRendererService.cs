using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FigmaSharp.Services
{
    public class FigmaCodeRendererService
    {
        protected IFigmaFileProvider figmaProvider;
        FigmaCodePositionConverter codePositionConverter;
        FigmaCodeAddChildConverter codeAddChildConverter;

        FigmaViewConverter[] figmaConverters;
        FigmaViewConverter[] customConverters;

        public FigmaCodeRendererService(IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters)
        {
            this.customConverters = figmaViewConverters.Where(s => !s.IsLayer).ToArray();
            this.figmaConverters = figmaViewConverters.Where(s => s.IsLayer).ToArray(); ;
            this.figmaProvider = figmaProvider;
            codePositionConverter = AppContext.Current.GetPositionConverter();
            codeAddChildConverter = AppContext.Current.GetAddChildConverter();
        }

        FigmaViewConverter GetConverter(FigmaNode node, FigmaViewConverter[] converters)
        {
            foreach (var customViewConverter in converters)
            {
                if (customViewConverter.CanConvert(node))
                {
                    return customViewConverter;
                }
            }
            return null;
        }

        public string GetCode (FigmaNode node, bool recursively = false)
        {
            var converter = GetConverter(node, customConverters);
            if (converter == null)
            {
                converter = GetConverter(node, figmaConverters);
            }

            if (converter != null)
            {
                var builder = new StringBuilder();
                var code = converter.ConvertToCode(node);
                var name = TryAddIdentifier(node.GetType());
                builder.AppendLine(code.Replace("[NAME]", name));

                if (recursively)
                {
                    Recursively(builder, name, node);
                }
                return builder.ToString();
            }

            return string.Empty;
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

        public void Recursively (StringBuilder builder, string parent, FigmaNode parentNode)
        {
            //we start to process all nodes
            var children = figmaProvider.Nodes.Where(s => s.Parent == parentNode);
            foreach (var child in children)
            {
				var name = TryAddIdentifier (child.GetType ());
                var code = GetCode(child, true);
                builder.AppendLine(code.Replace ("[NAME]", name));
                builder.AppendLine(codePositionConverter.ConvertToCode(name, child));
                builder.AppendLine(codeAddChildConverter.ConvertToCode (parent, name, child));
                Recursively(builder, name, child);
            }

        }
    }
}
