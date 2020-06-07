using System;
using System.Linq;
using FigmaSharp.Converters;
using FigmaSharp.Forms;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace FigmaSharp.Controls.Forms.Converters
{
    public class ButtonConverter : NodeConverter
    {
        public override IView ConvertToView (FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
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

        public override string ConvertToCode(CodeNode currentNode, CodeNode parent, CodeRenderService rendererService)
        {
            return $"var [NAME] = new {typeof(Xamarin.Forms.Button).FullName}();";
        }

        public override Type GetControlType(FigmaNode currentNode) => typeof(Xamarin.Forms.Button);
        public override bool CanConvert(FigmaNode currentNode) => currentNode.name == "button";
    }
}
