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

using Xamarin.Forms;

using FigmaSharp.Converters;
using FigmaSharp.Forms;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;

namespace FigmaSharp.Controls.Forms.Converters
{
    public class TextFieldConverter : NodeConverter
	{
		public override IView ConvertToView (FigmaNode currentNode, ViewNode parent, ViewRenderService rendererService)
		{
			var view = new Entry ();
			var keyValues = GetKeyValues (currentNode);

			foreach (var key in keyValues) {
				if (key.Key == "type") {
					continue;
				}
				if (key.Key == "enabled") {
					view.IsEnabled = key.Value == "true";
				} else if (key.Key == "size") {
					//view.ControlSize = ToEnum<NSControlSize>(key.Value);
				}
			}

			if (currentNode is IFigmaDocumentContainer container) {
				var placeholderView = container.children.OfType<FigmaText> ()
			.FirstOrDefault (s => s.name == "placeholderstring");
				if (placeholderView != null) {
					view.Placeholder = placeholderView.characters;
				}

				var textFieldView = container.children.OfType<FigmaText> ()
				   .FirstOrDefault (s => s.name == "text");
				if (textFieldView != null) {
					view.Text = textFieldView.characters;
					view.Configure (textFieldView);
				} else {
					view.Configure (currentNode);
				}
			} else {
				view.Configure (currentNode);
			}

			return new Views.Forms.View (view);
		}


		public override string ConvertToCode(CodeNode currentNode, CodeNode parent, CodeRenderService rendererService)
		{
			return $"var [NAME] = new {typeof(Entry).FullName}();";
		}

		public override Type GetControlType(FigmaNode currentNode) => typeof(Xamarin.Forms.Button);
		public override bool CanConvert(FigmaNode currentNode) => currentNode.name == "textfield";
	}
}
