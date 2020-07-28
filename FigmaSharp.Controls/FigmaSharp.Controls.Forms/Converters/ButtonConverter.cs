// Authors:
//   Jose Medrano <josmed@microsoft.com>
//
// Copyright (C) 2018 Microsoft, Corp
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the
// following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
// NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
// USE OR OTHER DEALINGS IN THE SOFTWARE.

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
