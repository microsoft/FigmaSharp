using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FigmaSharp.Models;

namespace FigmaSharp.Services
{
    public class FigmaCodeRendererService
    {
        protected IFigmaFileProvider figmaProvider;
        FigmaCodePositionConverterBase codePositionConverter;
        FigmaCodeAddChildConverterBase codeAddChildConverter;

        FigmaViewConverter[] figmaConverters;
        FigmaViewConverter[] customConverters;

        public FigmaCodeRendererService(IFigmaFileProvider figmaProvider, FigmaViewConverter[] figmaViewConverters,
            FigmaCodePositionConverterBase codePositionConverter, FigmaCodeAddChildConverterBase codeAddChildConverter)
        {
            this.customConverters = figmaViewConverters.Where(s => !s.IsLayer).ToArray();
            this.figmaConverters = figmaViewConverters.Where(s => s.IsLayer).ToArray(); ;
            this.figmaProvider = figmaProvider;
            this.codePositionConverter = codePositionConverter;
            this.codeAddChildConverter = codeAddChildConverter;
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

        public void GetCode (StringBuilder builder, FigmaNode node, string name, string parent)
        {
            if (parent == null)
            {
                identifiers.Clear();
            }

            var converter = GetConverter(node, customConverters);

            bool navigateChild = true;
            if (converter == null)
            {
                converter = GetConverter(node, figmaConverters);
            }
            else
            {
                navigateChild = false;
            }

            if (converter != null)
            {
                var code = converter.ConvertToCode(node, this);

                if (name == null)
                {
                    name = TryAddIdentifier(node.GetType());
                }

                builder.AppendLine(code.Replace("[NAME]", name));
                if (parent != null)
                {
                    builder.AppendLine(codeAddChildConverter.ConvertToCode(parent, name, node));
                    builder.AppendLine(codePositionConverter.ConvertToCode(parent, name, node));
                }
            }

            if (navigateChild && node is IFigmaNodeContainer nodeContainer)
            {
                foreach (var item in nodeContainer.children)
                {
                    GetCode(builder, item, null, name);
                }
            }
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

    //    public void Recursively (StringBuilder builder, string parent, FigmaNode parentNode)
    //    {
    //        //we start to process all nodes
    //        var children = figmaProvider.Nodes.Where(s => s.Parent == parentNode);
    //        foreach (var child in children)
    //        {
				//var name = TryAddIdentifier (child.GetType ());
        //        var code = GetCode(child, name, true);
        //        builder.AppendLine(code.Replace ("[NAME]", name));
        //        builder.AppendLine(codeAddChildConverter.ConvertToCode(parent, name, child));
        //        builder.AppendLine(codePositionConverter.ConvertToCode (parent, name, child));
        //        Recursively(builder, name, child);
        //    }
        //}
    }
}
