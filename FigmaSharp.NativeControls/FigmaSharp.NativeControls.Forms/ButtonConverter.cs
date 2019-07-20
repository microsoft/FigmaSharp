using System.Linq;
using System.Text;
using FigmaSharp.NativeControls.Base;
using FigmaSharp.Services;
using FigmaSharp.Models;
using FigmaSharp.Forms;
using FigmaSharp.Views;

namespace FigmaSharp.NativeControls.Forms
{
    public class ButtonConverter : ButtonConverterBase
    {
        public override IView ConvertTo (FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
        {
            var view = new Xamarin.Forms.Button();

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

            return new FigmaSharp.Views.Forms.View(view);
        }

        public override string ConvertToCode(FigmaNode currentNode, FigmaCodeRendererService rendererService)
        {
            return "var [NAME] = new Button();";
        }
    }
}
