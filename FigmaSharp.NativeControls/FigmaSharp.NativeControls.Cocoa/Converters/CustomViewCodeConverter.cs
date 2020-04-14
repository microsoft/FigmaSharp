//
// Author:
//   netonjm <josmed@microsoft.com>
//
// Copyright (C) 2020 Microsoft, Corp
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
//
using System;
using System.Text;
using FigmaSharp.Models;
using FigmaSharp.Services;
using FigmaSharp.Views;
using FigmaSharp.Cocoa;
using System.Linq;

namespace FigmaSharp.NativeControls.Cocoa.Converters
{
	public class CustomViewCodeConverter : FigmaNativeControlConverter
	{
		public override Type GetControlType(FigmaNode currentNode)
		{
			var customType = GetIdentifierValue(currentNode.name, "type", true);
			if (customType != null) {
				return GetAssemblyType(customType);
			}
			return null;
		}

		public CustomViewCodeConverter()
		{
		}

		public override bool CanConvert(FigmaNode currentNode)
		{
			return currentNode.name.StartsWith("customview");
		}

		public Type GetAssemblyType (string fullname)
		{
			foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				var type = assembly.GetType(fullname);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		protected override StringBuilder OnConvertToCode(FigmaCodeNode currentNode, FigmaCodeNode parentNode, FigmaCodeRendererService rendererService)
		{
			var builder = new StringBuilder ();

			var type = GetControlType(currentNode.Node);
			if (type != null) {
				if (rendererService.NeedsRenderConstructor(currentNode, parentNode))
					builder.WriteConstructor(currentNode.Name, type, !currentNode.Node.TryGetNodeCustomName(out var _));
			}
			return builder;
		}

		protected override IView OnConvertToView(FigmaNode currentNode, ProcessedNode parent, FigmaRendererService rendererService)
		{
			return null;
		}

	}
}
