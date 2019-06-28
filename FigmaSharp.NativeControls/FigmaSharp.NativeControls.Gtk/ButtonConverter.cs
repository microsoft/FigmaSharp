/* 
 * CustomButtonConverter.cs 
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using FigmaSharp.NativeControls.Base;
using Gtk;
using System.Linq;
using System.Text;
using FigmaSharp.GtkSharp;
using FigmaSharp.Models;

namespace FigmaSharp.NativeControls.GtkSharp
{
    public class ButtonConverter : ButtonConverterBase
    {
		public override IViewWrapper ConvertTo(FigmaNode currentNode, ProcessedNode parent)
        {
            var view = new Button();
			view.Label = "";

			view.Configure(currentNode);

			var keyValues = GetKeyValues (currentNode);
			foreach (var key in keyValues) {
				if (key.Key == "type") {
					continue;
				}

				if (key.Key == "enabled") {
					view.Sensitive = key.Value == "true";
				} else if (key.Key == "size") {
					//view.ControlSize = ToEnum<NSControlSize> (key.Value);
				} else if (key.Key == "style") {
					//view.BezelStyle = ToEnum<NSBezelStyle> (key.Value);
				} else if (key.Key == "buttontype") {
					//view.SetButtonType (ToEnum<NSButtonType> (key.Value));
				}
			}
			if (currentNode is IFigmaDocumentContainer instance) {
				var figmaText = instance.children.OfType<FigmaText> ().FirstOrDefault (s => s.name == "title");
				if (figmaText != null) {
					//view.Font = figmaText.style.ToFont ();
					view.Label = figmaText.characters;
				}

				var image = instance.children.OfType<FigmaVectorEntity> ().FirstOrDefault (s => s.name == "image");
				if (image != null) {
					var paint = image.fills.OfType<FigmaPaint> ().FirstOrDefault ();
					if (paint != null) {
						//var query = new FigmaImageQuery ()
						//FigmaApiHelper.GetFigmaImage (new FigmaImageQuery ())
					}
				}
			}
          
			return new ViewWrapper(view);
        }

        public override string ConvertToCode(FigmaNode currentNode)
        {
            var builder = new StringBuilder();
            var name = "[NAME]";
            builder.AppendLine($"var {name} = new Gtk.{nameof(Button)}();");
            builder.Configure(name, currentNode);

            var keyValues = GetKeyValues(currentNode);
            foreach (var key in keyValues)
            {
                if (key.Key == "type")
                {
                    continue;
                }

                if (key.Key == "enabled")
                {
                    var sensitive = (key.Value == "true").ToDesignerString();
                    builder.AppendLine(string.Format("{0}.Sensitive = \"{1}\";", name, sensitive));
                }
                else if (key.Key == "size")
                {
                    //TODO: not implemented
                }
                else if (key.Key == "style")
                {
                    //TODO: not implemented
                }
                else if (key.Key == "buttontype")
                {
                    //TODO: not implemented
                }
            }
            if (currentNode is IFigmaDocumentContainer instance)
            {
                var figmaText = instance.children.OfType<FigmaText>().FirstOrDefault(s => s.name == "title");
                if (figmaText != null)
                {
                    builder.AppendLine(string.Format("{0}.Text = \"{1}\";", name, figmaText.characters));
                }

                var image = instance.children.OfType<FigmaVectorEntity>().FirstOrDefault(s => s.name == "image");
                if (image != null)
                {
                    var paint = image.fills.OfType<FigmaPaint>().FirstOrDefault();
                    if (paint != null)
                    {
                        //TODO: not implemented
                        //var query = new FigmaImageQuery ()
                        //FigmaApiHelper.GetFigmaImage (new FigmaImageQuery ())
                    }
                }
            }

            return builder.ToString();
        }
    }
}
