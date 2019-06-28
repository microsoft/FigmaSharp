using System.Linq;
using System.Text;
using FigmaSharp.NativeControls.Base;
using Xamarin.Forms;
using FigmaSharp.Forms;
using FigmaSharp.Models;

namespace FigmaSharp.NativeControls.Forms
{
    public class ButtonConverter : ButtonConverterBase
    {
        public override IViewWrapper ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
            var view = new Button();

            view.Configure(currentNode);
   
            var keyValues = GetKeyValues(currentNode);
            foreach (var key in keyValues)
            {
                if (key.Key == "type")
                {
                    continue;
                }

                if (key.Key == "enabled")
                {
                    view.IsEnabled = key.Value == "true";
                }
            }
            if (currentNode is IFigmaDocumentContainer instance)
            {
                var figmaText = instance.children.OfType<FigmaText>().FirstOrDefault(s => s.name == "title");
                if (figmaText != null)
                {
                    view.Opacity = figmaText.opacity;
                    view.Font = figmaText.style.ToFont();
                    view.Text = figmaText.characters;
                }
            }

            return new ViewWrapper(view);
        }

        public override string ConvertToCode(FigmaNode currentNode)
        {
            return "var [NAME] = new Button();";
        }
    }
}
