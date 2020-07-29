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

using AppKit;

using FigmaSharp.Cocoa.CodeGeneration;
using FigmaSharp.Cocoa.Helpers;
using FigmaSharp.Cocoa.PropertyConfigure;
using FigmaSharp.Converters;
using FigmaSharp.Models;
using FigmaSharp.PropertyConfigure;
using FigmaSharp.Services;

namespace FigmaSharp.Controls.Cocoa.PropertyConfigure
{
    public class ControlCodePropertyConfigure : CodePropertyConfigure
    {
        protected override string GetDefaultParentName (FigmaNode currentNode, FigmaNode parentNode, CodeRenderService rendererService)
        {
            //component instance window case
            if (currentNode.Parent.IsInstanceContent(rendererService.NodeProvider, out var figmaInstance))
            {
                if (figmaInstance.IsDialog())
                    return $"{Members.This}.{nameof(NSWindow.ContentView)}";
                return Members.This;
            }

            if (currentNode.Parent.IsComponentContent(rendererService.NodeProvider, out var figmaComponent))
            {
                //in components we find the base
                if (figmaComponent.TryGetInstanceDialogParentContainer (rendererService.NodeProvider, out var currentInstance))
                {
                    if (currentInstance.IsDialog())
                        return $"{Members.This}.{nameof(NSWindow.ContentView)}";
                }
                return Members.This;
            }

            //window case
            if (currentNode.Parent.IsWindowContent() || currentNode.Parent.IsDialogParentContainer())
            {
                var contentView = $"{Members.This}.{nameof(NSWindow.ContentView)}";
                return contentView;
            }

            if (currentNode.Parent.IsMainDocumentView())
            {
                return Members.This;
            }

            return null;
        }

        public override string ConvertToCode (string propertyName, CodeNode currentNode, CodeNode parentNode, NodeConverter converter, CodeRenderService rendererService)
        {
			if (currentNode.Node.Parent != null && propertyName == PropertyNames.AddChild) {

                var defaultParentName = GetDefaultParentName(currentNode.Node, currentNode.Node.Parent, rendererService);
                if (!string.IsNullOrEmpty (defaultParentName))
                    return CodeGenerationHelpers.GetMethod(defaultParentName, nameof(NSView.AddSubview), currentNode.Name);
            }
            return base.ConvertToCode (propertyName, currentNode, parentNode, converter, rendererService);
        }
    }
}
