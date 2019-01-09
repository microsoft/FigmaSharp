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
            counter = 0;
            var current = figmaFileService.NodesProcessed.FirstOrDefault(s => s.FigmaNode == node);
            if (current == null) return string.Empty;

            var name = "view" + counter++;
            var builder = new StringBuilder();
            builder.AppendLine(current.Code.Replace("[NAME]", name));
            Recursively(builder, name, current);
            return builder.ToString ();
        }

        int counter;

        public void Recursively (StringBuilder builder, string parent, ProcessedNode parentNode)
        {
            //we start to process all nodes
            var children = figmaFileService.NodesProcessed.Where(s => s.ParentView == parentNode);
            foreach (var child in children)
            {
                var name = "view" + counter++;
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
